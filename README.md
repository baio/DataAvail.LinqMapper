<html>
<head>
    <title>LinqMapper</title>
</head>
<body class="c2">
    <h2 class="c0 c6">
        <a name="h.4a7x3gse4a8c"></a><span>LinqMapper</span></h2>
    <h3 class="c0">
        <a name="h.bn3cnmr53cuy"></a><span>Problem</span></h3>
    <p class="c0">
        <span>The </span><span class="c4"><a class="c5" href="http://lostechies.com/jimmybogard/author/jimmybogard/">
            Jimmy Bogard</a></span><span>&rsquo;s </span><span>library </span><span class="c4"><a
                class="c5" href="http://automapper.org/">AutoMapper </a></span><span>is an excellent
                    tool but unfortunately it doesn&rsquo;t support projections via Linq - see description
                    of the problem by Jimmy himself </span><span class="c4"><a class="c5" href="http://lostechies.com/jimmybogard/2011/02/09/autoprojecting-linq-queries/">
                        here</a></span><span>. </span>
    </p>
    <p class="c0">
        <span>This library is attempt to naive solution of the problem defined. The top layer
            of this library is intentionally made similar to one of AutoMapper, but internals
            are very simplified and now supports only scare of the whole projections available
            in LINQ.</span></p>
    <h3 class="c0">
        <a name="h.azv582skil4x"></a><span>Create projections</span></h3>
    <p class="c0">
        <span>Similar to AutoMapper library the mapping by projection is created via</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>LinqMapper.CreateMap&lt;SrcType, DestType&gt;()</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>The line above create default mapping between type SrcType and DestType. </span>
    </p>
    <p class="c0">
        <span>Default mapping follow these rules:</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>1. Properties with the same name and type (simple) are mapped.</span></p>
    <p class="c0">
        <span>2. Properties with the same name and type (complex, if there is defined mapping
            for these types) are mapped.</span></p>
    <p class="c0">
        <span>3. Properties with the same name and both derived from IEnumerable of complex
            type (where defined mapping between them) are mapped</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>Now you can add additional mapping rules to the defaults.</span></p>
    <h3 class="c0">
        <a name="h.38b077fm43ph"></a><span>Custom mapping rules</span></h3>
    <h4 class="c0">
        <a name="h.mrbyqoow124s"></a><span>MapFrom </span>
    </h4>
    <p class="c0">
        <span>Map destination property from source&rsquo;s property defined in option.</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>LinqMapper.CreateMap&lt;SrcType, DestType&gt;()</span></p>
    <p class="c0">
        <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.MapFrom(p =&gt; p.DestName, opt
            =&gt; opt.MapFrom(s =&gt; s.SrcName));</span></p>
    <h4 class="c0">
        <a name="h.gi87sn2qlx4c"></a><span>Ignore </span>
    </h4>
    <p class="c0">
        <span>Ignore mapping for property.</span></p>
    <p class="c0 c1">
        <span></span>
    </p>
    <p class="c0">
        <span>LinqMapper.CreateMap&lt;SrcType, DestType&gt;()</span></p>
    <p class="c0">
        <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.MapFrom(p =&gt; p.DestName, opt
            =&gt; opt.Ignore());</span></p>
    <h4 class="c0">
        <a name="h.kio6qmf1nrqn"></a><span>ResolveUsing </span>
    </h4>
    <p class="c0">
        <span>Custom mapping between source and destination.</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>LinqMapper.CreateMap&lt;SrcType, DestType&gt;()</span></p>
    <p class="c0">
        <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.MapFrom(p =&gt; p.DestName, opt
            =&gt; opt.ResolveUsing( s =&gt; s.SrcName + &ldquo;I see you&rdquo;));</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>Custom mapping should be defined in fashion known by underlying LINQ provider.</span></p>
    <h3 class="c0">
        <a name="h.s3im331wdcyl"></a><span>Mapping</span></h3>
    <p class="c0">
        <span>IQueryable&lt;TSrc&gt; dest = LinqMapper.</span><span>Map&lt;TSrc, TDest&gt;(IQueryable&lt;TSrc&gt;
            Src, params string [] Expands)</span><span class="c3">&nbsp;</span></p>
    <p class="c1 c0">
        <span class="c3"></span>
    </p>
    <p class="c0">
        <span>Map source query to destination by defined rules. </span>
    </p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>The second parameter define which complex properties should be projected for query.
        </span>
    </p>
    <p class="c0">
        <span>For example if Item source entity has Parent property (also entity) - one should
            be defined in the list of the Expands in order to be selected from underlying data
            source (same for the properties with enumerable types).</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
    <p class="c0">
        <span>LinqMapper.Map&lt;TSrc, TDest&gt;(srcQueryable, &ldquo;Parent&rdquo;)</span><span
            class="c3">&nbsp;</span></p>
    <p class="c1 c0">
        <span></span>
    </p>
</body>
</html>
