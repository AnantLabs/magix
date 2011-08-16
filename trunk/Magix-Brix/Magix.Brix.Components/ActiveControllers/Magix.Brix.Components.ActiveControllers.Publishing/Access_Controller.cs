/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Data;
using System.Reflection;
using Magix.Brix.Publishing.Common;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level3: Helps out sorting out which Pages and Menu Items which Users and Roles 
     * and such have access to
     */
    [ActiveController]
    public class Access_Controller : ActiveController
    {
        /**
         * Level3: Returns 'STOP' to true unless User has access to Page, either explicitly
         * or implicitly
         */
        [ActiveEvent(Name = "Magix.Publishing.VerifyUserHasAccessToPage")]
        protected void Magix_Publishing_VerifyUserHasAccessToPage(object sender, ActiveEventArgs e)
        {
            User user = User.Current;

            if (e.Params.Contains("UserID"))
                user = User.SelectByID(e.Params["ID"].Get<int>());

            bool? haveAccess = UerHaveAccess(WebPage.SelectByID(e.Params["ID"].Get<int>()), user);
            if (haveAccess.HasValue)
                e.Params["STOP"].Value = !haveAccess.Value;
        }

        /**
         * Level3: Will recursively traverse children until it find the first page [breadth first] User has access
         * to from given page
         */
        [ActiveEvent(Name = "Magix.Publishing.FindFirstChildPageUserCanAccess")]
        protected void Magix_Publishing_FindFirstChildPageUserCanAccess(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());

            // Assuming already checked for access against this bugger ...
            foreach (WebPage idx in p.Children)
            {
                if (GetFirstAccessiblePageForUser(p, e.Params))
                {
                    return;
                }
            }
        }

        /*
         * Checks to see if access against specific page. Will recursively traverse inwards and return
         * the first Page the user has access to, if not this one
         */
        private bool GetFirstAccessiblePageForUser(WebPage p, Node node)
        {
            Node ch1 = new Node();
            ch1["ID"].Value = p.ID;

            RaiseEvent(
                "Magix.Publishing.VerifyUserHasAccessToPage",
                ch1);

            if (!ch1.Contains("STOP") ||
                !ch1["STOP"].Get<bool>())
            {
                node["AccessToID"].Value = p.ID;
                return true;
            }
            foreach (WebPage idx in p.Children)
            {
                if (GetFirstAccessiblePageForUser(idx, node))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Returns true if user has Access to given WebPage
         * Will traverse all Roles for user and check each role 
         * until hit is found, or giving up
         */
        private bool? UerHaveAccess(WebPage po, User user)
        {
            if (user != null)
            {
                foreach (Role idx in user.Roles)
                {
                    bool? val = RoleHaveAccess(po, idx);
                    if (val.HasValue)
                        return val.Value;
                }
            }
            else
            {
                bool? val = RoleHaveAccess(po, null);
                if (val.HasValue)
                    return val.Value;
            }
            return null;
        }

        /*
         * Returns true if user has Access to given WebPage
         */
        private static bool? RoleHaveAccess(WebPage po, Role role)
        {
            while (po != null)
            {
                foreach (WebPageRoleAccess idx in 
                    WebPageRoleAccess.Select(Criteria.ExistsIn(po.ID, true)))
                {
                    if (role == null)
                        return false;
                    if (role == idx.Role)
                    {
                        return true;
                    }
                }
                if (WebPageRoleAccess.CountWhere(Criteria.ExistsIn(po.ID, true)) > 0)
                    return false;
                po = po.Parent;
            }
            return null;
        }

        /**
         * Level3: Will return a List of Roles that have explicit access [or not] to the given Page
         */
        [ActiveEvent(Name = "Magix.Publishing.GetRolesListForPage")]
        protected void Magix_Publishing_GetRolesListForPage(object sender, ActiveEventArgs e)
        {
            WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());
            foreach (Role idx in Role.Select())
            {
                bool? hasAccess = RoleHaveAccess(p, idx);
                if (hasAccess.HasValue)
                {
                    e.Params["ActiveRoles"]["r-" + idx.ID]["ID"].Value = idx.ID;
                    e.Params["ActiveRoles"]["r-" + idx.ID]["Name"].Value = idx.Name;
                    e.Params["ActiveRoles"]["r-" + idx.ID]["HasAccess"].Value = hasAccess.Value;

                    if (WebPageRoleAccess.CountWhere(
                        Criteria.ExistsIn(p.ID, true),
                        Criteria.ExistsIn(idx.ID, true)) > 0)
                        e.Params["ActiveRoles"]["r-" + idx.ID]["Explicitly"].Value = true;
                    else
                        e.Params["ActiveRoles"]["r-" + idx.ID]["Explicitly"].Value = false;
                }
            }
        }

        /**
         * Level3: Will change the Access rights for a specific page
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangePageAccess")]
        protected void Magix_Publishing_ChangePageAccess(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPage p = WebPage.SelectByID(e.Params["ID"].Get<int>());

                Role r = Role.SelectByID(e.Params["RoleID"].Get<int>());

                WebPageRoleAccess t = WebPageRoleAccess.SelectFirst(
                    Criteria.ExistsIn(p.ID, true),
                    Criteria.ExistsIn(r.ID, true));

                if (e.Params["Access"].Get<bool>())
                {
                    if (t == null)
                    {
                        t = new WebPageRoleAccess();
                        t.Page = p;
                        t.Role = r;
                        t.Save();
                    } // No need for else here ...
                }
                else
                {
                    if (t != null)
                        t.Delete();
                }
                tr.Commit();
            }
        }

        /**
         * Level3: Handled to make sure we delete all WebPageRoleAccess objects
         * belonging to WebPage too
         */
        [ActiveEvent(Name = "Magix.Publishing.PageObjectDeleted")]
        protected void Magix_Publishing_PageObjectDeleted(object sender, ActiveEventArgs e)
        {
            foreach (WebPageRoleAccess idx in WebPageRoleAccess.Select(
                Criteria.ExistsIn(e.Params["ID"].Get<int>(), true)))
            {
                idx.Delete();
            }
        }
    }
}

























