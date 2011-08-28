/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Helps feed the SliderMenu in Front-Web to get its Items to build its structure upon
     */
    [ActiveController]
    public class SliderMenu_Controller : ActiveController
    {
        /**
         * Level2: Will return a node containing all the menu items in your Pages hierarchy, possibly
         * according to how you've got access to them, as long as the Access Controller is in no
         * ways jeopardized
         */
        [ActiveEvent(Name = "Magix.Publishing.GetSliderMenuItems")]
        protected void Magix_Publishing_GetSliderMenuItems(object sender, ActiveEventArgs e)
        {
            WebPage root = null;
            foreach (WebPage idx in WebPage.Select())
            {
                if (idx.Parent != null)
                    continue; // Looking for Root Page and starting traversal from it ...

                root = idx;

                GetOneMenuItem(e.Params, idx, true);
            }
            if (!e.Params.Contains("Items"))
            {
                // Finding first page level user [or anonymous] have access to from here ...
                Node node = new Node();
                node["ID"].Value = root.ID;

                RaiseEvent(
                    "Magix.Publishing.FindFirstChildPageUserCanAccess",
                    node);

                if (node.Contains("AccessToID"))
                {
                    // First [breadth-first] page menu object user has access too ...
                    GetOneMenuItem(e.Params, WebPage.SelectByID(node["AccessToID"].Get<int>()), true);
                }
            }
        }

        /**
         * Level2: Will build up the Node structure for one Menu item, first verifying the User
         * has access to that particular page by raising the
         * 'Magix.Publishing.GetRolesListForPage' event
         */
        private void GetOneMenuItem(Node node, WebPage po, bool isRoot)
        {
            bool canAccess = true;
            if (User.Current != null)
            {
                canAccess = CheckAccessForUser(po, canAccess);
            }
            else
            {
                canAccess = CheckAccessForAnonymous(po, canAccess);
            }

            if (canAccess)
            {
                BuildNodeForOneMenuItem(node, po, isRoot);
            }
        }

        /*
         * Verifies that the User.Current has access to the spesific WebPage
         */
        private bool CheckAccessForUser(WebPage po, bool canAccess)
        {
            foreach (Role idx in User.Current.Roles)
            {
                Node xx = new Node();

                xx["ID"].Value = po.ID;

                RaiseEvent(
                    "Magix.Publishing.GetRolesListForPage",
                    xx);

                if (xx.Contains("ActiveRoles") &&
                    !xx["ActiveRoles"]["r-" + idx.ID]["HasAccess"].Get<bool>())
                {
                    canAccess = false;
                }
            }
            return canAccess;
        }

        /*
         * Checks access to the specific page for an anonymous visitor 
         * [not in any ways logged in]
         */
        private bool CheckAccessForAnonymous(WebPage po, bool canAccess)
        {
            Node xx = new Node();

            xx["ID"].Value = po.ID;

            RaiseEvent(
                "Magix.Publishing.GetRolesListForPage",
                xx);

            if (xx.Contains("ActiveRoles"))
            {
                canAccess = false;
            }
            return canAccess;
        }

        /*
         * Will build up the Node structure needed for one Menu item in the
         * front-web
         */
        private void BuildNodeForOneMenuItem(Node node, WebPage po, bool isRoot)
        {
            node["Items"]["i" + po.ID]["Caption"].Value = po.Name;
            node["Items"]["i" + po.ID]["Event"].Value = "Magix.Publishing.SliderMenuItemClicked";
            node["Items"]["i" + po.ID]["Event"]["WebPageURL"].Value = po.URL;
            foreach (WebPage idx in po.Children)
            {
                if (isRoot) // We inject root on the same node as the children of root ...
                    GetOneMenuItem(node, idx, false);
                else
                    GetOneMenuItem(node["Items"]["i" + po.ID], idx, false);
            }
        }

        /**
         * Level2: Will Open the specific WebPage accordingly to which SlidingMenuItem was clicked
         */
        [ActiveEvent(Name = "Magix.Publishing.SliderMenuItemClicked")]
        protected void Magix_Publishing_SliderMenuItemClicked(object sender, ActiveEventArgs e)
        {
            WebPage o = WebPage.SelectFirst(Criteria.Eq("URL", e.Params["WebPageURL"].Get<string>()));

            Node node = new Node();

            node["ID"].Value = o.ID;

            RaiseEvent(
                "Magix.Publishing.OpenPage",
                node);
        }
    }
}
