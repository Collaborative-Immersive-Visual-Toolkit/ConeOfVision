d3.functor = function functor(v) {
  return typeof v === "function" ? v : function() {
    return v;
  };
};

d3.tip = function() {

  var direction = d3_tip_direction,
    offset = d3_tip_offset,
    html = d3_tip_html,
    node = initNode(),
    svg = null,
    point = null,
    target = null

  function tip(vis) {
    svg = getSVGNode(vis)
    point = svg.createSVGPoint()
    document.body.appendChild(node)
  }

  // Public - show the tooltip on the screen
  //
  // Returns a tip
  tip.show = function() {
    var args = Array.prototype.slice.call(arguments)
    if (args[args.length - 1] instanceof SVGElement) target = args.pop()

    var content = html.apply(this, args),
      poffset = offset.apply(this, args),
      dir = direction.apply(this, args),
      nodel = getNodeEl(),
      i = directions.length,
      coords,
      scrollTop = document.documentElement.scrollTop || document.body.scrollTop,
      scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft

    nodel.html(content)
      .style('position', 'absolute')
      .style('opacity', 1)
      .style('pointer-events', 'all')

    while (i--) nodel.classed(directions[i], false)
    coords = direction_callbacks[dir].apply(this)
    nodel.classed(dir, true)
      .style('top', (coords.top + poffset[0]) + scrollTop + 'px')
      .style('left', (coords.left + poffset[1]) + scrollLeft + 'px')

    return tip
  }

  // Public - hide the tooltip
  //
  // Returns a tip
  tip.hide = function() {
    var nodel = getNodeEl()
    nodel
      .style('opacity', 0)
      .style('pointer-events', 'none')
    return tip
  }

  // Public: Proxy attr calls to the d3 tip container.  Sets or gets attribute value.
  //
  // n - name of the attribute
  // v - value of the attribute
  //
  // Returns tip or attribute value
  tip.attr = function(n, v) {
    if (arguments.length < 2 && typeof n === 'string') {
      return getNodeEl().attr(n)
    } else {
      var args = Array.prototype.slice.call(arguments)
      d3.selection.prototype.attr.apply(getNodeEl(), args)
    }

    return tip
  }

  // Public: Proxy style calls to the d3 tip container.  Sets or gets a style value.
  //
  // n - name of the property
  // v - value of the property
  //
  // Returns tip or style property value
  tip.style = function(n, v) {
    // debugger;
    if (arguments.length < 2 && typeof n === 'string') {
      return getNodeEl().style(n)
    } else {
      var args = Array.prototype.slice.call(arguments);
      if (args.length === 1) {
        var styles = args[0];
        Object.keys(styles).forEach(function(key) {
          return d3.selection.prototype.style.apply(getNodeEl(), [key, styles[key]]);
        });
      }
    }

    return tip
  }

  // Public: Set or get the direction of the tooltip
  //
  // v - One of n(north), s(south), e(east), or w(west), nw(northwest),
  //     sw(southwest), ne(northeast) or se(southeast)
  //
  // Returns tip or direction
  tip.direction = function(v) {
    if (!arguments.length) return direction
    direction = v == null ? v : d3.functor(v)

    return tip
  }

  // Public: Sets or gets the offset of the tip
  //
  // v - Array of [x, y] offset
  //
  // Returns offset or
  tip.offset = function(v) {
    if (!arguments.length) return offset
    offset = v == null ? v : d3.functor(v)

    return tip
  }

  // Public: sets or gets the html value of the tooltip
  //
  // v - String value of the tip
  //
  // Returns html value or tip
  tip.html = function(v) {
    if (!arguments.length) return html
    html = v == null ? v : d3.functor(v)

    return tip
  }

  // Public: destroys the tooltip and removes it from the DOM
  //
  // Returns a tip
  tip.destroy = function() {
    if (node) {
      getNodeEl().remove();
      node = null;
    }
    return tip;
  }

  function d3_tip_direction() {
    return 'n'
  }

  function d3_tip_offset() {
    return [0, 0]
  }

  function d3_tip_html() {
    return ' '
  }

  var direction_callbacks = {
    n: direction_n,
    s: direction_s,
    e: direction_e,
    w: direction_w,
    nw: direction_nw,
    ne: direction_ne,
    sw: direction_sw,
    se: direction_se
  };

  var directions = Object.keys(direction_callbacks);

  function direction_n() {
    var bbox = getScreenBBox()
    return {
      top: bbox.n.y - node.offsetHeight,
      left: bbox.n.x - node.offsetWidth / 2
    }
  }

  function direction_s() {
    var bbox = getScreenBBox()
    return {
      top: bbox.s.y,
      left: bbox.s.x - node.offsetWidth / 2
    }
  }

  function direction_e() {
    var bbox = getScreenBBox()
    return {
      top: bbox.e.y - node.offsetHeight / 2,
      left: bbox.e.x
    }
  }

  function direction_w() {
    var bbox = getScreenBBox()
    return {
      top: bbox.w.y - node.offsetHeight / 2,
      left: bbox.w.x - node.offsetWidth
    }
  }

  function direction_nw() {
    var bbox = getScreenBBox()
    return {
      top: bbox.nw.y - node.offsetHeight,
      left: bbox.nw.x - node.offsetWidth
    }
  }

  function direction_ne() {
    var bbox = getScreenBBox()
    return {
      top: bbox.ne.y - node.offsetHeight,
      left: bbox.ne.x
    }
  }

  function direction_sw() {
    var bbox = getScreenBBox()
    return {
      top: bbox.sw.y,
      left: bbox.sw.x - node.offsetWidth
    }
  }

  function direction_se() {
    var bbox = getScreenBBox()
    return {
      top: bbox.se.y,
      left: bbox.e.x
    }
  }

  function initNode() {
    var node = d3.select(document.createElement('div'))
    node
      .style('position', 'absolute')
      .style('top', '0')
      .style('opacity', '0')
      .style('pointer-events', 'none')
      .style('box-sizing', 'border-box')

    return node.node()
  }

  function getSVGNode(el) {
    el = el.node()
    if (el.tagName.toLowerCase() === 'svg')
      return el

    return el.ownerSVGElement
  }

  function getNodeEl() {
    if (node === null) {
      node = initNode();
      // re-add node to DOM
      document.body.appendChild(node);
    };
    return d3.select(node);
  }

  // Private - gets the screen coordinates of a shape
  //
  // Given a shape on the screen, will return an SVGPoint for the directions
  // n(north), s(south), e(east), w(west), ne(northeast), se(southeast), nw(northwest),
  // sw(southwest).
  //
  //    +-+-+
  //    |   |
  //    +   +
  //    |   |
  //    +-+-+
  //
  // Returns an Object {n, s, e, w, nw, sw, ne, se}
  function getScreenBBox() {
    var targetel = target || d3.event.target;

    while ('undefined' === typeof targetel.getScreenCTM && 'undefined' === targetel.parentNode) {
      targetel = targetel.parentNode;
    }

    var bbox = {},
      matrix = targetel.getScreenCTM(),
      tbbox = targetel.getBBox(),
      width = tbbox.width,
      height = tbbox.height,
      x = tbbox.x,
      y = tbbox.y

    point.x = x
    point.y = y
    bbox.nw = point.matrixTransform(matrix)
    point.x += width
    bbox.ne = point.matrixTransform(matrix)
    point.y += height
    bbox.se = point.matrixTransform(matrix)
    point.x -= width
    bbox.sw = point.matrixTransform(matrix)
    point.y -= height / 2
    bbox.w = point.matrixTransform(matrix)
    point.x += width
    bbox.e = point.matrixTransform(matrix)
    point.x -= width / 2
    point.y -= height / 2
    bbox.n = point.matrixTransform(matrix)
    point.y += height
    bbox.s = point.matrixTransform(matrix)

    return bbox
  }

  return tip
};

var url = "https://docs.google.com/spreadsheets/d/1eMr2KPi2D6vHa-j95XTGrD4YcfjzmJMm4usyv2Ebc0c/gviz/tq?";
var url1 = "https://docs.google.com/spreadsheets/d/1GVQZSWDZb--Lo8zQ6el2tdGNlO95QB2Yye7hY-4m2sE/gviz/tq?";
var url3 = "https://docs.google.com/spreadsheets/d/1EZdprGdJyL9kgyJDPLAvHQu4kpiLZQ5kIxR2XIcgveo/gviz/tq?";
var wwidth = $( window ).width();
var wheight = $( window ).height(); 

var margin = { top: 20, right: 10, bottom: 40, left: 50 },
width = (wwidth/3)-16 - margin.left - margin.right,
height = wheight/2 - margin.top - margin.bottom;

$('#sizeContainer').text("width: "+wwidth+ " height: "+wheight);

var title = $(location).attr("href").split("/")[3].split(".")[0] ;

//first dataset


