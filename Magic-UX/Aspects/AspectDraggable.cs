/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Drawing;
using System.ComponentModel;
using Magic.UX.Core;
using Magic.UX.Widgets;
using Magic.UX.Helpers;
using Magic.UX.Widgets.Core;

namespace Magic.UX.Aspects
{
    /**
     * Aspect for creating controls that can be moved around on the screen using your mouse. 
     * It is also possible to handle the Dragged event and track when the dropping of the
     * control occurs.
     */
    public class AspectDraggable : AspectBase
	{
        /**
         * Handle this event to track when the control is dragged and dropped by the user.
         * If this one is ommitted, there will not be raised an Ajax Request at all when
         * the control is moved, and hence the ViewState value of the top/left corner
         * of the control will never be updated.
         */
		public event EventHandler Dragged;

        /**
         * The bounding rectangle that the control is possible to move within. 
         * Rectangular constraints. The default is 'no constraints'.
         */
        public Rectangle Bounds
        {
            get { return ViewState["Bounds"] == null ? Rectangle.Empty : (Rectangle)ViewState["Bounds"]; }
            set
            {
                if (value != Bounds)
                    SetJsonValue("Bounds", value);
                ViewState["Bounds"] = value;
            }
        }

        /**
         * The grid that the control will be moved within. If this one is set to 50,50 then
         * the control will snap into a 'grid' of 50 pixels large areas. Useful for making
         * sure a control is being moved only within some rectangular constraints.
         */
        public Point Snap
        {
            get { return ViewState["Snap"] == null ? Point.Empty : (Point)ViewState["Snap"]; }
            set
            {
                if (value != Snap)
                    SetJsonValue("Snap", value);
                ViewState["Snap"] = value;
            }
        }

        /**
         * The handle from which the control itself will be able to drag from. If ommitted
         * you can drag the control from any place inside of the control. If a handle is 
         * defined, then the control can only be dragged from this specific handle.
         * The Window control is a good example of utilizing this logic. You can drag the
         * entire Window, which means the Window has an AspectDraggable associated with
         * it. But you can only drag the Window by its Caption, which is the handle
         * for the AspectDraggable within the Window.
         */
        public string Handle
        {
            get { return ViewState["Handle"] == null ? "" : (string)ViewState["Handle"]; }
            set
            {
                if (value != Handle)
                    SetJsonValue("Handle", value);
                ViewState["Handle"] = value;
            }
        }

        /**
         * If this one is false, then the control cannot be dragged. Useful for having 
         * an AspectDraggable which might be enabled or not on the fly. The Window is
         * a good example of this. It always have the AspectDraggable associated with
         * it, but if the Window is being set to 'non-movable', then the AspectDraggable
         * will be disabled.
         */
        public bool Enabled
        {
            get { return ViewState["Enabled"] == null ? true : (bool)ViewState["Enabled"]; }
            set
            {
                if (value != Enabled)
                    SetJsonValue("Enabled", value);
                ViewState["Enabled"] = value;
            }
        }

		public override string GetAspectRegistrationScript()
		{
			string options = string.Empty;
            if (Bounds != Rectangle.Empty)
            {
                options = string.Format(",{{bounds:{{left:{0},top:{1},width:{2},height:{3}}}",
                    Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
            }
            if (Snap != Point.Empty)
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{", 
                    ",", 
                    string.Format("snap:{{x:{0},y:{1}}}", Snap.X, Snap.Y));
            }
            if (!string.IsNullOrEmpty(Handle))
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    string.Format("handle:MUX.$('{0}')", Handle));
            }
            if (Dragged != null)
            {
                options = StringHelper.ConditionalAdd(
                    options,
                    ",{",
                    ",",
                    "callback:true");
            }

            options = StringHelper.ConditionalAdd(
                options,
                ",{",
                ",",
                string.Format("enabled:{0}", Enabled.ToString().ToLower()));

            if (!string.IsNullOrEmpty(options))
                options += "}";

			return string.Format("new MUX.AspectDraggable('{0}'{1})", this.ClientID, options);
		}

        public override void EnsureViewStateLoads()
        {
            if (Page.Request.Params["__MUX_CONTROL_CALLBACK"] == this.ClientID &&
                Page.Request.Params["__MUX_EVENT"] == "Moved")
            {
                BaseWebControl parent = Parent as BaseWebControl;
                parent.Style.SetStyleValueViewStateOnly("left", Page.Request.Params["x"] + "px");
                parent.Style.SetStyleValueViewStateOnly("top", Page.Request.Params["y"] + "px");
                if (parent.Style[Styles.position] != "fixed")
                    parent.Style.SetStyleValueViewStateOnly("position", "absolute");
            }
        }

        public override void RaiseEvent(string name)
        {
            switch (name)
            {
                case "Moved":
                    if (Dragged != null)
                        Dragged(this, new EventArgs());
                    break;
                default:
                    base.RaiseEvent(name);
                    break;
            }
        }
    }
}
