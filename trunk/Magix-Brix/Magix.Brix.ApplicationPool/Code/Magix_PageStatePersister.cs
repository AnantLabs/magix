/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Web.UI;
using System.IO;
using Magix.Brix.Data;
using System.Text;
using System.Web;
using Magix.UX;

namespace Magix.Brix.ApplicationPool
{
    /**
     * Level4: Implements serialization of ViewState into the database on the server side
     */
    public class Magix_PageStatePersister : PageStatePersister
    {
        private Guid _session;

        public Magix_PageStatePersister(Page page)
            : base(page)
        {
            if (page.IsPostBack)
            {
                _session = new Guid(page.Request["__VIEWSTATE_KEY"]);
            }
            else
            {
                _session = Guid.NewGuid();
            }
            if (!AjaxManager.Instance.IsCallback)
            {
                LiteralControl lit = new LiteralControl();
                lit.Text = string.Format(@"
<input type=""hidden"" value=""{0}"" name=""__VIEWSTATE_KEY"" />", _session);
                page.Form.Controls.Add(lit);
            }
        }

        /**
         * Level3: Loads Viewstate from database
         */
        public override void Load()
        {
            IPersistViewState state = Magix.Brix.Data.Adapter.Instance as IPersistViewState;
            LosFormatter formatter = new LosFormatter();
            string obj = state.Load(_session.ToString(), Page.Request.Url.ToString());
            Pair pair = formatter.Deserialize(obj) as Pair;
            ViewState = pair.First;
            ControlState = pair.Second;
        }

        /**
         * Level3: Saves Viewstate to database
         */
        public override void Save()
        {
            IPersistViewState state = Magix.Brix.Data.Adapter.Instance as IPersistViewState;
            LosFormatter formatter = new LosFormatter();
            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                formatter.Serialize(writer, new Pair(ViewState, ControlState));
            }
            state.Save(_session.ToString(), Page.Request.Url.ToString(), builder.ToString());
        }
    }
}
