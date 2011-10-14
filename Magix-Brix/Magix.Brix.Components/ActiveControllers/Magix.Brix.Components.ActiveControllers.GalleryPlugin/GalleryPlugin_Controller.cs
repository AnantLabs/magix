/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.IO;
using System.Web;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Gallery;
using Magix.Brix.Data;
using Magix.UX.Widgets;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Globalization;

namespace Magix.Brix.Components.ActiveControllers.GalleryPlugin
{
    /**
     * Level2: Gallery plugin for the Gallery modules. Also serves as a perfect vantage point for
     * creating your own plugins to Magix and the publishing system
     */
    [ActiveController]
    public class GalleryPlugin_Controller : ActiveController
    {
        /**
         * Level2: Handled to override the 
         * 'DBAdmin.Common.ComplexInstanceDeletedConfirmed' event
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Magix.Brix.Loader.ActiveEvents.Instance.CreateEventMapping(
                "DBAdmin.Common.ComplexInstanceDeletedConfirmed",
                "DBAdmin.Common.ComplexInstanceDeletedConfirmed-Override");

            Magix.Brix.Loader.ActiveEvents.Instance.CreateEventMapping(
                "DBAdmin.Common.ComplexInstanceDeletedConfirmed-Passover",
                "DBAdmin.Common.ComplexInstanceDeletedConfirmed");
        }

        /**
         * Level2: Loads the Administrator SlidingMenu for our GalleryPlugin injected
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Publishing"]["Items"]["Plugins"]["Caption"].Value = "Plugins";
            e.Params["Items"]["Publishing"]["Items"]["Plugins"]["Items"]["Gallery"]["Caption"].Value = "Galleries";
            e.Params["Items"]["Publishing"]["Items"]["Plugins"]["Items"]["Gallery"]["Event"]["Name"].Value = "Magix.Publishing.Gallery.EditGalleries";
        }

        /**
         * Level2: Loads up the grid for editing all galleries
         */
        [ActiveEvent(Name = "Magix.Publishing.Gallery.EditGalleries")]
        protected void Magix_Publishing_Gallery_EditGalleries(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Gallery).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 16;
            node["Last"].Value = true;
            node["CssClass"].Value = "large-bottom-margin edit-gallery";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["Folder"].Value = true;
            node["WhiteListColumns"]["Folder"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["User"].Value = true;
            node["WhiteListColumns"]["User"]["ForcedWidth"].Value = 4;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Publishing.Gallery.EditGallery";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["ReuseNode"].Value = true;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
            node["Type"]["Properties"]["Folder"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Folder"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
            node["Type"]["Properties"]["User"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["User"]["TemplateColumnEvent"].Value = "Magix.Gallery.GetUserTemplateColumn";

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);

            node = new Node();

            node["Caption"].Value = "Galleries";

            RaiseEvent(
                "Magix.Core.SetFormCaption",
                node);
        }

        /**
         * Creates a Label with the username of the Gallery and returns back to caller
         */
        [ActiveEvent(Name = "Magix.Gallery.GetUserTemplateColumn")]
        protected void Magix_Gallery_GetUserTemplateColumn(object sender, ActiveEventArgs e)
        {
            Gallery g = Gallery.SelectByID(e.Params["ID"].Get<int>());
            Label l = new Label();
            l.Text = g.User.Username;
            e.Params["Control"].Value = l;
        }

        /**
         * Level2: Edits the given 'ID' Gallery
         */
        [ActiveEvent(Name = "Magix.Publishing.Gallery.EditGallery")]
        protected void Magix_Publishing_Gallery_EditGallery(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Gallery).FullName;
            node["PropertyName"].Value = "Images";
            node["ID"].Value = e.Params["ID"].Value;
            node["IsList"].Value = true;
            node["NoIdColumn"].Value = true;
            node["AppendEvent"].Value = "Magix.Gallery.AppendButtonClicked";

            node["WhiteListColumns"]["FileName"].Value = true;
            node["WhiteListColumns"]["FileName"]["ForcedWidth"].Value = 8;

            node["Type"]["Properties"]["FileName"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["FileName"]["MaxLength"].Value = 150;

            node["Padding"].Value = 6;
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["ReUseNode"].Value = true;

            node["Container"].Value = "content4";

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);

            // Creating our 'Append Files Uploader' control ...
            CreateUploaderForAppendingFilesInEdit(e.Params);
        }