if( title == "Histograms"){

  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);

    margin.bottom = 60;
  
    width = (wwidth/3)-16 - margin.left - margin.right,
    height = wheight/2 - margin.top - margin.bottom-10;

    Histogram(data, "duration");
    Histogram(data, "genre");
    Histogram(data, "country");
    Histogram(data, "language");
    Histogram(data, "audience_score");
    Histogram(data, "metascore");
  });
}

if( title == "BoxAndWhiskers2"){
    

  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,340);


      var textHeight = 0;
      width = (wwidth/2)-20 - margin.left - margin.right,
      margin.bottom=120,
      height = (wheight-textHeight)/2 - margin.top - (margin.bottom+15);

      BoxAndWhiskers(data, "production_company", "profitability",width,height)
      BoxAndWhiskers(data, "production_company", "worldwide_gross",width,height)
      BoxAndWhiskers(data, "production_company", "audience_score",width,height)
      BoxAndWhiskers(data, "production_company", "metascore",width,height)

    });
}

if( title == "BoxAndWhiskers"){

  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,340);
    
    margin.bottom = 60;
    var textHeight = 0;
    width = (wwidth/3)-16 - margin.left - margin.right,
    height = (wheight-textHeight)/2 - margin.top - margin.bottom-10;

    BoxAndWhiskers(data, "genre", "profitability",width,height)	
    BoxAndWhiskers(data, "genre", "budget",width,height)
    BoxAndWhiskers(data, "genre","worldwide_gross",width,height);
    BoxAndWhiskers(data, "genre", "audience_score",width,height)
    BoxAndWhiskers(data, "genre","metascore",width,height);
    BoxAndWhiskers(data, "genre", "duration",width,height);
  });

} 

if( title == "Oscar"){

  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,340);
  
    margin.bottom = 60;
    var textHeight = 0;
    width = (wwidth/3)-16 - margin.left - margin.right,
    height = (wheight-textHeight)/2 - margin.top - margin.bottom-10;

    BoxAndWhiskers(data,"oscar","metascore",width,height);
    BoxAndWhiskers(data,"oscar","audience_score",width,height);
    BoxAndWhiskers(data,"oscar","profitability",width,height);
    BoxAndWhiskers(data,"oscar","worldwide_gross",width,height);
    BoxAndWhiskers(data,"oscar","budget",width,height);
    BoxAndWhiskers(data,"oscar","duration",width,height);
  });
}

if( title == "Scatterplot1"){

  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,340);

  legend({
    color: d3.scaleSequential([22, 2], d3.interpolateWarm),
    title: "Profitability",
    title2: "Worldwidee Gross",
    marginLeft: margin.left
  })

  var textHeight = 0;
    width = (wwidth/3)-16 - margin.left - margin.right;
    margin.bottom=45;
    height = (wheight-textHeight)/2 - margin.top - margin.bottom - 50;

    ScatterPlot(data, "males_audience_score", "females_audience_score", "worldwide_gross", "profitability", "title", true);
    ScatterPlot(data,"us_audience_score","non_us_audience_score","worldwide_gross","profitability","title",true);
    ScatterPlot(data,"audience_score","metascore","worldwide_gross","profitability","title");
    ScatterPlot(data,"budget","worldwide_gross","worldwide_gross","profitability","title");
    ScatterPlot(data,"profitability","worldwide_gross","worldwide_gross","profitability","title");
    ScatterPlot(data,"profitability","budget","worldwide_gross","profitability","title");

    setclickforcircles();
    });
 }

if( title == "Scatterplot2"){


  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,340);

      legend({
        color: d3.scaleSequential([22, 2], d3.interpolateWarm),
        title: "Profitability",
        title2: "Worldwidee Gross",
        marginLeft: margin.left
      })

      var textHeight = 0;
      width = (wwidth/3)-16 - margin.left - margin.right;
      margin.bottom=40;
      height = (wheight-textHeight)/2 - margin.top - margin.bottom - 50;


      ScatterPlot(data,"audience_score","profitability","worldwide_gross","profitability","title");
      ScatterPlot(data,"audience_score","worldwide_gross","worldwide_gross","profitability","title");
      ScatterPlot(data,"audience_score","budget","worldwide_gross","profitability","title");
      ScatterPlot(data,"metascore","profitability","worldwide_gross","profitability","title");
      ScatterPlot(data,"metascore","worldwide_gross","worldwide_gross","profitability","title");	
      ScatterPlot(data,"metascore","budget","worldwide_gross","profitability","title");	

      setclickforcircles();
    });

}

if( title == "StackBarChart"){

  fetch(url)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,100);

    width = (wwidth/2)-16 - margin.left - margin.right,
    margin.bottom=70,
    height = wheight/2 - margin.top - (margin.bottom+10);

    StackedBarPlot(data,"genre","country");
    StackedBarPlot(data,"genre","language");
    StackedBarPlot(data,"oscar","country");
    StackedBarPlot(data,"oscar","language");
   
    
  });
}

  //second  dataset
if( title == "Histograms_Gender"){


  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);
      
  
      margin.bottom = 60;
    
      width = (wwidth/3)-16 - margin.left - margin.right,
      height = wheight/2 - margin.top - margin.bottom-10;
  
      Histogram(data, "bechdel_clean_test");
      Histogram(data, "bechdel_binary");
      Histogram(data, "female_words");
      Histogram(data, "male_words");
      Histogram(data, "female_words");
      Histogram(data, "total_words");

    });
}
  
if( title == "Scatterplot1_Gender"){

  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,300);
      
      
    legend({
      color: d3.scaleSequential([85,0], d3.interpolateWarm),
      title: "female_words_percentage",
      title2: "total_words",
      marginLeft: margin.left
    })
      
      width = (wwidth/3)-16 - margin.left - margin.right;
      margin.bottom=55;
      height = wheight/2 - margin.top - margin.bottom - 50;
  
      ScatterPlot(data, "female_words", "male_words", "total_words", "female_words_percentage", "title", true);
      ScatterPlot(data, "total_words", "male_words", "total_words", "female_words_percentage", "title", true);
      ScatterPlot(data, "total_words", "female_words", "total_words", "female_words_percentage", "title", true);
      ScatterPlot(data, "budget", "female_words", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "domestic_gross", "female_words", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "international_gross", "female_words", "total_words", "female_words_percentage", "title", false);


      setclickforcircles();
      });
}

if( title == "Scatterplot2_Gender"){

  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,300);
      
      
    legend({
      color: d3.scaleSequential([85,0], d3.interpolateWarm),
      title: "female_words_percentage",
      title2: "total_words",
      marginLeft: margin.left
    })
      
      width = (wwidth/3)-16 - margin.left - margin.right;
      margin.bottom=55;
      height = wheight/2 - margin.top - margin.bottom - 50;
  
      ScatterPlot(data, "female_words", "median_rating", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "male_words", "median_rating", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "total_words", "median_rating", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "budget", "male_words", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "domestic_gross", "male_words", "total_words", "female_words_percentage", "title", false);
      ScatterPlot(data, "international_gross", "male_words", "total_words", "female_words_percentage", "title", false);


      setclickforcircles();
      });
}

if( title == "BoxAndWhiskers_Gender"){

  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,300);

      data = removeCommaSeparetedArrays(data,"genre");
      data = removeCommaSeparetedArrays(data,"country");

      width = (wwidth/2)-16 - margin.left - margin.right,
      margin.bottom=120,
      height = wheight/2 - margin.top - (margin.bottom+10);

      BoxAndWhiskers(data, "production_company", "female_words",width,height)
      BoxAndWhiskers(data, "production_company", "male_words",width,height)
      BoxAndWhiskers(data, "production_company", "total_words",width,height)
      BoxAndWhiskers(data, "production_company", "female_words_percentage",width,height)

    });
}

if( title == "BoxAndWhiskers2_Gender"){
    

  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,300);
      data = removeCommaSeparetedArrays(data,"genre");
      data = removeCommaSeparetedArrays(data,"country");
        
      margin.bottom = 60;
    
      width = (wwidth/3)-16 - margin.left - margin.right,
      height = wheight/2 - margin.top - margin.bottom-10;
  
      BoxAndWhiskers(data, "genre", "female_words",width,height)	;
      BoxAndWhiskers(data, "genre", "female_words_percentage",width,height);
      BoxAndWhiskers(data, "genre", "median_rating",width,height);
      BoxAndWhiskers(data, "genre", "male_words",width,height);
      BoxAndWhiskers(data, "genre", "total_words",width,height);     
      BoxAndWhiskers(data, "genre", "international_gross",width,height);
    });
}

