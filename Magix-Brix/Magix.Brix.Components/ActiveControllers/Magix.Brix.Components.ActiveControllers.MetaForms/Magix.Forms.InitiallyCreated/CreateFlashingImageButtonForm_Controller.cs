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
                    n_1.Value = "Magix.MetaForms.Plugins.Panel";
                    n_0.Children.Add(n_1);

                    n_1 = new MetaForm.Node();
                    n_1.Name = "Properties";

                    n_0.Children.Add(n_1);
                    {
                        MetaForm.Node n_2 = new MetaForm.Node();
                        n_2.Name = "CssClass";
                        n_2.Value = "span-22";
                        n_1.Children.Add(n_2);
                    }
                    n_1 = new MetaForm.Node();
                    n_1.Name = "Surface";

                    n_0.Children.Add(n_1);
                    {
                        MetaForm.Node n_2 = new MetaForm.Node();
                        n_2.Name = "c-0";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.Button";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = " ";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = " flashlight-3";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "float";
                                    n_5.Value = "left";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "display";
                                    n_5.Value = "block";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "width";
                                    n_5.Value = "200px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "height";
                                    n_5.Value = "200px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-image";
                                    n_5.Value = "url(media/images/magix-logo.png)";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-attachment";
                                    n_5.Value = "scroll";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-position";
                                    n_5.Value = "0 0";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-repeat";
                                    n_5.Value = "repeat";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-color";
                                    n_5.Value = "";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "box-shadow";
                                    n_5.Value = "3px 3px 5px #00300B";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "border-radius";
                                    n_5.Value = "100px 100px 100px 100px ";
                                    n_4.Children.Add(n_5);
                                }
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-1";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.Label";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-10 push-1";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Basically a funny functioning 'Button' with animation and tons of 'Bling'. Mostly here to serve as an example of what you can accomplish, exclusively using the 'Style Builder'. Meaning the 'Builder' button in the Properties & Actions window ...";
                                n_3.Children.Add(n_4);
                            }
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
