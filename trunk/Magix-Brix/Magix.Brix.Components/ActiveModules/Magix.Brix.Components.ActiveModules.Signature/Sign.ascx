<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Signature" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Signature.Sign" %>

<link href="media/modules/signature.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">

(function() {

var allSplines = <%=GetCoords() %>;
var curSplines = [];

var enabled = !allSplines.splines;

function loaded() {
  var canvas = document.getElementById('surface');
  var context = canvas.getContext('2d');

  if( allSplines.splines && allSplines.splines.length > 0) {
    for( var idx = 0; idx < allSplines.splines.length; idx++ ) {
      for( var idxI = 0; idxI < allSplines.splines[idx].coords.length; idxI++ ) {
        if(idxI == 0) {
          context.moveTo(allSplines.splines[idx].coords[idxI].x, allSplines.splines[idx].coords[idxI].y);
        } else {
          context.lineTo(allSplines.splines[idx].coords[idxI].x, allSplines.splines[idx].coords[idxI].y);
          context.stroke();
        }
      }
    }
  }
  
  allSplines = [];

  var drawer = {
    isDrawing: false,
    touchstart: function(coors) {
      context.beginPath();
      context.moveTo(coors.x,coors.y);
      this.isDrawing = true;
      curSplines = [];
    },
    touchmove: function(coors) {
      if(this.isDrawing) {
        context.lineTo(coors.x,coors.y);
        context.stroke();
        curSplines.push({x: coors.x, y:coors.y});
      }
    },
    touchend: function(coors) {
      if(this.isDrawing) {
        this.touchmove(coors);
        this.isDrawing = false;
        allSplines.push(curSplines);
      }
    }
  };

  drawer.mousedown = drawer.touchstart;
  drawer.mousemove = drawer.touchmove;
  drawer.mouseup = drawer.touchend;

  function draw(event){

    var curleft = 0;
    var curtop = 0;

    var obj = canvas;
    do {
	  curleft += obj.offsetLeft;
	  curtop += obj.offsetTop;
    } while (obj = obj.offsetParent);

    var posx = 0;
    var posy = 0;

    if (event.pageX || event.pageY) {
      posx = event.pageX;
      posy = event.pageY;
    }
    else if(event.clientX || event.clientY) {
      posx = event.clientX + document.body.scrollLeft + 
        document.documentElement.scrollLeft;
      posy = event.clientY + document.body.scrollTop + 
        document.documentElement.scrollTop;
    }

    var coors = {
      x: (event.targetTouches ? (event.targetTouches[0].pageX - curleft) : (posx - curleft)),
      y: (event.targetTouches ? (event.targetTouches[0].pageY - curtop) : (posy - curtop))
    };
    drawer[event.type](coors);
  }

  if( enabled ) {
    canvas.addEventListener('touchstart',draw, false);
    canvas.addEventListener('touchmove',draw, false);
    document.body.addEventListener('touchend',draw, false);

    canvas.addEventListener('mousedown',draw, false);
    canvas.addEventListener('mousemove',draw, false);
    document.body.addEventListener('mouseup',draw, false);

    canvas.addEventListener('touchmove',
      function(event){
        event.preventDefault();
      },false);
  }
}

MUX.submitSign = function() {
  var points = '{\'splines\':[';
  for(var idx = 0; idx < allSplines.length; idx++ ) {
    if(idx != 0) {
      points += ',';
    }
    points += '{\'coords\':[';
    var cur = allSplines[idx];
    for(var idxI = 0; idxI < cur.length; idxI++ ) {
      if(idxI != 0) {
        points += ',';
      }
      points += '{\'x\':\'' + cur[idxI].x + '\'';
      points += ',\'y\':\'' + cur[idxI].y + '\'}';
    }
    points += ']}';
  }
  points += "]}";
  MUX.Control.callServerMethod('<%=this.ClientID%>.submitSignature', {
    onError: function(status, fullTrace) {
      alert(fullTrace);
    }
  }, [points]);
}

setTimeout(function(){loaded();}, 500);



})();

</script>

<div class="signature">
    <canvas 
        id="surface" 
        width='<%=GetWidth() %>'
        height='<%=GetHeight() %>'
        class="surface"></canvas>
    <input
        runat="server"
        id="sub"
        type="button"
        class="submitSignature"
        onclick="MUX.submitSign();"
        value="Sign!" />
    <mux:Button
        runat="server"
        id="cancel"
        CssClass="submitSignatureCancel"
        OnClick="cancel_Click"
        Text="Cancel" />
</div>






