if( title == "Oscar_Gender"){
    

  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,300);
    margin.bottom = 60;
  
    width = (wwidth/3)-16 - margin.left - margin.right,
    height = wheight/2 - margin.top - margin.bottom-10;

    BoxAndWhiskers(data,"oscar","female_words",width,height);
    BoxAndWhiskers(data,"oscar","female_words_percentage",width,height);
    BoxAndWhiskers(data,"oscar","median_rating",width,height);
    BoxAndWhiskers(data,"oscar","male_words",width,height);
    BoxAndWhiskers(data,"oscar","total_words",width,height);
    BoxAndWhiskers(data,"oscar","international_gross",width,height);

  });
}

if( title == "StackBarChart_Gender"){


  fetch(url1)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,300);

    var textHeight = 0;
    width = (wwidth/2)-16 - margin.left - margin.right;
    margin.bottom=70;
    height = (wheight-textHeight)/2 - margin.top - (margin.bottom+10);

    StackedBarPlot(data,"genre","bechdel_binary");
    StackedBarPlot(data,"genre","bechdel_clean_test");
    StackedBarPlot(data,"oscar","bechdel_binary");
    StackedBarPlot(data,"oscar","bechdel_clean_test");
   
    
  });
}

 //third  dataset
 if( title == "Histograms_Third"){


  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);
      
      
      margin.bottom = 60;
    
      width = (wwidth/3)-16 - margin.left - margin.right,
      height = wheight/2 - margin.top - margin.bottom-10;
  
      Histogram(data, "length");
      Histogram(data, "width");
      Histogram(data, "horsepower");
      Histogram(data, "peak-rpm");
      Histogram(data, "city-mpg");
      Histogram(data, "highway-mpg");
     


  });
}
  
if( title == "Scatterplot1_Third"){

  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);
      
  
    legend({
      color: d3.scaleSequential([3, -3], d3.interpolateWarm),
      title: "insurance risk rating",
      title2: "losees",
      marginLeft: margin.left
    })
  
    var textHeight = 0;
      width = (wwidth/3)-16 - margin.left - margin.right;
      margin.bottom=45;
      height = (wheight-textHeight)/2 - margin.top - margin.bottom - 50;
  
      ScatterPlot(data,"normalized-losees", "engine-size", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"normalized-losees", "width", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"normalized-losees", "horsepower", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"normalized-losees", "compression-ratio", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"normalized-losees", "price", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"normalized-losees", "highway-mpg", "normalized-losees", "insurance_risk_rating", "make", false);

  
      setclickforcircles();
      });
}

if( title == "Scatterplot2_Third"){

 
  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);
      
  
    legend({
      color: d3.scaleSequential([3, -3], d3.interpolateWarm),
      title: "insurance risk rating",
      title2: "losees",
      marginLeft: margin.left
    })
  
    var textHeight = 0;
      width = (wwidth/3)-16 - margin.left - margin.right;
      margin.bottom=45;
      height = (wheight-textHeight)/2 - margin.top - margin.bottom - 50;
  
      ScatterPlot(data,"insurance_risk_rating", "engine-size", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"insurance_risk_rating", "width", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"insurance_risk_rating", "horsepower", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"insurance_risk_rating", "compression-ratio", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"insurance_risk_rating", "price", "normalized-losees", "insurance_risk_rating", "make", false);
      ScatterPlot(data,"insurance_risk_rating", "highway-mpg", "normalized-losees", "insurance_risk_rating", "make", false);

  
      setclickforcircles();
      });
}

if( title == "BoxAndWhiskers_Third"){

  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);

      /*data = removeCommaSeparetedArrays(data,"genre");
      data = removeCommaSeparetedArrays(data,"country");*/

      margin.bottom = 60;
    
      width = (wwidth/3)-16 - margin.left - margin.right,
      height = wheight/2 - margin.top - margin.bottom-10;

      BoxAndWhiskers(data, "body-style", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "drive-wheels", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "engine-location", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "num-of-cylinders", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "engine-type", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "fuel-system", "insurance_risk_rating",width,height)

    });
}

if( title == "BoxAndWhiskers2_Third"){
    

  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);
      data = removeCommaSeparetedArrays(data,"genre");
      data = removeCommaSeparetedArrays(data,"country");
        
      margin.bottom = 60;
    
      width = (wwidth/3)-16 - margin.left - margin.right,
      height = wheight/2 - margin.top - margin.bottom-10;
  
    
      BoxAndWhiskers(data, "fuel-type", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "aspiration", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "num-of-doors", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "make", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "peak-rpm", "insurance_risk_rating",width,height)
      BoxAndWhiskers(data, "compression-ratio", "insurance_risk_rating",width,height)
    });
}

if( title == "Oscar_Third"){
    

  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);
    margin.bottom = 60;
  
    width = (wwidth/3)-16 - margin.left - margin.right,
    height = wheight/2 - margin.top - margin.bottom-10;

    BoxAndWhiskers(data,"engine-location","normalized-losees",width,height);
    BoxAndWhiskers(data,"num-of-cylinders","normalized-losees",width,height);
    BoxAndWhiskers(data,"body-style","normalized-losees",width,height);
    BoxAndWhiskers(data,"num-of-doors","normalized-losees",width,height);
    BoxAndWhiskers(data,"drive-wheels","normalized-losees",width,height);
    BoxAndWhiskers(data,"make","normalized-losees",width,height);

  });
}

if( title == "StackBarChart_Third"){


  fetch(url3)
  .then(res=>res.text())
  .then(rep =>{
    data = JSON.parse(rep.substr(47).slice(0,-2));
    data = parseJson2(data,200);

    var textHeight = 0;
    width = (wwidth/2)-16 - margin.left - margin.right;
    margin.bottom=70;
    height = (wheight-textHeight)/2 - margin.top - (margin.bottom+10);

    StackedBarPlot(data,"make","num-of-doors");
    StackedBarPlot(data,"make","num-of-cylinders");
    StackedBarPlot(data,"make","body-style");
    StackedBarPlot(data,"make","engine-location");
   
    
  });
}

function removeCommaSeparetedArrays(data,index){

  for (r in data) {

    if(typeof(data[r][index])=="string"){

          data[r][index] = data[r][index].split(",")[0];

        }

  }

  return data;
}

function setclickforcircles(){

  $("circle").click(function(){
     $(this).siblings().css( "display", "block" );
  });

  $(".clearButton").click(function(){
    $(this).parent().parent().find("circle").siblings().css("display","none");
 });

}

function cleanDataFromNullValues(data, keyX, keyY, keyR, keyC){

var i = data.length

while (i--) {

		if ( data[i][keyX] == -1 || data[i][keyY] == -1 || data[i][keyR] == -1  || data[i][keyC] == -1){
		 
		 data.splice(i, 1);
		
		}
	
	}
		
	return data;

}

function cleanDataFromNullValues2(data, keyX, keyY){

  var i = data.length
  
  while (i--) {
  
      if ( data[i][keyX] == -1 || data[i][keyY] == -1 ){
       
       data.splice(i, 1);
      
      }
    
    }
      
    return data;
  
}

function cleanDataFromNullValues3(data, keyX){

    var i = data.length
    
    while (i--) {
    
        if ( data[i][keyX] == -1 ){
         
         data.splice(i, 1);
        
        }
      
      }
        
      return data;
    
}

function BoxAndWhiskersData(index, key, data, remove = false) {

  var set = CreateSet(data, index);

  newData = {}

  for (i in set) {

    newData[set[i]] = []

  }

  for (r in data) {
    newData[data[r][index]].push(data[r][key]);

  }

  if(remove){

    for (i in set) {

      if(newData[set[i]].length<=1){

        delete newData[set[i]];

      }
  
    }

  }

  return newData

}

function BoxAndWhiskersDataOrdinal(index, key, data, remove = false) {

  var set = CreateSet(data, index);
  var set2 =  CreateSet(data, key);

  newData = {}

  for (i in set) {

    newData[set[i]] = []

  }

  for (r in data) {
    newData[data[r][index]].push(set2.indexOf(data[r][key]));
  }

  if(remove){

    for (i in set) {

      if(newData[set[i]].length<=1){

        delete newData[set[i]];

      }
  
    }

  }

  return newData

}

function objKeyToArr(obj, key) {

  var arr = [];

  for (i = 0; i < obj.length; i++) {

    arr.push(obj[i][key]);

  }

  return arr;
}

function parseJson(data,max=0) {

  var result = findNode('gs$cell', data);
  result = result[0][0]

  var row = findNode('row', result);
  var maxrow = max==0 ? parseInt(row[row.length - 1]): max;

  var col = findNode('col', result);
  var maxcol = parseInt(col[col.length - 1]);

	

  var array = [];

  var index = 0;

  for (i = 0; i < maxrow; i++) {

    var rowarray = []

    for (j = 0; j < maxcol; j++) {
      if (index < result.length) {
        var value = result[index]['inputValue'];
        value = isNumeric(value) ? parseFloat(value) : value;
        rowarray.push(value);
        index += 1;
      }
    }
    array.push(rowarray);
  }

  var result = [];

  for (i = 1; i < maxrow; i++) {

    var obj = {};

    for (j = 0; j < maxcol; j++) {

      obj[array[0][j]] = array[i][j];
    }

    result.push(obj);

  }

  for (j = 0; j < maxcol; j++) {

    let cardinalSet = new Set();

    if (typeof(array[1][j]) == "string") {

      //create set 
      for (r in result) {
        cardinalSet.add(result[r][array[0][j]]);
      }

      //transform set into list
      var cardinaArray = Array.from(cardinalSet);

      for (r in result) {


        result[r][array[0][j] + "_index"] = cardinaArray.indexOf(result[r][array[0][j]])

      }

    }

  }

  return result;


}

