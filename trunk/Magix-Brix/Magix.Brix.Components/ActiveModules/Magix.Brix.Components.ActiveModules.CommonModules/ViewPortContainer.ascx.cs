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
     * Level2: Encapsulates a ViewPort Container Component, basically a container for
     * other modules
     */
    [ActiveModule]
    public class ViewPortContainer : ActiveModule
    {
        protected DynamicPanel dyn;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load += delegate
            {
                if (node.Contains("ChildCssClass"))
                    dyn.CssClass = node["ChildCssClass"].Get<string>();
            };
        }

        /**
         * Level2: Clears the Viewport for Modules. Make sure you pass in the 
         * correct 'Position', and that it matches the 'Container' parameter in the module's
         * DataSource
         */
        [ActiveEvent(Name = "Magix.Core.ClearViewportContainer")]
        protected void Magix_Core_ClearViewportContainer(object sender, ActiveEventArgs e)
        {
            string container = e.Params["Position"].Value as string;

            if (container == DataSource["Container"].Get<string>() ||
                container == Parent.ID)
            {
                foreach (Control idx in dyn.Controls)
                {
                    ActiveEvents.Instance.RemoveListener(idx);
                }
                dyn.ClearControls();
            }
        }

        /**
         * Level2: Handled to be able to clear container upon parent container cleared ...
         */
        [ActiveEvent(Name = "Magix.Core.ContainerAboutToBeCleared")]
        protected void Magix_Core_ContainerAboutToBeCleared(object sender, ActiveEventArgs e)
        {
            string container = e.Params["Container"].Value as string;

            if (container == Parent.ID)
            {
                foreach (Control idx in dyn.Controls)
                {
                    ActiveEvents.Instance.RemoveListener(idx);
                }
                dyn.ClearControls();
            }
        }

        /**
         * Level2: Will return the number of Active Modules [or controls] a specific
         * Viewport Container contains. Useful for determining if a specific container
         * is available or not
         */
        [ActiveEvent(Name = "Magix.Core.GetNumberOfChildrenOfContainer")]
        protected void Magix_Core_GetNumberOfChildrenOfContainer(object sender, ActiveEventArgs e)
        {
            if (e.Params["Container"].Get<string>() == DataSource["Container"].Get<string>())
            {
                e.Params["Count"].Value = dyn.Controls.Count;
            }
        }

        /**
         * Level2: Handled to make it possible to load Active Modules into this Viewport's containers.
         * Make sure the 'Position' parameter matches the 'Container' parameter passed in during
         * loading the ViewPortContainer module
         */
        [ActiveEvent(Name = "Magix.Core.LoadActiveModule")]
        protected void Magix_Core_LoadActiveModule(object sender, ActiveEventArgs e)
        {
            string moduleName = e.Params["Name"].Get<string>();
            string position = e.Params["Position"].Get<string>();

            if (position == DataSource["Container"].Get<string>())
            {
                if (e.Params["Parameters"].Contains("Append") &&
                    e.Params["Parameters"]["Append"].Get<bool>())
                {
                    dyn.AppendControl(
                        moduleName,
                        e.Params["Parameters"]);

                    if (e.Params["Parameters"].Contains("AppendMaxCount"))
                    {
                        int count = e.Params["Parameters"]["AppendMaxCount"].Get<int>();
                        while (dyn.Controls.Count > count)
                        {
                            dyn.RemoveFirst();
                        }
                    }
                }
                else
                {
                    dyn.LoadControl(moduleName, e.Params["Parameters"]);
                }
            }
        }

        protected void dynamic_LoadControls(object sender, DynamicPanel.ReloadEventArgs e)
        {
            DynamicPanel dynamic = sender as DynamicPanel;
            string moduleName = e.Key;
            Control ctrl = PluginLoader.Instance.LoadActiveModule(moduleName);
            if (e.FirstReload)
            {
                Node nn = e.Extra as Node;
                ctrl.Init +=
                    delegate
                    {
                        IModule module = ctrl as IModule;
                        if (module != null)
                        {
                            module.InitialLoading(nn);
                        }
                    };

                ctrl.Load +=
                    delegate
                    {
                        if (nn != null &&
                            nn.Contains("ModuleInitializationEvent") &&
                            !string.IsNullOrEmpty(nn["ModuleInitializationEvent"].Get<string>()))
                        {
                            nn["_ctrl"].Value = ctrl;

                            RaiseSafeEvent(
                                nn["ModuleInitializationEvent"].Get<string>(),
                                nn);
                            nn["_ctrl"].UnTie();
                        }
                    };
            }
            if (e.InsertAtBeginning)
                dynamic.Controls.AddAt(0, ctrl);
            else
                dynamic.Controls.Add(ctrl);
        }
    }
}
