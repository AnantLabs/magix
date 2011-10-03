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
                    n_1.Name = "Surface";

                    n_0.Children.Add(n_1);
                    {
                        MetaForm.Node n_2 = new MetaForm.Node();
                        n_2.Name = "c-0";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.Panel";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-left";
                                    n_5.Value = "40px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-top";
                                    n_5.Value = "36px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-right";
                                    n_5.Value = "40px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-bottom";
                                    n_5.Value = "36px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "float";
                                    n_5.Value = "left";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "display";
                                    n_5.Value = "block";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "box-shadow";
                                    n_5.Value = "5px 5px 2px #2F3E31";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-image";
                                    n_5.Value = "linear-gradient(#F7F9F7 0%, #ACBEA8 100%)";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "border-radius";
                                    n_5.Value = "0px 100px 0px 100px ";
                                    n_4.Children.Add(n_5);
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "DefaultWidget";
                                n_4.Value = "submit";
                                n_3.Children.Add(n_4);
                            }
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Surface";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "c-0";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.Label";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-8 last";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Tag";
                                        n_6.Value = "h2";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "Please Login ...";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-5";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.Label";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-5 last clear-both";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Tag";
                                        n_6.Value = "label";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "Username";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-1";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.TextBox";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-8 last clear-both";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AutoCapitalize";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AutoCorrect";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AutoComplete";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "MaxLength";
                                        n_6.Value = "50";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "PlaceHolder";
                                        n_6.Value = "Username ...";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AccessKey";
                                        n_6.Value = "U";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Info";
                                        n_6.Value = "Username";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Style";

                                        n_5.Children.Add(n_6);
                                        {
                                            MetaForm.Node n_7 = new MetaForm.Node();
                                            n_7.Name = "font-size";
                                            n_7.Value = "24px";
                                            n_6.Children.Add(n_7);

                                            n_7 = new MetaForm.Node();
                                            n_7.Name = "height";
                                            n_7.Value = "34px";
                                            n_6.Children.Add(n_7);
                                        }
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-2";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.Label";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "Password";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-5 last clear-both prepend-top";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Tag";
                                        n_6.Value = "label";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-3";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.TextBox";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-8 last clear-both";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AutoCapitalize";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AutoCorrect";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AutoComplete";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "MaxLength";
                                        n_6.Value = "50";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "TextMode";
                                        n_6.Value = "Password";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AccessKey";
                                        n_6.Value = "P";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Info";
                                        n_6.Value = "Password";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Style";

                                        n_5.Children.Add(n_6);
                                        {
                                            MetaForm.Node n_7 = new MetaForm.Node();
                                            n_7.Name = "font-size";
                                            n_7.Value = "24px";
                                            n_6.Children.Add(n_7);

                                            n_7 = new MetaForm.Node();
                                            n_7.Name = "height";
                                            n_7.Value = "34px";
                                            n_6.Children.Add(n_7);
                                        }
                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "PlaceHolder";
                                        n_6.Value = "Password ...";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-4";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.Button";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-4 last clear-both push-4 prepend-top";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "Login!";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "AccessKey";
                                        n_6.Value = "L";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "ID";
                                        n_6.Value = "submit";
                                        n_5.Children.Add(n_6);
                                    }
                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Actions";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "Click";
                                        n_6.Value = "Magix.DynamicEvent.CreateNodeFromMetaForm|Magix.DynamicEvent.LogInUser";
                                        n_5.Children.Add(n_6);
                                    }
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
                                n_4.Value = "Basically a perfectly, finished working example of an entire Login Form, complete with Actions and everything needed to become a drop in login form in your own Meta Forms ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                    }
                    n_1 = new MetaForm.Node();
                    n_1.Name = "Properties";

                    n_0.Children.Add(n_1);
                    {
                        MetaForm.Node n_2 = new MetaForm.Node();
                        n_2.Name = "CssClass";
                        n_2.Value = "span-22";
                        n_1.Children.Add(n_2);
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