function parseJson2(data,max=0) {

  var result = findNode('table', data);

  var row = findNode('rows', result);
  var maxrow = max==0 ? row.length : max;

  var col = findNode('cols', result);
  var maxcol = col.length ;

	
  var array = [];

  for (i = 0; i < maxrow; i++) {

    var rowarray = []

    for (j = 0; j < maxcol; j++) {
      
      try {
        var value = row[i]['c'][j]['v'];
        value = isNumeric(value) ? parseFloat(value) : value;
        rowarray.push(value);
    
      } catch (error) {
        console.error(error);
      }

    }
    array.push(rowarray);
  }

  var result = [];

  for (i = 1; i < maxrow; i++) {

    var obj = {};

    for (j = 0; j < maxcol; j++) {

      obj[col[j]["label"]] = array[i][j];
    }

    result.push(obj);

  }

  for (j = 0; j < maxcol; j++) {

    let cardinalSet = new Set();

    if (typeof(array[1][j]) == "string") {

      //create set 
      for (r in result) {
        cardinalSet.add(result[r][col[j]["label"]]);
      }

      //transform set into list
      var cardinaArray = Array.from(cardinalSet);

      for (r in result) {


        result[r][col[j]["label"] + "_index"] = cardinaArray.indexOf(result[r][col[j]["label"]])

      }

    }

  }

  return result;


}

function getDataColumn(data, columNumber) {

  var row = data.length;
  var col = data[0].length;
  var array = [];

  for (i = 0; i < row; i++) {

    array.push(data[i][columNumber]);

  }
  return array;
}

function findNode(id, currentNode) {
  var i,
    currentChild;
  var result = [];

  for (key in currentNode) {

    if (key == id) {
      return currentNode[key];
    }

    currentChild = currentNode[key];

    if (typeof currentChild === 'object' && currentChild !== null) {

      // Search in the current child
      var tempresult = findNode(id, currentChild);

      // Return the result if the node has been found
      if (tempresult !== false) {
        result.push(tempresult);

      }
    }

  }
  if (result.length > 0) {
    return result;
  } else {
    return false;
  }
}

function isNumeric(str) {
  if (typeof str != "string") return false // we only process strings!  
  return !isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
    !isNaN(parseFloat(str)) // ...and ensure strings of whitespace fail
}

function ScatterPlot(data, keyX, keyY, keyR, keyC, label, sameScale = false) {

  keyX = typeof(data[0][keyX]) == "string" ? keyX + "_index" : keyX;
  keyY = typeof(data[0][keyY]) == "string" ? keyY + "_index" : keyY;
  keyR = typeof(data[0][keyR]) == "string" ? keyR + "_index" : keyR;
  keyC = typeof(data[0][keyC]) == "string" ? keyC + "_index" : keyC;
	
	data = cleanDataFromNullValues(data, keyX, keyY, keyR, keyC)
	
  //data extremes
  dataXmax = d3.max(objKeyToArr(data, keyX));
  dataXmin = d3.min(objKeyToArr(data, keyX));
  dataYmax = d3.max(objKeyToArr(data, keyY));
  dataYmin = d3.min(objKeyToArr(data, keyY));
  dataRmin = d3.min(objKeyToArr(data, keyR));
  dataRmax = d3.max(objKeyToArr(data, keyR));
  dataCmin = d3.min(objKeyToArr(data, keyC));
  dataCmax = d3.max(objKeyToArr(data, keyC));

	

  if (sameScale) {

    dataXmax = dataXmax > dataYmax ? dataXmax : dataYmax;
    dataYmax = dataXmax;

    dataXmin = dataXmin < dataYmin ? dataXmin : dataYmin;
    dataYmin = dataXmin;

  }

  //colorscale
  const colorScale = d3.scaleOrdinal()
    .range(d3.interpolateWarm);

  // set the dimensions and margins of the graph

  // append the svg object to the body of the page
  var svG = d3.select("#visual_area")
    .append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("class","ScatterPlot")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

  var spaceX = (dataXmax - dataXmin) / 15;
  // X scale and Axis
  var x = d3.scaleLinear()
    .domain([dataXmin - spaceX, dataXmax + spaceX]) // This is the min and the max of the data: 0 to 100 if percentages
    .range([0, width]); // This is the corresponding value I want in Pixel

  var axisX = d3.axisBottom(x);

  //add cardinal text values
  var TickTextX = keyX.includes("_index");
  if (TickTextX) {

    let arr = CreateSet(data, keyX.replace("_index", ""))

    axisX
      .ticks(arr.length)
      .tickFormat(function(d) {
        return arr[d];
      })


  }

  svG
    .append('g')
    .attr("transform", "translate(0," + height + ")")
    .call(axisX)
    .attr("class","xAxis")
    .selectAll("text")	
    .style("text-anchor", "end")
    .attr("dx", "-.8em")
    .attr("dy", ".15em")
    .attr("transform", "rotate(-65)");

  //ticks rotation in case is cardinal text values
  if (TickTextX) {

    svG
      .selectAll("text")
      .style("text-anchor", "end")
      .attr("dx", "-.8em")
      .attr("dy", "-.55em")
      .attr("transform", "rotate(-90)");

  }

  //Axes Label
  svG.append("text")
    .attr("class", "xlabel")
    .attr("text-anchor", "end")
    .attr("x", width)
    .attr("y", height - 6)
    .text(keyX.replaceAll('_', ' ').replaceAll('index', ''));

  // X scale and Axis
  var spaceY = (dataYmax - dataYmin) / 15;
  var y = d3.scaleLinear()
    .domain([dataYmin - spaceY, dataYmax + spaceY]) // This is the min and the max of the data: 0 to 100 if percentages
    .range([height, 0]); // This is the corresponding value I want in Pixel

  var axisY = d3.axisLeft(y)

  svG.append("text")
    .attr("class", "ylabel")
    .attr("text-anchor", "end")
    .attr("y", 6)
    .attr("dy", ".75em")
    .attr("transform", "rotate(-90)")
    .text(keyY.replaceAll('_', ' ').replaceAll('index', ''));

  //add cardinal text values
  var TickTextY = keyY.includes("_index");
  if (TickTextY) {

    let arr = CreateSet(data, keyY.replace("_index", ""))

    axisY
      .ticks(arr.length)
      .tickFormat(function(d) {
        return arr[d];
      })

  }

  svG
    .append('g')
    .call(axisY).attr("class","yAxis");

  svG
    .selectAll("whatever")
    .data(data)
    .enter()
    .call(function addPoints(d) {

      var g = d.append("g")
        .attr("class", "hidden")
        .attr("id", function(d) {
          return d[label].replaceAll(' ', '_').replaceAll(':', '').replaceAll('.', '').replaceAll('\'', '').replaceAll('!', '');
        })

      g.append("circle")
        .attr("cx", function(d) {
          return x(d[keyX])
        })
        .attr("cy", function(d) {
          return y(d[keyY])
        })
        .attr("r", function(d) {
          var value = map_range(d[keyR], dataRmin, dataRmax, 2, 10);
          return value;
        })
        .attr("fill", function(d) {

          var value = map_range(d[keyC], dataCmin, dataCmax, 1, 0);
          value = d3.interpolateWarm(value);

          return value;
        }).attr("data",function(d){
          return d[label]
        });

      g.append('text')
        .attr("dx", function(d) {
          return x(d[keyX])
        })
        .attr("dy", function(d) {
          return y(d[keyY])
        })
        .text(function(d) {
          return d[label]
        })
        .attr("class", "label")

    });

	var correlationValue = calculateCorrelation(data,keyX,keyY);
  
	//Axes Label
  svG.append("text")
    .attr("class", "xlabel")
    .attr("text-anchor", "end")
    .attr("x", width)
    .attr("y", height + margin.bottom)
    .text("correlation: "+correlationValue);
	
  svG.append("text")
    .attr("class", "clearButton")
    .attr("text-anchor", "end")
    .attr("x", margin.left)
    .attr("y", height + margin.bottom)
    .text("clear labels");
	

	if (sameScale) {
    svG.append("line")
      .attr("x1", x(dataXmin - spaceX))
      .attr("y1", y(dataYmin - spaceY))
      .attr("x2", x(dataXmax + spaceX))
      .attr("y2", y(dataYmax + spaceY))
      .attr("class", "diagonal");
  }
	/*else{
	
		calculateCorrelation(data,keyX,keyY);
		var linedata = calculateRegressionLine(data, keyX, keyY);
		var line = d3.line()
					.x(function(d) {
							return x(d.x);
					})
					.y(function(d) {
							return y(d.yhat);
					});
		svG.append("line")
        .datum(linedata)
        .attr("class", "line")
        .attr("d", line);
					
	}*/
}

