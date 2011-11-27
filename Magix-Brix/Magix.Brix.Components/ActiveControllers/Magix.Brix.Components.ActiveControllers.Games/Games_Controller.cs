/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.Logger
{
    /**
     * Level2: Contains logic to load up games in menu etc
     */
    [ActiveController]
    public class Games_Controller : ActiveController
    {
        /**
         * Level2: Sink for allowing the System Games to become Menu Items in the Administrator Menu
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Other"]["Caption"].Value = "Other";
            e.Params["Items"]["Other"]["Items"]["Games"]["Caption"].Value = "Games";
            e.Params["Items"]["Other"]["Items"]["Games"]["Items"]["Tetris"]["Caption"].Value = "Tetris";
            e.Params["Items"]["Other"]["Items"]["Games"]["Items"]["Tetris"]["Event"]["Name"].Value = "Magix.Games.OpenTetris";
        }

        /**
         * Level2: Will open up Tetris and let the end user allow to play Tetris
         */
        [ActiveEvent(Name = "Magix.Games.OpenTetris")]
        protected void Magix_Games_OpenTetris(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Last"].Value = true;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Games.Tetris",
                "content3",
                node);

            node["Caption"].Value = "Tetris";

            RaiseEvent(
                "Magix.Core.SetFormCaption",
                node);
        }
    }
}
