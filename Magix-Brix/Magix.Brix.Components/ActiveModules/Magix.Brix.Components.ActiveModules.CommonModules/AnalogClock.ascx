<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.CommonModules.AnalogClock" %>

<mux:Panel 
    runat="server" 
    id="pnl">
    <canvas
        id="myDrawing"
        runat="server"
        width="370"
        height="300">
    </canvas>
</mux:Panel>

<script type="text/ecmascript">
(function() {
    function InitClock() {
    MUX.clearRect = function(context) {
        context.clearRect(0,0,370,300);
        MUX.reDraw(context);
    }

    MUX.reDraw = function(context) {
        var now = new Date();
        var hours = now.getHours();
        var minutes = now.getMinutes();
        var seconds = now.getMilliseconds() + (now.getSeconds() * 1000);
        MUX.calculateHandPositions(context,185,150,hours,minutes,seconds);
    }

    MUX.calculateHandPositions = function(context,posX,posY,h,m,s) {
        var secondHandLength = 145;
        var secondHandAngle = 2 * Math.PI * s / 60000;
        MUX.drawHand(context,posX,posY,secondHandLength,secondHandAngle,'rgba(0,0,0,.2)',2);

        var minuteHandLength = 140;
        var minuteHandAngle = 2 * Math.PI * m / 60;
        MUX.drawHand(context,posX,posY,minuteHandLength,minuteHandAngle,'rgba(0,0,0,.2)',5);

        var hourHandLength = 100;
        var hourHandAngle = ( 2 * Math.PI * h / 12) + (( 2 * Math.PI * m) / (12 * 60));
        MUX.drawHand(context,posX,posY,hourHandLength,hourHandAngle,'rgba(0,0,0,.2)',8);
    }

    MUX.drawHand = function(context,posX,posY,handLength,handAngle,color,width) {
        context.beginPath();
        context.moveTo(posX,posY);
        var x = posX + (handLength * Math.sin(handAngle));
        var y = posY - (handLength * Math.cos(handAngle));
        context.lineTo(x,y);
        context.closePath();
        context.strokeStyle = color;
        context.lineWidth = width;
        context.stroke();
    }

    var drawingCanvas = MUX.$('<%=myDrawing.ClientID%>');

    if(drawingCanvas.getContext) {
        MUX.context=drawingCanvas.getContext('2d');
        setInterval('MUX.clearRect(MUX.context)',100);
    }
    }
if (window.addEventListener) {
  window.addEventListener('load', InitClock, false);
} else {
  window.attachEvent('onload', InitClock);
}
})();
</script>
