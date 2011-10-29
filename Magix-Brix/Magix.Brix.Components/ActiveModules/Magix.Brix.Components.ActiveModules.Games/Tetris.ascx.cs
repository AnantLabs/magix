/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX;
using Magix.UX.Widgets.Core;

[assembly: WebResource("Magix.Brix.Components.ActiveModules.Games.tetris.js", "text/javascript")]


namespace Magix.Brix.Components.ActiveModules.Games
{
    /**
     * Level2: Contains the Tetris game
     */
    [ActiveModule]
    public class Tetris : ActiveModule
    {
        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!AjaxManager.Instance.IsCallback || FirstLoad)
            {
                AjaxManager.Instance.IncludeScriptFromResource(
                    typeof(Tetris),
                    "Magix.Brix.Components.ActiveModules.Games.tetris.js",
                    false);

                AjaxManager.Instance.WriterAtBack.Write("MUX.tetris.init();");
            }
        }
    }
}
