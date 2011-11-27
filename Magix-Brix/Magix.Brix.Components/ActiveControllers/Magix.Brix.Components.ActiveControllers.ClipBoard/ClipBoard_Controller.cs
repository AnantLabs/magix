/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveControllers.ClipBoard
{
    /**
     * Level2: Encapsulates the logic needed for manipulating the clipboard, add items to it, and 
     * paste items to the Clipboard. Notice that by overriding some of these events, and 
     * have them go to other servers, you can copy and paste any clipboard items from one server
     * to the other
     */
    [ActiveController]
    public class ClipBoard_Controller : ActiveController
    {
        /**
         * Level2: Will return the menu items needed to fire up 'View Clipboard'
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            if (!e.Params["Items"].Contains("Admin"))
            {
                e.Params["Items"]["Admin"]["Caption"].Value = "Admin";
                e.Params["Items"]["Admin"]["Items"]["System"]["Caption"].Value = "System";
            }

            e.Params["Items"]["Admin"]["Items"]["ClipBoard"]["Caption"].Value = "Clipboard ...";
            e.Params["Items"]["Admin"]["Items"]["ClipBoard"]["Event"]["Name"].Value = "Magix.ClipBoard.ShowClipBoard";
        }

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

            RaiseEvent("Magix.ClipBoard.ShowClipBoard");
        }

        /**
         * Level2: Will load the clipboard module into the floater Container
         */
        [ActiveEvent(Name = "Magix.ClipBoard.ShowClipBoard")]
        protected void Magix_ClipBoard_ShowClipBoard(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["CssClass"].Value = "mux-clipboard";

            int idxNo = 0;
            foreach (Node idx in ClipBoard)
            {
                string tooltip = idx.ToJSONString();
                if (tooltip.Length > 200)
                    tooltip = tooltip.Substring(0, 200) + "...";
                node["ClipBoard"]["i-" + idxNo]["Name"].Value = idx.Name;
                node["ClipBoard"]["i-" + idxNo]["ToolTip"].Value = tooltip;
                node["ClipBoard"]["i-" + idxNo]["ID"].Value = idxNo;
                idxNo += 1;
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.ClipBoard.Clips",
                "floater",
                node);
        }

        /**
         * Level2: Will erase the given 'ID' item from the ClipBoard
         */
        [ActiveEvent(Name = "Magix.ClipBoard.RemoveItemFromClipBoard")]
        protected void Magix_ClipBoard_RemoveItemFromClipBoard(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            ClipBoard.RemoveAt(id);

            if (ClipBoard.Count > 0)
            {
                // TODO: Too easy of a shortcut ...
                RaiseEvent("Magix.ClipBoard.ShowClipBoard");
            }
            else
            {
                ActiveEvents.Instance.RaiseClearControls("floater");
            }
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

        /**
         * Level2; Will prepare to send the PasteItem event. Expects the 'ID' to point to a ClipBoard 
         * Item ID
         */
        [ActiveEvent(Name = "Magix.ClipBoard.PrepareToPasteNode")]
        protected void Magix_ClipBoard_PrepareToPasteNode(object sender, ActiveEventArgs e)
        {
            Node node = ClipBoard[e.Params["ID"].Get<int>()];

            Node tmp = new Node();

            tmp["PasteNode"].Value = node;

            RaiseEvent(
                "Magix.ClipBoard.PasteItem",
                tmp);
        }

        /**
         * Level2: Closes the Clipboard Module, if it is up
         */
        [ActiveEvent(Name = "Magix.ClipBoard.CloseClipBoard")]
        protected void Magix_ClipBoard_CloseClipBoard(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("floater");
            ShowMessage(
                "You can Open the Clipboard again using the Menu Item Admin/Clipboard ...", 
                "Hint, hint ...", 
                3500);
        }
    }
}