function calculateCorrelation(data, keyX, keyY){

 		x = data.map(d => d[keyX]);
			
		y = data.map(d => d[keyY]);	
				  
		Sxx = d3.sum(x.map(d => Math.pow(d-d3.mean(x), 2)));
			
		Sxy = d3.sum(x.map( (d, i) => (x[i]-d3.mean(x))*(y[i]-d3.mean(y))));
			
		Syy = d3.sum(y.map(d => Math.pow(d-d3.mean(y), 2)));
			
		corrcoef = Sxy/(Math.sqrt(Sxx)*Math.sqrt(Syy));
		
		return corrcoef;

}

function calculateRegressionLine(data, keyX, keyY){

	var x = data.map(d => d[keyX]);		
	var y = data.map(d => d[keyY]);	
	var n = x.length;
	var x_mean = 0;
	var y_mean = 0;
	var term1 = 0;
	var term2 = 0;
	var noise = 0;
	
	// create x and y values
	for (var i = 0; i < n; i++) {
		x_mean += x[i]
		y_mean += y[i]
	}
	// calculate mean x and y
	x_mean /= n;
	y_mean /= n;
	
	// calculate coefficients
	var xr = 0;
	var yr = 0;
	for (i = 0; i < x.length; i++) {
		xr = x[i] - x_mean;
		yr = y[i] - y_mean;
		term1 += xr * yr;
		term2 += xr * xr;
	
	}
	var b1 = term1 / term2;
	var b0 = y_mean - (b1 * x_mean);
	// perform regression 
	
	yhat = [];
	// fit line using coeffs
	for (i = 0; i < x.length; i++) {
		yhat.push(b0 + (x[i] * b1));
	}
	
	var linedata = [];
	for (i = 0; i < y.length; i++) {
		linedata.push({
			"yhat": yhat[i],
			"y": y[i],
			"x": x[i]
		})
	}
	
	linedata.forEach(function(d) {
        d.x = +d.x;
        d.y = +d.y;
        d.yhat = +d.yhat;
    });
	
return linedata;    

}

function map_range(value, low1, high1, low2, high2) {

  var result = low2 + ((high2 - low2) * (value - low1) / (high1 - low1));

  return result;
}

function CreateSet(data, key) {

  let cardinalSet = new Set();

  //create set 
  for (r in data) {
    cardinalSet.add(data[r][key]);
  }

  //transform set into list
  var cardinaArray = Array.from(cardinalSet);

  for (i in cardinaArray){
    
    element=cardinaArray[i];

    if(element==undefined){
      cardinaArray.splice(i,1);
    }

  }

  return cardinaArray;

}

function BoxAndWhiskers(data, key1, key2,width,height) {

  data = cleanDataFromNullValues2(data, key1, key2);

  var groupCounts = BoxAndWhiskersData(key1, key2, data, true);

//console.log(groupCounts);

  var totalWidth = width + margin.left + margin.right;
  var totalheight = height + margin.top + margin.bottom;

  //var barWidth = (totalWidth/Object.keys(groupCounts).length)-80;

  var barWidth = 10;
  var boxPlotColor = "#898989";
  var medianLineColor = "#ffffff";
  var axisColor = "#898989";

  // Setup the svg and group we will draw the box plot in
  var svg = d3.select("#visual_area").append("svg")
    .attr("width", totalWidth)
    .attr("height", totalheight)
    .append("g")
    .attr("class","BoxAndWhiskers")
    .attr("transform", "translate(" + (margin.left - barWidth) + "," + margin.top + ")");

  // Move axis to center align the bars with the xAxis ticks
  var yAxisBox = svg.append("g").attr("transform", "translate(0,0)");
  var xAxisBox = svg.append("g").attr("transform", "translate(0,0)");

  // Select all values into one Array for axis scaling (min/ max)
  // and sort group counts so quantile methods work
  var globalCounts = [];
  for (var key in groupCounts) {
    var groupCount = groupCounts[key]
    groupCounts[key] = groupCount.sort(sortNumber);
    groupCounts[key].forEach(element => {
      globalCounts.push(element);
    });
  }

  // Prepare the data for the box plots
  var plotData = [];
  var colorIndex = 0.1;
  var colorIndexStepSize = 0.08;
  for (var [key, groupCount] of Object.entries(groupCounts)) {
    var record = {};
    var localMin = d3.min(groupCount);
    var localMax = d3.max(groupCount);

    record["key"] = key;
    record["counts"] = groupCount;
    record["quartile"] = boxQuartiles(groupCount);
    record["whiskers"] = [localMax, localMin];
    record["color"] = d3.interpolateRainbow(colorIndex);

    plotData.push(record);
    colorIndex += colorIndexStepSize;
  }

  // Create Tooltips
  var tip = d3.tip().attr('class', 'd3-tip').direction('e').offset([0, 5])
    .html(function(d) {
      var content = "<span style='margin-left: 2.5px;'><b>" + d.key + "</b></span><br>";
      content += `
	<table style="margin-top: 2.5px;">
	<tr><td>Max: </td><td style="text-align: right">` + d3.format(".2f")(d.whiskers[0]) + `</td></tr>
	<tr><td>Q3: </td><td style="text-align: right">` + d3.format(".2f")(d.quartile[0]) + `</td></tr>
	<tr><td>Median: </td><td style="text-align: right">` + d3.format(".2f")(d.quartile[1]) + `</td></tr>
	<tr><td>Q1: </td><td style="text-align: right">` + d3.format(".2f")(d.quartile[2]) + `</td></tr>
	<tr><td>Min: </td><td style="text-align: right">` + d3.format(".2f")(d.whiskers[1]) + `</td></tr>
	</table>
	`;
      return content;
    });
  svg.call(tip);

  // Compute an ordinal xScale for the keys in plotData
  var xScale = d3.scalePoint()
    .domain(Object.keys(groupCounts))
    .rangeRound([0, width])
    .padding([0.5]);

  // Compute a global y scale based on the global counts
  var min = d3.min(globalCounts);
  var max = d3.max(globalCounts);
  var yScale = d3.scaleLinear()
    .range([height, 0])
    .domain([min, max])
    .nice();

  // Setup the group the box plot elements will render in
  var g = svg.append("g").attr("id","content")
    .attr("transform", "translate(" + (-barWidth / 2) + ",0)");

  // Draw the box plot vertical lines
  var verticalLines = g.selectAll(".verticalLines")
    .data(plotData)
    .enter()
    .append("line")
    .attr("x1", d => {
      return xScale(d.key) + barWidth / 2;
    })
    .attr("y1", d => {
      return yScale(d.whiskers[0]);
    })
    .attr("x2", d => {
      return xScale(d.key) + barWidth / 2;
    })
    .attr("y2", d => {
      return yScale(d.whiskers[1]);
    })
    .attr("stroke", boxPlotColor)
    .attr("stroke-width", 1)
    .attr("stroke-dasharray", "5,5")
    .attr("fill", "none");

  // Draw the boxes of the box plot, filled in white and on top of vertical lines
  var rects = g.selectAll("rect")
    .data(plotData)
    .enter()
    .append("rect")
    .attr("width", barWidth)
    .attr("height", d => {
      return yScale(d.quartile[2]) - yScale(d.quartile[0]);
    })
    .attr("x", d => {
      return xScale(d.key);
    })
    .attr("y", d => {
      return yScale(d.quartile[0]);
    })
    .attr("fill", d => {
      return d.color;
    })
    .attr("stroke", boxPlotColor)
    .attr("stroke-width", 1)
    .on('mouseover', tip.show)
    .on('mouseout', tip.hide);

  // Config for whiskers and median
  var horizontalLineConfigs = [{ // Top whisker
      x1: d => {
        return xScale(d.key)
      },
      y1: d => {
        return yScale(d.whiskers[0])
      },
      x2: d => {
        return xScale(d.key) + barWidth
      },
      y2: d => {
        return yScale(d.whiskers[0])
      },
      color: boxPlotColor
    },
    { // Median
      x1: d => {
        return xScale(d.key)
      },
      y1: d => {
        return yScale(d.quartile[1])
      },
      x2: d => {
        return xScale(d.key) + barWidth
      },
      y2: d => {
        return yScale(d.quartile[1])
      },
      color: medianLineColor
    },
    { // Bottom whisker
      x1: d => {
        return xScale(d.key)
      },
      y1: d => {
        return yScale(d.whiskers[1])
      },
      x2: d => {
        return xScale(d.key) + barWidth
      },
      y2: d => {
        return yScale(d.whiskers[1])
      },
      color: boxPlotColor
    }
  ];

  // Draw the whiskers and median line
  for (var i = 0; i < horizontalLineConfigs.length; i++) {
    var lineConfig = horizontalLineConfigs[i];
    var horizontalLine = g.selectAll(".whiskers")
      .data(plotData)
      .enter()
      .append("line")
      .attr("x1", lineConfig.x1)
      .attr("y1", lineConfig.y1)
      .attr("x2", lineConfig.x2)
      .attr("y2", lineConfig.y2)
      .attr("stroke", lineConfig.color)
      .attr("stroke-width", 1)
      .attr("fill", "none");
  }

  // add the Y gridlines
  svg.append("g")
    .attr("transform", "translate(0,0)")
    .attr("class", "grid")
    .call(d3.axisLeft(yScale)
      .tickSize(-width)
      .tickFormat("")
    )

  // Setup a series axis on the bottom
  var xAxis = d3.axisBottom(xScale);
  xAxisBox.append("g")
    .attr("class", "xAxis")
    .attr("transform", "translate(0," + height + ")")
    .call(xAxis);

  svg.selectAll("text")
    .style("text-anchor", "end")
    .attr("dx", "-.8em")
    .attr("dy", "-.55em")
    .attr("transform", "rotate(-90)");

  //Axes Label
  svg.append("text")
    .attr("class", "xlabel")
    .attr("text-anchor", "end")
    .attr("x", width)
    .attr("y", height - 6)
    .text(key1.replaceAll('_', ' ').replaceAll('index', ''));

  // Setup a scale on the left
  var yAxis = d3.axisLeft(yScale);

  yAxisBox.append("g")
    .attr("class", "yAxis")
    .call(yAxis);

  svg.append("text")
    .attr("class", "ylabel")
    .attr("text-anchor", "end")
    .attr("y", 6)
    .attr("dy", ".75em")
    .attr("transform", "rotate(-90)")
    .text(key2.replaceAll('_', ' ').replaceAll('index', ''));
}

