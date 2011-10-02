/*
* Magix - A Web Application Framework for Humans
* Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
* Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
*/

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.MetaForms;

namespace Magix.Brix.Components.ActiveControllers.MetaForms
{
    /**
     * Level2: Will upon startup create an Image Button which 'flashes' which you can use as 
     * a 'starter kit' for your own logic, or directly consume as is, as a User Control
     */
    [ActiveController]
    public class CreateFlashingImageButton_Controller : ActiveController
    {
        /**
         * Level2: Overridden to actually create our Form
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.FlashingImageButton")) > 0)
                    return;

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.FlashingImageButton";

                MetaForm.Node n_0 = new MetaForm.Node();
                n_0.Name = "c-0";

                {
                    MetaForm.Node n_1 = new MetaForm.Node();
                    n_1.Name = "TypeName";
                    n_1.Value = "Magix.MetaForms.Plugins.Button";
                    n_0.Children.Add(n_1);

                    n_1 = new MetaForm.Node();
                    n_1.Name = "Properties";

                    n_0.Children.Add(n_1);
                    {
                        MetaForm.Node n_2 = new MetaForm.Node();
                        n_2.Name = "Text";
                        n_2.Value = " ";
                        n_1.Children.Add(n_2);

                        n_2 = new MetaForm.Node();
                        n_2.Name = "CssClass";
                        n_2.Value = " flashlight-3";
                        n_1.Children.Add(n_2);

                        n_2 = new MetaForm.Node();
                        n_2.Name = "Style";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "float";
                            n_3.Value = "left";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "display";
                            n_3.Value = "block";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "width";
                            n_3.Value = "200px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "height";
                            n_3.Value = "200px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-image";
                            n_3.Value = "url(media/images/magix-logo.png)";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-attachment";
                            n_3.Value = "scroll";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-position";
                            n_3.Value = "0 0";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-repeat";
                            n_3.Value = "repeat";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-color";
                            n_3.Value = "";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "box-shadow";
                            n_3.Value = "3px 3px 5px #00300B";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "border-radius";
                            n_3.Value = "100px 100px 100px 100px ";
                            n_2.Children.Add(n_3);
                        }
                    }
                }

                f.Form["Surface"].Children.Add(n_0);

                // This actually saves your Object
                f.Save();

                // Unless this line of code is executed, the whole Data Serialization Job is 'rolled back' and discarded ...
                tr.Commit();
            }
        }
    }
}
