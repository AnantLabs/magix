/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using System.Collections.Generic;

namespace Magix.Bri.Components.ActiveControllers.ClipBoard
{
    /**
     * Level2: Encapsulates the logic needed for manipulating the clipboard, add items to it, and 
     * paste items
     */
    [ActiveController]
    public class ClipBoard_Controller : ActiveController
    {
        /**
         * Level2: Will take the incoming 'ClipBoardNode' and append it into the current user's
         * Session object
         */
        [ActiveEvent(Name = "Magix.ClipBoard.CopyNode")]
        protected void Magix_ClipBoard_CopyNode(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("ClipBoardNode"))
                throw new ArgumentException("You need to add up a ClipBoardNode parameter if you want to append to the clipboard");

            Node tmp = e.Params["ClipBoardNode"].Get<Node>();
            if (tmp == null)
                throw new ArgumentException("You need to add up a ClipBoardNode parameter if you want to append to the clipboard. And this Parameter must be of type Node");

            ClipBoard.Add(tmp);
        }

        private List<Node> ClipBoard
        {
            get
            {
                if (Page.Session["Magix.Brix.Components.ActiveControllers.ClipBoard"] == null)
                    Page.Session["Magix.Brix.Components.ActiveControllers.ClipBoard"] = new List<Node>();
                return Page.Session["Magix.Brix.Components.ActiveControllers.ClipBoard"] as List<Node>;
            }
        }
    }
}