function BoxAndWhiskersOrdinal(data, key1, key2,width,height) {

  data = cleanDataFromNullValues2(data, key1, key2);

  var groupCounts = BoxAndWhiskersDataOrdinal(key1, key2, data, true);

//console.log(groupCounts);

  var totalWidth = width + margin.left + margin.right;
  var totalheight = height + margin.top + margin.bottom;

  //var barWidth = (totalWidth/Object.keys(groupCounts).length)-80;

  var barWidth = 10;
  var boxPlotColor = "#898989";
  var medianLineColor = "#ffffff";
  var axisColor = "#898989";

  // Setup the svg and group we will draw the box plot in
  var svg = d3.select("#visual_area").append("svg")
    .attr("width", totalWidth)
    .attr("height", totalheight)
    .append("g")
    .attr("class","BoxAndWhiskers")
    .attr("transform", "translate(" + (margin.left - barWidth) + "," + margin.top + ")");

  // Move axis to center align the bars with the xAxis ticks
  var yAxisBox = svg.append("g").attr("transform", "translate(0,0)");
  var xAxisBox = svg.append("g").attr("transform", "translate(0,0)");

  // Select all values into one Array for axis scaling (min/ max)
  // and sort group counts so quantile methods work
  var globalCounts = [];
  for (var key in groupCounts) {
    var groupCount = groupCounts[key]
    groupCounts[key] = groupCount.sort(sortNumber);
    groupCounts[key].forEach(element => {
      globalCounts.push(element);
    });
  }

  // Prepare the data for the box plots
  var plotData = [];
  var colorIndex = 0.1;
  var colorIndexStepSize = 0.08;
  for (var [key, groupCount] of Object.entries(groupCounts)) {
    var record = {};
    var localMin = d3.min(groupCount);
    var localMax = d3.max(groupCount);

    record["key"] = key;
    record["counts"] = groupCount;
    record["quartile"] = boxQuartiles(groupCount);
    record["whiskers"] = [localMax, localMin];
    record["color"] = d3.interpolateWarm(colorIndex);

    plotData.push(record);
    colorIndex += colorIndexStepSize;
  }

  // Create Tooltips
  var tip = d3.tip().attr('class', 'd3-tip').direction('e').offset([0, 5])
    .html(function(d) {
      var content = "<span style='margin-left: 2.5px;'><b>" + d.key + "</b></span><br>";
      content += `
	<table style="margin-top: 2.5px;">
	<tr><td>Max: </td><td style="text-align: right">` + d3.format(".2f")(d.whiskers[0]) + `</td></tr>
	<tr><td>Q3: </td><td style="text-align: right">` + d3.format(".2f")(d.quartile[0]) + `</td></tr>
	<tr><td>Median: </td><td style="text-align: right">` + d3.format(".2f")(d.quartile[1]) + `</td></tr>
	<tr><td>Q1: </td><td style="text-align: right">` + d3.format(".2f")(d.quartile[2]) + `</td></tr>
	<tr><td>Min: </td><td style="text-align: right">` + d3.format(".2f")(d.whiskers[1]) + `</td></tr>
	</table>
	`;
      return content;
    });
  svg.call(tip);

  // Compute an ordinal xScale for the keys in plotData
  var xScale = d3.scalePoint()
    .domain(Object.keys(groupCounts))
    .rangeRound([0, width])
    .padding([0.5])
    .attr("class", "xAxis");

  // Compute a global y scale based on the global counts
  var min = d3.min(globalCounts);
  var max = d3.max(globalCounts);
  var yScale = d3.scaleLinear()
    .range([height, 0])
    .domain([min, max])
    .nice()
    .attr("class", "yAxis");

  // Setup the group the box plot elements will render in
  var g = svg.append("g").attr("id","content")
    .attr("transform", "translate(" + (-barWidth / 2) + ",0)");

  // Draw the box plot vertical lines
  var verticalLines = g.selectAll(".verticalLines")
    .data(plotData)
    .enter()
    .append("line")
    .attr("x1", d => {
      return xScale(d.key) + barWidth / 2;
    })
    .attr("y1", d => {
      return yScale(d.whiskers[0]);
    })
    .attr("x2", d => {
      return xScale(d.key) + barWidth / 2;
    })
    .attr("y2", d => {
      return yScale(d.whiskers[1]);
    })
    .attr("stroke", boxPlotColor)
    .attr("stroke-width", 1)
    .attr("stroke-dasharray", "5,5")
    .attr("fill", "none");

  // Draw the boxes of the box plot, filled in white and on top of vertical lines
  var rects = g.selectAll("rect")
    .data(plotData)
    .enter()
    .append("rect")
    .attr("width", barWidth)
    .attr("height", d => {
      return yScale(d.quartile[2]) - yScale(d.quartile[0]);
    })
    .attr("x", d => {
      return xScale(d.key);
    })
    .attr("y", d => {
      return yScale(d.quartile[0]);
    })
    .attr("fill", d => {
      return d.color;
    })
    .attr("stroke", boxPlotColor)
    .attr("stroke-width", 1)
    .on('mouseover', tip.show)
    .on('mouseout', tip.hide);

  // Config for whiskers and median
  var horizontalLineConfigs = [{ // Top whisker
      x1: d => {
        return xScale(d.key)
      },
      y1: d => {
        return yScale(d.whiskers[0])
      },
      x2: d => {
        return xScale(d.key) + barWidth
      },
      y2: d => {
        return yScale(d.whiskers[0])
      },
      color: boxPlotColor
    },
    { // Median
      x1: d => {
        return xScale(d.key)
      },
      y1: d => {
        return yScale(d.quartile[1])
      },
      x2: d => {
        return xScale(d.key) + barWidth
      },
      y2: d => {
        return yScale(d.quartile[1])
      },
      color: medianLineColor
    },
    { // Bottom whisker
      x1: d => {
        return xScale(d.key)
      },
      y1: d => {
        return yScale(d.whiskers[1])
      },
      x2: d => {
        return xScale(d.key) + barWidth
      },
      y2: d => {
        return yScale(d.whiskers[1])
      },
      color: boxPlotColor
    }
  ];

  // Draw the whiskers and median line
  for (var i = 0; i < horizontalLineConfigs.length; i++) {
    var lineConfig = horizontalLineConfigs[i];
    var horizontalLine = g.selectAll(".whiskers")
      .data(plotData)
      .enter()
      .append("line")
      .attr("x1", lineConfig.x1)
      .attr("y1", lineConfig.y1)
      .attr("x2", lineConfig.x2)
      .attr("y2", lineConfig.y2)
      .attr("stroke", lineConfig.color)
      .attr("stroke-width", 1)
      .attr("fill", "none");
  }

  // add the Y gridlines
  svg.append("g")
    .attr("transform", "translate(0,0)")
    .attr("class", "grid")
    .call(d3.axisLeft(yScale)
      .tickSize(-width)
      .tickFormat("")
    )

  // Setup a series axis on the bottom
  var xAxis = d3.axisBottom(xScale);
  xAxisBox.append("g")
    .attr("class", "xAxis")
    .attr("transform", "translate(0," + height + ")")
    .call(xAxis);

  svg.selectAll("text")
    .style("text-anchor", "end")
    .attr("dx", "-.8em")
    .attr("dy", "-.55em")
    .attr("transform", "rotate(-90)");

  //Axes Label
  svg.append("text")
    .attr("class", "xlabel")
    .attr("text-anchor", "end")
    .attr("x", width)
    .attr("y", height - 6)
    .text(key1.replaceAll('_', ' ').replaceAll('index', ''));

  // Setup a scale on the left
  var yAxis = d3.axisLeft(yScale);

  yAxisBox.append("g")
    .attr("class", "yAxis")
    .call(yAxis);

  svg.append("text")
    .attr("class", "ylabel")
    .attr("text-anchor", "end")
    .attr("y", 6)
    .attr("dy", ".75em")
    .attr("transform", "rotate(-90)")
    .text(key2.replaceAll('_', ' ').replaceAll('index', ''));
}

