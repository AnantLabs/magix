/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.UX.Effects;

namespace Magix.Brix.Components
{
    public abstract class NestedDynamic : UserControl, IModule
    {
        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindObjects();
        }

        protected abstract void DataBindObjects();

        protected Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }

        protected int TotalCount
        {
            get { return DataSource["TotalCount"].Get<int>(); }
        }

        protected string FullTypeName
        {
            get { return DataSource["FullTypeName"].Get<string>(); }
        }

        protected string TypeName
        {
            get { return DataSource["FullTypeName"].Get<string>().Substring(DataSource["FullTypeName"].Get<string>().LastIndexOf(".") + 1); }
        }

        protected string ParentPropertyName
        {
            get { return DataSource["ParentPropertyName"].Get<string>(); }
        }

        protected string ParentType
        {
            get { return DataSource["ParentType"].Get<string>(); }
        }

        protected string ParentFullType
        {
            get { return DataSource["ParentFullType"].Get<string>(); }
        }

        protected int ParentID
        {
            get { return DataSource["ParentID"].Get<int>(); }
        }

        protected abstract void ReDataBind();

        [ActiveEvent(Name = "RefreshWindowContent")]
        protected virtual void RefreshWindowContent(object sender, ActiveEventArgs e)
        {
            if (ClientID.IndexOf(e.Params["ClientID"].Get<string>()) == 0)
            {
                ReDataBind();
            }
        }
    }
}
