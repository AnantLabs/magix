/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
    [ActiveController]
    public class AccessController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.CanLoadPageObject")]
        protected void Magix_Publishing_CanLoadPageObject(object sender, ActiveEventArgs e)
        {
            User user = User.Current;

            if (e.Params.Contains("UserID"))
                user = User.SelectByID(e.Params["ID"].Get<int>());

            bool? haveAccess = HaveAccess(PageObject.SelectByID(e.Params["ID"].Get<int>()), user);
            if (haveAccess.HasValue)
                e.Params["STOP"].Value = !haveAccess.Value;
        }

        private bool? HaveAccess(PageObject po, User user)
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

        private static bool? RoleHaveAccess(PageObject po, Role role)
        {
            while (po != null)
            {
                foreach (PageObjectAccess idx in 
                    PageObjectAccess.Select(Criteria.ExistsIn(po.ID, true)))
                {
                    if (role == null)
                        return false;
                    if (role == idx.Role)
                    {
                        return true;
                    }
                }
                if (PageObjectAccess.CountWhere(Criteria.ExistsIn(po.ID, true)) > 0)
                    return false;
                po = po.Parent;
            }
            return null;
        }

        [ActiveEvent(Name = "Magix.Publishing.GetRolesListForPage")]
        protected void Magix_Publishing_GetRolesListForPage(object sender, ActiveEventArgs e)
        {
            PageObject p = PageObject.SelectByID(e.Params["ID"].Get<int>());
            foreach (Role idx in Role.Select())
            {
                bool? hasAccess = RoleHaveAccess(p, idx);
                if (hasAccess.HasValue)
                {
                    e.Params["ActiveRoles"]["r-" + idx.ID]["ID"].Value = idx.ID;
                    e.Params["ActiveRoles"]["r-" + idx.ID]["Name"].Value = idx.Name;
                    e.Params["ActiveRoles"]["r-" + idx.ID]["HasAccess"].Value = hasAccess.Value;

                    if (PageObjectAccess.CountWhere(
                        Criteria.ExistsIn(p.ID, true),
                        Criteria.ExistsIn(idx.ID, true)) > 0)
                        e.Params["ActiveRoles"]["r-" + idx.ID]["Explicitly"].Value = true;
                    else
                        e.Params["ActiveRoles"]["r-" + idx.ID]["Explicitly"].Value = false;
                }
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.ChangePageAccess")]
        protected void Magix_Publishing_ChangePageAccess(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                PageObject p = PageObject.SelectByID(e.Params["ID"].Get<int>());
                Role r = Role.SelectByID(e.Params["RoleID"].Get<int>());
                PageObjectAccess t = PageObjectAccess.SelectFirst(
                    Criteria.ExistsIn(p.ID, true),
                    Criteria.ExistsIn(r.ID, true));
                if (e.Params["Access"].Get<bool>())
                {
                    if (t == null)
                    {
                        t = new PageObjectAccess();
                        t.Page = p;
                        t.Role = r;
                        t.Save();
                    }
                }
                else
                {
                    if (t != null)
                        t.Delete();
                    else
                    {
                        // Implcitily giving it access, assuming user behavior ...
                        t = new PageObjectAccess();
                        t.Page = p;
                        t.Role = r;
                        t.Save();
                    }
                }
                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.PageObjectDeleted")]
        protected void Magix_Publishing_PageObjectDeleted(object sender, ActiveEventArgs e)
        {
            foreach (PageObjectAccess idx in PageObjectAccess.Select(
                Criteria.ExistsIn(e.Params["ID"].Get<int>(), true)))
            {
                idx.Delete();
            }
        }
    }
}

























