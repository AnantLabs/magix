/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.UX;

namespace Magix.Brix.Components.ActiveModules.CommonModules
{
    /**
     * Level2: Shows an Analog Clock module
     */
    [ActiveModule]
    public class AnalogClock : ActiveModule
    {
        protected Panel pnl;
        protected System.Web.UI.HtmlControls.HtmlGenericControl myDrawing;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    if (node.Contains("ChildCssClass"))
                        pnl.CssClass = node["ChildCssClass"].Get<string>();
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (FirstLoad || !AjaxManager.Instance.IsCallback)
            {
                AjaxManager.Instance.WriterAtBack.Write(@"
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

    var drawingCanvas = MUX.$('" + myDrawing.ClientID + @"');

    if(drawingCanvas.getContext) {
        MUX.context = drawingCanvas.getContext('2d');
        setInterval(function() {
          MUX.clearRect(MUX.context);
        }, 100);
    }
");
            }
        }
    }
}
