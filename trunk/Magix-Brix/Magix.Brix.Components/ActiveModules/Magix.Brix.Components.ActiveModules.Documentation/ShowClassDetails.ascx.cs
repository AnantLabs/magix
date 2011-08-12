/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using ASP = System.Web.UI.WebControls;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.Documentation
{
    [ActiveModule]
    public class ShowClassDetails : ActiveModule, IModule
    {
        protected Label header;
        protected Label content;
        protected System.Web.UI.WebControls.Repeater rep;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load += delegate
            {
                header.Text = node["FullName"].Get<string>();
                content.Text = node["Description"].Get<string>();
                rep.DataSource = node["Methods"];
                rep.DataBind();
            };
        }

        protected string GetParams(object obj)
        {
            Node n = obj as Node;

            string retVal = "";

            foreach (Node idx in n)
            {
                if (!string.IsNullOrEmpty(retVal))
                    retVal += ", ";
                retVal += idx["Type"].Get<string>() + 
                    " " +
                    idx["Name"].Get<string>();
            }
            if (!string.IsNullOrEmpty(retVal))
                return "(" + retVal + ")";
            return "";
        }
    }
}



