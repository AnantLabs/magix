/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;
using Magix.UX.Core;

namespace Magix.Brix.Components.ActiveModules.Signature
{
    /**
     * Level2: A Signature module, basically a place where the end user can 'sign his name' to
     * confirm a transaction of some sort. Will load up a big white thing, which can be 'drawn
     * upon', together with two buttons, OK and Cancel which will raise the 
     * 'CancelEvent' and the 'OKEvent. 'OKEvent' will pass in 'Signature' being the coords
     * for all the splines that comprises the Signature, which can be stored and later
     * used as input to this Module, which will again load those splines in 'read-only mode'
     */
    [ActiveModule]
    public class Sign : ActiveModule
    {
        protected HtmlInputButton sub;
        protected Button cancel;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    DataSource = node;

                    if (DataSource.Contains("Coords") &&
                        !string.IsNullOrEmpty(DataSource["Coords"].Get<string>()))
                    {
                        sub.Visible = false;
                        cancel.Text = "Close";
                    }
                };
        }

        protected string GetCoords()
        {
            if (DataSource.Contains("Coords") &&
                !string.IsNullOrEmpty(DataSource["Coords"].Get<string>()))
                return DataSource["Coords"].Get<string>();
            return "[]";
        }

        protected int GetWidth()
        {
            if (DataSource.Contains("Width"))
                return DataSource["Width"].Get<int>();
            return 710;
        }

        protected int GetHeight()
        {
            if (DataSource.Contains("Height"))
                return DataSource["Height"].Get<int>();
            return 360;
        }

        [WebMethod]
        protected void submitSignature(string signature)
        {
            DataSource["OKEvent"]["Params"]["Signature"].Value = signature;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DataSource["OKEvent"].Get<string>(),
                DataSource["OKEvent"]["Params"]); // TODO: Standardize how we pass parameters like these somehow intelligently ...??
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                DataSource["CancelEvent"].Get<string>(),
                DataSource["CancelEvent"]["Params"]);
        }
    }
}