        private void CreateUploaderForAppendingFilesInEdit(Node node)
        {
            Node n = new Node();

            n["Width"].Value = 1; // Must have some arbitrary width, since otherwise element
            // won't 'float', and mess up our layout ...
            n["GalleryID"].Value = node["ID"].Value;
            n["FileUploadedEvent"].Value = "Magix.Gallery.FileWasAppendedToGallery";
            n["FreezeContainer"].Value = true;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Uploader",
                "content5",
                n);
        }

        [ActiveEvent(Name = "Magix.Publishing.Gallery.GetAllGalleries")]
        protected void Magix_Publishing_Gallery_GetAllGalleries(object sender, ActiveEventArgs e)
        {
            foreach (Gallery idx in 
                Gallery.Select(
                    Criteria.Range(0, 12, "Created", false)))
            {
                e.Params["Galleries"]["g-" + idx.ID]["Name"].Value = idx.Name;
                if (idx.User != null)
                {
                    e.Params["Galleries"]["g-" + idx.ID]["User"].Value = idx.User.Username;
                    e.Params["Galleries"]["g-" + idx.ID]["UserAvatarURL"].Value = idx.User.AvatarURL;
                }
                else
                {
                    e.Params["Galleries"]["g-" + idx.ID]["User"].Value = "[anonymous-coward]";
                    e.Params["Galleries"]["g-" + idx.ID]["UserAvatarURL"].Value = "media/images/marvin-headshot.png";
                }
                e.Params["Galleries"]["g-" + idx.ID]["Count"].Value = idx.Images.Count;
                e.Params["Galleries"]["g-" + idx.ID]["ID"].Value = idx.ID;
                e.Params["Galleries"]["g-" + idx.ID]["Created"].Value = 
                    idx.Created.ToString("ddd d. MMM yy", CultureInfo.InvariantCulture);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.Gallery.OpenGalleryInCurrentContainer")]
        protected void Magix_Publishing_Gallery_OpenGalleryInCurrentContainer(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            Gallery g = Gallery.SelectByID(id);

            OpenGallery(g, e.Params);
        }

        private void OpenGallery(Gallery g, Node node)
        {
            WebPart p = WebPart.SelectByID(node["OriginalWebPartID"].Get<int>());

            Node n = new Node();
            n["OriginalWebPartID"].Value = node["OriginalWebPartID"].Value;
            n["GalleryName"].Value = g.Name;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.PublisherImage.ImageGallery",
                p.Container.ViewportContainer,
                n);

        }

        /**
         * Level3: File was appended to gallery, saving the file locally, within
         * the folder of the Gallery, and attaching the image to the existing Gallery
         */
        [ActiveEvent(Name = "Magix.Gallery.FileWasAppendedToGallery")]
        protected void Magix_Gallery_FileWasAppendedToGallery(object sender, ActiveEventArgs e)
        {
            string fileContent = e.Params["File"].Get<string>();
            string fileName = e.Params["FileName"].Get<string>();

            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Gallery g = Gallery.SelectByID(e.Params["GalleryID"].Get<int>());

                if (g == null)
                    throw new ArgumentException("That gallery doesn't exist");

                string folder = g.Folder;

                string fullPath = Page.MapPath("~/" + folder + fileName);

                byte[] rawContent = Convert.FromBase64String(fileContent);

                using (FileStream stream = File.Create(fullPath))
                {
                    stream.Write(rawContent, 0, rawContent.Length);
                }

                Gallery.Image i = new Gallery.Image();
                i.FileName = folder + fileName;
                g.Images.Add(i);
                g.Save();

                tr.Commit();

                Node x = new Node();
                x["Message"].Value = string.Format(@"
Image was uploaded as '{0}' and attached to Gallery '{1}'",
                    i.FileName,
                    g.Name);

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    x);

                x = new Node();
                x["FullTypeName"].Value = 
                    typeof(Gallery).FullName + "|" + 
                    typeof(Gallery.Image).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    x);
            }
        }

        [ActiveEvent(Name = "Magix.Gallery.AppendButtonClicked")]
        protected void Magix_Gallery_AppendButtonClicked(object sender, ActiveEventArgs e)
        {
            Gallery g = Gallery.SelectByID(e.Params["ParentID"].Get<int>());

            string clientImageFolder = g.Folder ?? "";

            Node node = new Node();

            node["Top"].Value = 2;
            node["IsSelect"].Value = true;
            node["Push"].Value = 1;
            node["Width"].Value = 20;
            node["Folder"].Value = clientImageFolder;
            node["Filter"].Value = "*.png;*.gif;*.jpg;*.jpeg;";
            node["SelectEvent"].Value = "Magix.Publishing.AppendImageToGallery";
            node["SelectEvent"]["Params"]["GalleryID"].Value = e.Params["ParentID"].Value;
            node["Seed"].Value = e.Params["ID"].Value;

            RaiseEvent(
                "Magix.FileExplorer.LaunchExplorer",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.AppendImageToGallery")]
        protected void Magix_Publishing_AppendImageToGallery(object sender, ActiveEventArgs e)
        {
            Gallery g = Gallery.SelectByID(e.Params["Params"]["GalleryID"].Get<int>());

            string fileName = e.Params["Folder"].Get<string>() + e.Params["FileName"].Get<string>();
            Gallery.Image i = new Gallery.Image();
            i.FileName = fileName;
            g.Images.Add(i);

            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                g.Save();

                tr.Commit();
            }

            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /**
         * Level3: Will return all images from the given 'GalleryName' Gallery as 
         * 'Images'
         */
        [ActiveEvent(Name = "Magix.Gallery.GetGalleryData")]
        protected void Magix_Gallery_GetGalleryData(object sender, ActiveEventArgs e)
        {
            Gallery g = Gallery.SelectFirst(
                Criteria.Eq(
                    "Name", 
                    e.Params["GalleryName"].Get<string>()));

            if (g == null)
                throw new ArgumentException(
                    "Sorry mate, but the '" + 
                    e.Params["GallerName"].Get<string>() + 
                    "' Gallery doesn't exist ...");

            int idxNo = 0;
            foreach (Gallery.Image idx in g.Images)
            {
                e.Params["Images"]["i-" + idxNo].Value = idx.FileName;
                idxNo += 1;
            }
        }

        /**
         * Level2: Will create a Gallery component from a list of images ['Files'] within
         * the given 'Folder' and return its id as 'ID'
         */
        [ActiveEvent(Name = "Magix.Common.CreateGallery")]
        protected void Magix_Common_CreateGallery(object sender, ActiveEventArgs e)
        {
            string folder = e.Params["Folder"].Get<string>();

            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Gallery g = new Gallery();
                g.Folder = folder;

                foreach (Node idx in e.Params["Files"])
                {
                    string fileName = folder + idx.Get<string>();
                    Gallery.Image i = new Gallery.Image();
                    i.FileName = fileName;
                    g.Images.Add(i);
                }
                g.Save();

                tr.Commit();
            }
        }

        // TODO: Get Generic SelectList Control
        /**
         * Level3: Will return a SelectList with all the Galleries back to caller
         */
        [ActiveEvent(Name = "Magix.Gallery.GetTemplateColumnSelectGallery")]
        protected void Magix_Gallery_GetTemplateColumnSelectGallery(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["WebPartID"].Value = e.Params["WebPartID"].Value;
                    tx["Value"].Value = ls.SelectedItem.Value;

                    RaiseEvent(
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };

            ListItem item = new ListItem("Select a Gallery ...", "");
            ls.Items.Add(item);

            foreach (Gallery idx in Gallery.Select(Criteria.Sort("Created", false)))
            {
                ListItem i = new ListItem(
                    idx.Name + " - [" + idx.Images.Count + " img]", 
                    idx.Name);

                if (idx.Name == e.Params["Value"].Get<string>())
                    i.Selected = true;

                ls.Items.Add(i);
            }
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Overridden to clear from content 4 and out ...
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed-Override")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed_Override(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(Gallery).FullName)
            {
                // We have to clear number 4 and out, before the object is deleted ...
                ActiveEvents.Instance.RaiseClearControls("content4");

                RaiseEvent(
                    "DBAdmin.Common.ComplexInstanceDeletedConfirmed-Passover",
                    e.Params);
            }
            else
            {
                RaiseEvent(
                    "DBAdmin.Common.ComplexInstanceDeletedConfirmed-Passover",
                    e.Params);
            }
        }
    }
}