function boxQuartiles(d) {
  return [
    d3.quantile(d, .75),
    d3.quantile(d, .5),
    d3.quantile(d, .25),
  ];
}

function sortNumber(a, b) {
  return a - b;
}

function Histogram(data, key) {

	key = typeof(data[0][key]) == "string" ? key + "_index" : key;
	var TickText = typeof(data[0][key]) == "string";
	
  data = cleanDataFromNullValues3(data,key);
		
  //data extremes
  dataXmax = TickText ? d3.max(objKeyToArr(data, key)): d3.max(objKeyToArr(data, key))+1;
  dataXmin = d3.min(objKeyToArr(data, key));
  newData = getDataColumn(data, key);

		
  // set the ranges
  var x = d3.scaleLinear()
    .domain([dataXmin, dataXmax])
    .rangeRound([0, width]);
  var y = d3.scaleLinear()
    .range([height, 0]);

	//add cardinal text values
  var TickText = key.includes("_index");
  if (TickText) {  
	
		arr = CreateSet(data, key.replace("_index", ""));

    axisX = d3.axisBottom(x)
		.ticks(arr.length)
    .tickFormat(function(d) {return arr[d];});
		
		var ticks = [];
		var count = 0;
		
		arr.forEach(function(part, index) {
  		ticks[index] = index;
		}, ticks); // use arr as this
		
	}else{
	
	 axisX = d3.axisBottom(x); 
	 var ticks = x.ticks();

	}
	
	
  // set the parameters for the histogram
  var histogram = d3.histogram()
    .domain(x.domain())
    .thresholds(ticks);

  var svg = d3.select("#visual_area").append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("class","Histogram")
    .attr("transform","translate(" + margin.left + "," + margin.top + ")");

  var bins = histogram(newData);

	console.log(bins);

  // Scale the range of the data in the y domain
   y.domain([0, d3.max(bins, function(d) { return d.length; })]);

  // append the bar rectangles to the svg element
  svg.selectAll("rect")
      .data(bins)
      .enter().append("rect")
      .attr("class", "bar")
      .attr("x", 1)
      .attr("transform", function(d) {
		  return "translate(" + x(d.x0) + "," + y(d.length) + ")"; })
      .attr("width", function(d) { return x(d.x1) - x(d.x0) -1 ; })
      .attr("height", function(d) { return height - y(d.length); });
			
  // add the x Axis
  svg.append("g")
      .attr("transform", "translate(0," + height + ")")
      .call(axisX)
			.attr("class","xAxis");

			
svg.selectAll("text")
    .style("text-anchor", "end")
    .attr("dx", "-.8em")
    .attr("dy", "-.55em")
    .attr("transform", "rotate(-90)");
		
  // add the y Axis
  svg.append("g")
      .call(d3.axisLeft(y))
				.attr("class","yAxis");

  //Axes Label
  svg.append("text")
    .attr("class", "xlabel")
    .attr("text-anchor", "end")
    .attr("x", width)
    .attr("y", height - 6)
    .text(key.replaceAll('_', ' ').replaceAll('index', ''));

   svg.append("text")
    .attr("class", "ylabel")
    .attr("text-anchor", "end")
    .attr("y", 6)
    .attr("dy", ".75em")
    .attr("transform", "rotate(-90)")
    .text('number of movies');

}

function HistogramTreshold(data, key) {

	key = typeof(data[0][key]) == "string" ? key + "_index" : key;
	var TickText = typeof(data[0][key]) == "string";
		
  //data extremes
  dataXmax = TickText ? d3.max(objKeyToArr(data, key)): d3.max(objKeyToArr(data, key))+1;
  dataXmin = d3.min(objKeyToArr(data, key));
  newData = getDataColumn(data, key);

		
  // set the ranges
  var x = d3.scaleLinear()
    .domain([dataXmin, dataXmax])
    .rangeRound([0, width]);
  var y = d3.scaleLinear()
    .range([height, 0]);

	//add cardinal text values
  var TickText = key.includes("_index");
  if (TickText) {  
	
		arr = CreateSet(data, key.replace("_index", ""));

    axisX = d3.axisBottom(x)
		.ticks(arr.length)
    .tickFormat(function(d) {return arr[d];});
		
		var ticks = [];
		var count = 0;
		
		arr.forEach(function(part, index) {
  		ticks[index] = index;
		}, ticks); // use arr as this
		
	}else{
	
	 axisX = d3.axisBottom(x); 
	 var ticks = x.ticks();

	}
	
	
  // set the parameters for the histogram
  var histogram = d3.histogram()
    .domain(x.domain())
    .thresholds(ticks);

  var svg = d3.select("#visual_area").append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("class","Histogram")
    .attr("transform","translate(" + margin.left + "," + margin.top + ")");

  var bins = histogram(newData);

	console.log(bins);

  // Scale the range of the data in the y domain
   y.domain([0, d3.max(bins, function(d) { return d.length; })]);

  // append the bar rectangles to the svg element
  svg.selectAll("rect")
      .data(bins)
      .enter().append("rect")
      .attr("class", "bar")
      .attr("x", 1)
      .attr("transform", function(d) {
		  return "translate(" + x(d.x0) + "," + y(d.length) + ")"; })
      .attr("width", function(d) { return x(d.x1) - x(d.x0) -1 ; })
      .attr("height", function(d) { return height - y(d.length); });
			
  // add the x Axis
  svg.append("g")
      .attr("transform", "translate(0," + height + ")")
      .call(axisX)
			.attr("class","xAxis");

			
svg.selectAll("text")
    .style("text-anchor", "end")
    .attr("dx", "-.8em")
    .attr("dy", "-.55em")
    .attr("transform", "rotate(-90)");
		
  // add the y Axis
  svg.append("g")
      .call(d3.axisLeft(y))
				.attr("class","yAxis");

  //Axes Label
  svg.append("text")
    .attr("class", "xlabel")
    .attr("text-anchor", "end")
    .attr("x", width)
    .attr("y", height - 6)
    .text(key.replaceAll('_', ' ').replaceAll('index', ''));

   svg.append("text")
    .attr("class", "ylabel")
    .attr("text-anchor", "end")
    .attr("y", 6)
    .attr("dy", ".75em")
    .attr("transform", "rotate(-90)")
    .text('number of movies');

}

