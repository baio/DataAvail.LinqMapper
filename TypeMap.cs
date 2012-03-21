using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using DataAvail.Utils;

namespace DataAvail.LinqMapper
{
    internal static class TypeMap
    {
        internal static Type GetGenericEnumerableType(Type Enumerable)
        {
            if (Enumerable.IsGenericType && Enumerable.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return Enumerable.GetGenericArguments()[0];

            foreach (Type interfaceType in Enumerable.GetInterfaces())
            {
                if (interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }

            return null;
        }

        internal static bool IsValidTypePair(Type SrcPropertyType, Type DestPropertyType)
        {
            if (SrcPropertyType == DestPropertyType)
                return true; //Simple Or Complex type

            var t1 = GetGenericEnumerableType(SrcPropertyType);
            var t2 = GetGenericEnumerableType(DestPropertyType);

            if (t1 != null && t2 != null)
                return true;

            if (Mapper.Find(SrcPropertyType, DestPropertyType) != null)
                return true;

            return false;
        }

        internal static Expression Bind(PropertyInfo SrcPropertyInfo, PropertyInfo DestPropertyInfo, Expression ParameterExpression)
        {
            MemberExpression memberAccessExpr = null;

            var mapType = GetMapType(SrcPropertyInfo, DestPropertyInfo);

            switch (mapType)
            {
                case PropertyMapType.Simple:
                    return Expression.Property(ParameterExpression, SrcPropertyInfo);
                case PropertyMapType.List:
                    var srcType = GetGenericEnumerableType(SrcPropertyInfo.PropertyType);
                    var destType = GetGenericEnumerableType(DestPropertyInfo.PropertyType);
                    var lambdaExpr = BuildExpression(srcType, destType);
                    memberAccessExpr = Expression.MakeMemberAccess(ParameterExpression, SrcPropertyInfo);
                    var selectCall = Expression.Call(typeof(Enumerable), "Select", new Type[] { srcType, destType }, memberAccessExpr, lambdaExpr);
                    return selectCall;
                case PropertyMapType.Complex:
                    memberAccessExpr = Expression.Property(ParameterExpression, SrcPropertyInfo);
                    return BuildMemberInitExpression(SrcPropertyInfo.PropertyType, DestPropertyInfo.PropertyType, memberAccessExpr);
            }

            throw new Exception("Can't determine propertie's bind type");
        }

        internal static bool IsMapped(PropertyInfo SrcPropertyInfo, PropertyInfo DestPropertyInfo, string[] Expands)
        {
            var mapType = GetMapType(SrcPropertyInfo, DestPropertyInfo);

            return mapType != PropertyMapType.Unmapped &&
                (mapType != PropertyMapType.Complex && mapType != PropertyMapType.List || Expands.Select(p=>p.Split('/')[0]).Contains(DestPropertyInfo.Name));
        }

        private static PropertyMapType GetMapType(PropertyInfo SrcPropertyInfo, PropertyInfo DestPropertyInfo)
        {
                if (SrcPropertyInfo == null)
                    return PropertyMapType.Unmapped;

                var srcType = SrcPropertyInfo.PropertyType;
                var destType = DestPropertyInfo.PropertyType;

                if (srcType == destType && Reflection.IsPrimitive(srcType))
                    //&& (srcType == typeof(string) || srcType.IsPrimitive || DataAvail. srcType.IsNu))
                {
                    return PropertyMapType.Simple;
                }

                var t1 = TypeMap.GetGenericEnumerableType(srcType);
                var t2 = TypeMap.GetGenericEnumerableType(destType);

                if (t1 != null && t2 != null && Mapper.Find(t1, t2) != null)
                    return PropertyMapType.List;

                if (Mapper.Find(srcType, destType) != null)
                    return PropertyMapType.Complex;

                return PropertyMapType.Undefined;
        }

        private static Expression BuildExpression(Type SrcType, Type DestType)
        {
            var mapping = Mapper.Find(SrcType, DestType);

            if (mapping == null)
                throw new Exception(string.Format("Mapping between {0} and {1} not found", SrcType.Name, DestType.Name));

            return mapping.BuildExpression();
        }

        private static Expression BuildMemberInitExpression(Type SrcType, Type DestType, Expression Expression)
        {
            var mapping = Mapper.Find(SrcType, DestType);

            if (mapping == null)
                throw new Exception(string.Format("Mapping between {0} and {1} not found", SrcType.Name, DestType.Name));

            return mapping.BuildMemberInitExpression(Expression, new string[0]);
        }
    }

    public enum PropertyMapType
    {
        Undefined,

        Unmapped,

        Simple,

        Complex,

        List,

        Custom
    }
}
