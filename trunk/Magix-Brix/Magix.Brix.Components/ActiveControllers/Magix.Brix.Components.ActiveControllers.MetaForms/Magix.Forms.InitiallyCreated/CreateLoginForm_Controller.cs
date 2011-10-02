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
     * Level2: Will upon startup create a Login Form which you can use as a 'starter kit' for your 
     * own logic, or directly consume as is, as a User Control
     */
    [ActiveController]
    public class CreateLoginForm_Controller : ActiveController
    {
        /**
         * Level2: Overridden to actually create our Form
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.GreenLogin-Local")) > 0)
                    return; // Don't want duals here ...

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.GreenLogin-Local";

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
                        n_2.Name = "Style";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "padding-left";
                            n_3.Value = "40px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "padding-top";
                            n_3.Value = "36px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "padding-right";
                            n_3.Value = "40px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "padding-bottom";
                            n_3.Value = "36px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "float";
                            n_3.Value = "left";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "display";
                            n_3.Value = "block";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "box-shadow";
                            n_3.Value = "5px 5px 2px #2F3E31";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-image";
                            n_3.Value = "linear-gradient(#F7F9F7 0%, #ACBEA8 100%)";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "border-radius";
                            n_3.Value = "0px 100px 0px 100px ";
                            n_2.Children.Add(n_3);
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "CssClass";
                        n_2.Value = "";
                        n_1.Children.Add(n_2);

                        n_2 = new MetaForm.Node();
                        n_2.Name = "DefaultWidget";
                        n_2.Value = "submit";
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
                            n_3.Value = "Magix.MetaForms.Plugins.Label";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-8 last";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "h2";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Please Login ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-5";

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
                                n_4.Value = "span-5 last clear-both";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "label";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Username";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-1";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.TextBox";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-8 last clear-both";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AutoCapitalize";
                                n_4.Value = "False";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AutoCorrect";
                                n_4.Value = "False";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AutoComplete";
                                n_4.Value = "False";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "MaxLength";
                                n_4.Value = "50";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "PlaceHolder";
                                n_4.Value = "Username ...";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AccessKey";
                                n_4.Value = "U";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Info";
                                n_4.Value = "Username";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "font-size";
                                    n_5.Value = "24px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "height";
                                    n_5.Value = "34px";
                                    n_4.Children.Add(n_5);
                                }
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-2";

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
                                n_4.Name = "Text";
                                n_4.Value = "Password";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-5 last clear-both prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "label";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-3";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.TextBox";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-8 last clear-both";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AutoCapitalize";
                                n_4.Value = "False";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AutoCorrect";
                                n_4.Value = "False";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AutoComplete";
                                n_4.Value = "False";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "MaxLength";
                                n_4.Value = "50";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "TextMode";
                                n_4.Value = "Password";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AccessKey";
                                n_4.Value = "P";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Info";
                                n_4.Value = "Password";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "font-size";
                                    n_5.Value = "24px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "height";
                                    n_5.Value = "34px";
                                    n_4.Children.Add(n_5);
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "PlaceHolder";
                                n_4.Value = "Password ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-4";

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
                                n_4.Name = "CssClass";
                                n_4.Value = "span-4 last clear-both push-4 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Login!";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "AccessKey";
                                n_4.Value = "L";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "ID";
                                n_4.Value = "submit";
                                n_3.Children.Add(n_4);
                            }
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Actions";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Click";
                                n_4.Value = "Magix.DynamicEvent.CreateNodeFromMetaForm|Magix.DynamicEvent.LogInUser";
                                n_3.Children.Add(n_4);
                            }
                        }
                    }
                }

                f.Form["Surface"].Children.Add(n_0);

                f.Save();

                tr.Commit();
            }
        }
    }
}