function legend({
  color,
  title,
	title2 = "",
  tickSize = 6,
  width = 320,
  height = 44 + tickSize,
  marginTop = 18,
  marginRight = 0,
  marginBottom = 16 + tickSize,
  marginLeft = 100,
  ticks = width / 64,
  tickFormat,
  tickValues
} = {}) 
{
  const svg = d3.select("svg")
	  .attr("margin-left", marginLeft)
    .attr("width", width)
    .attr("height", height)
    .attr("viewBox", [0, 0, width, height])
    .style("overflow", "visible")
    .style("display", "block");

  let tickAdjust = g => g.selectAll(".tick line").attr("y1", marginTop + marginBottom - height);
  let x;

  // Continuous
  if (color.interpolate) {
    const n = Math.min(color.domain().length, color.range().length);

    x = color.copy().rangeRound(d3.quantize(d3.interpolate(marginLeft, width - marginRight), n));

    svg.append("image")
      .attr("x", marginLeft)
      .attr("y", marginTop)
      .attr("width", width - marginLeft - marginRight)
      .attr("height", height - marginTop - marginBottom)
      .attr("preserveAspectRatio", "none")
      .attr("xlink:href", ramp(color.copy().domain(d3.quantize(d3.interpolate(0, 1), n))).toDataURL());
  }

  // Sequential
  else if (color.interpolator) {
    x = Object.assign(color.copy()
      .interpolator(d3.interpolateRound(marginLeft, width - marginRight)), {
        range() {
          return [marginLeft, width - marginRight];
        }
      });

    svg.append("image")
      .attr("x", marginLeft)
      .attr("y", marginTop)
      .attr("width", width - marginLeft - marginRight)
      .attr("height", height - marginTop - marginBottom)
      .attr("preserveAspectRatio", "none")
      .attr("xlink:href", ramp(color.interpolator()).toDataURL());

    // scaleSequentialQuantile doesn’t implement ticks or tickFormat.
    if (!x.ticks) {
      if (tickValues === undefined) {
        const n = Math.round(ticks + 1);
        tickValues = d3.range(n).map(i => d3.quantile(color.domain(), i / (n - 1)));
      }
      if (typeof tickFormat !== "function") {
        tickFormat = d3.format(tickFormat === undefined ? ",f" : tickFormat);
      }
    }
  }

  // Threshold
  else if (color.invertExtent) {
    const thresholds = color.thresholds ? color.thresholds() // scaleQuantize
      :
      color.quantiles ? color.quantiles() // scaleQuantile
      :
      color.domain(); // scaleThreshold

    const thresholdFormat = tickFormat === undefined ? d => d :
      typeof tickFormat === "string" ? d3.format(tickFormat) :
      tickFormat;

    x = d3.scaleLinear()
      .domain([-1, color.range().length - 1])
      .rangeRound([marginLeft, width - marginRight]);

    svg.append("g")
      .selectAll("rect")
      .data(color.range())
      .join("rect")
      .attr("x", (d, i) => x(i - 1))
      .attr("y", marginTop)
      .attr("width", (d, i) => x(i) - x(i - 1))
      .attr("height", height - marginTop - marginBottom)
      .attr("fill", d => d);

    tickValues = d3.range(thresholds.length);
    tickFormat = i => thresholdFormat(thresholds[i], i);
  }

  // Ordinal
  else {
    x = d3.scaleBand()
      .domain(color.domain())
      .rangeRound([marginLeft, width - marginRight]);

    svg.append("g")
      .selectAll("rect")
      .data(color.domain())
      .join("rect")
      .attr("x", x)
      .attr("y", marginTop)
      .attr("width", Math.max(0, x.bandwidth() - 1))
      .attr("height", height - marginTop - marginBottom)
      .attr("fill", color);

    tickAdjust = () => {};
  }

  svg.append("g")
    .attr("transform", `translate(0,${height - marginBottom})`)
    .call(d3.axisBottom(x)
      .ticks(ticks, typeof tickFormat === "string" ? tickFormat : undefined)
      .tickFormat(typeof tickFormat === "function" ? tickFormat : undefined)
      .tickSize(tickSize)
      .tickValues(tickValues))
    .call(tickAdjust)
    .call(g => g.select(".domain").remove())
    .call(g => g.append("text")
      .attr("x", marginLeft)
      .attr("y", marginTop + marginBottom - height - 6)
      .attr("fill", "currentColor")
      .attr("text-anchor", "start")
      .attr("font-weight", "bold")
      .text(title))
		.call(g => g.append("text")
      .attr("x", width +20)
      .attr("y", marginTop + marginBottom - height - 6)
      .attr("fill", "currentColor")
      .attr("text-anchor", "start")
      .attr("font-weight", "bold")
      .text(title2));

 x = d3.scaleBand()
      .domain(color.domain())
      .rangeRound([marginLeft, width - marginRight]);

	 svg.append("circle")
        .attr("cx", width + 30 )
        .attr("cy", height/2 )
        .attr("r", 10)
        .attr("fill", "grey")

	 svg.append("circle")
        .attr("cx", width + 50 )
        .attr("cy", height/2 )
        .attr("r", 2)
        .attr("fill", "grey")

  return svg.node();
}

function ramp(color, n = 256) {
  var canvas = document.createElement('canvas');
  canvas.width = n;
  canvas.height = 1;
  const context = canvas.getContext("2d");
  for (let i = 0; i < n; ++i) {
    context.fillStyle = color(i / (n - 1));
    context.fillRect(i, 0, 1, 1);
  }
  return canvas;
}

function dataForStackedBarPlot(data, keyX, keyY){

  var keyset = CreateSet(data,keyX);
  var subkeyset = CreateSet(data,keyY);
  var newdata =[];

  //columns
  var columns = [...subkeyset];
  columns.unshift('group');
  newdata['columns'] = columns

  for (i in keyset){

    key  = keyset[i];

    var groupElement ={group:key};

    for (j in subkeyset){

      subkey = subkeyset[j];
      groupElement[subkey] =0;
          
      for (r in data) { 
        if(data[r][keyY] == subkey & data[r][keyX] == key){
          groupElement[subkey]+=1;
        }
      }
    }
    newdata.push(groupElement);
  }

  return newdata;

}

function findMinMaxinStackedBarPlotData(data){

  var max;

  for (i in data){

    if(i!='columns'){

        entry = data[i];
        
        localmax =0; 
        
        for(j in entry){

          if(j!='group'){

          localmax += entry[j];
          }
        }

        max = max>localmax? max:localmax;
    }
  }

  return max;
}

function StackedBarPlot(data, keyX, keyY){

  data = removeCommaSeparetedArrays(data,keyX);
  data = cleanDataFromNullValues2(data, keyX, keyY);
  data = dataForStackedBarPlot(data, keyX, keyY);

  internalwidth = width - margin.right*7;

  var dataYmax  = findMinMaxinStackedBarPlotData(data);

  const svg = d3.select("#visual_area")
  .append("svg")
    .attr("width", internalwidth + margin.left + margin.right*8)
    .attr("height", height + margin.top + margin.bottom  )
    .append("g")
   .attr("class","StackedBarPlot")
    .attr("transform", `translate(${margin.left},${margin.top})`);

  // List of subgroups = header of the csv files = soil condition here
  const subgroups = data.columns.slice(1)

  // List of groups = species here = value of the first column called group -> I show them on the X axis
  const groups = data.map(d => (d.group))

  //  X axis
  const x = d3.scaleBand()
      .domain(groups)
      .range([0, internalwidth])
      .padding([0.2]);

 

  // Y axis
  const y = d3.scaleLinear()
    .domain([0, dataYmax])
    .range([ height, 0 ]);
  

  const color  = d3.scaleSequential().domain([0,subgroups.length])
    .interpolator(d3.interpolateSinebow);
 
  const  z = d3.scaleSequential().domain([0,subgroups.length])
    .interpolator(d3.interpolateSinebow);


  //stack the data? --> stack per subgroup
  const stackedData = d3.stack()
    .keys(subgroups)
    (data)

  // Show the bars
  svg.append("g")
    .selectAll("g")
    // Enter in the stack data = loop key per key = group per group
    .data(stackedData)
    .join("g")
      .attr("fill", function(d,i){
        return color(i);
    })
      .selectAll("rect")
      // enter a second time = loop subgroup per subgroup to add all rectangles
      .data(d => d)
      .join("rect")
        .attr("x", d => x(d.data.group))
        .attr("y", d => y(d[1]))
        .attr("height", d => y(d[0]) - y(d[1]))
        .attr("width",x.bandwidth())

  

  var legend = svg.append('g')
    .attr('class', 'legend')
    .attr('transform', 'translate('+(internalwidth+10)+',0)');

    // Add X axis
    svg.append("g")
    .attr("transform", `translate(0, ${height})`)
    .call(d3.axisBottom(x).tickSizeOuter(0))
    .attr("class", "xAxis")
    .selectAll("text")	
        .style("text-anchor", "end")
        .attr("dx", "-.8em")
        .attr("dy", ".15em")
        .attr("transform", "rotate(-65)");
        

    svg.append("text")
        .attr("class", "xlabel")
        .attr("text-anchor", "end")
        .attr("x", internalwidth)
        .attr("y", height - 6)
        .text(keyX.replaceAll('_', ' ').replaceAll('index', ''));
  
        // Add Y axis
  svg.append("g")
  .call(d3.axisLeft(y))
  .attr("class", "yAxis");

  svg.append("text")
    .attr("class", "ylabel")
    .attr("text-anchor", "end")
    .attr("y", 6)
    .attr("dy", ".75em")
    .attr("transform", "rotate(-90)")
    .text(keyY.replaceAll('_', ' ').replaceAll('index', ''));

    //legend
legend.selectAll('rect')
    .data(subgroups)
    .enter()
    .append('rect')
    .attr('x', 0)
    .attr('y', function(d,i){
        return i * 18;
    })
    .attr('width', 12)
    .attr('height', 12)
    .attr('fill', function(d,i){
        return z(i);
    });

legend.selectAll('text')
    .data(subgroups)
    .enter()
    .append('text')
    .text(function(d){
        return d;
    })
    .attr('x', 15)
    .attr('y', function(d, i){
        return i * 18;
    })
    .attr('text-anchor', 'start')
    .attr('alignment-baseline', 'hanging');

}