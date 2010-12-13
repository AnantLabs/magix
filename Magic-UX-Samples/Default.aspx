<%@ Page 
    Language="C#" 
    AutoEventWireup="true" 
    CodeFile="Default.aspx.cs" 
    Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Main MagicUX samples</title>
    <script type="text/ecmascript">

function foo() {
  var el = MUX.$('scr');
  new MUX.Effect.Generic('scr', {
    transition: 'Explosive',
    start: function() {
      this._beginning = parseInt(MUX.$('scr').style.marginTop, 10) || 0;
    },
    loop: function(pos) {
      var delta = -(pos * 100);
      el.setStyle('marginTop', this._beginning + delta + 'px');
    },
    end: function() {
      el.setStyle('marginTop', this._beginning - 100 + 'px');
    },
    duration: 250
  });
}

function foo2() {
  var el = MUX.$('scr');
  new MUX.Effect.Generic('scr', {
    transition: 'Explosive',
    start: function() {
      this._beginning = parseInt(MUX.$('scr').style.marginTop, 10) || 0;
    },
    loop: function(pos) {
      var delta = pos * 100;
      el.setStyle('marginTop', this._beginning + delta + 'px');
    },
    end: function() {
      el.setStyle('marginTop', this._beginning + 100 + 'px');
    },
    duration: 250
  });
}

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <mux:Label runat="server" ID="lbl" Text="X" style="position:absolute;top:50px;left:500px;">
            <mux:AspectDraggable runat="server" ID="drg" Bounds="500, 50, 550, 75" />
        </mux:Label>
        <input type="button" value="Scroll Down" onclick="foo();" />
        <input type="button" value="Scroll Up" onclick="foo2();" />
        <div style="border:solid 1px Black;width:250px;height:200px;position:absolute;top:100px;left:250px;overflow:hidden;">
            <div id="scr" style="width:1000px;height:1000px;">
                <iframe style="width:100%;height:100%;" src="http://ra-ajax.org" frameborder="0" scrolling="no">
                </iframe>
            </div>
        </div>
        <div>
            <ul>
                <li>
                    <a href="Timer/Default.aspx">Timer</a>
                </li>
                <li>
                    <a href="Calendar/Default.aspx">Calendar</a>
                </li>
                <li>
                    <a href="AspectUpdater/Default.aspx">AspectUpdater</a>
                </li>
                <li>
                    <a href="Accordion/Default.aspx">Accordion</a>
                </li>
                <li>
                    <a href="Button/Default.aspx">Button</a>
                </li>
                <li>
                    <a href="CheckBox/Default.aspx">CheckBox</a>
                </li>
                <li>
                    <a href="HiddenField/Default.aspx">HiddenField</a>
                </li>
                <li>
                    <a href="HyperLink/Default.aspx">HyperLink</a>
                </li>
                <li>
                    <a href="Image/Default.aspx">Image</a>
                </li>
                <li>
                    <a href="ImageButton/Default.aspx">ImageButton</a>
                </li>
                <li>
                    <a href="Label/Default.aspx">Label</a>
                </li>
                <li>
                    <a href="LinkButton/Default.aspx">LinkButton</a>
                </li>
                <li>
                    <a href="Menu/Default.aspx">Menu</a>
                </li>
                <li>
                    <a href="MultiPanel/Default.aspx">MultiPanel</a>
                </li>
                <li>
                    <a href="Panel/Default.aspx">Panel</a>
                </li>
                <li>
                    <a href="RadioButton/Default.aspx">RadioButton</a>
                </li>
                <li>
                    <a href="SelectList/Default.aspx">SelectList</a>
                </li>
                <li>
                    <a href="SlidingMenu/Default.aspx">SlidingMenu</a>
                </li>
                <li>
                    <a href="TextArea/Default.aspx">TextArea</a>
                </li>
                <li>
                    <a href="TextBox/Default.aspx">TextBox</a>
                </li>
                <li>
                    <a href="TreeView/Default.aspx">TreeView</a>
                </li>
                <li>
                    <a href="Window/Default.aspx">Window</a>
                </li>
            </ul>
        </div>
    </form>
</body>
</html>
