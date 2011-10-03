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
     */
    [ActiveController]
    public class CreateRadioButtonForm_Controller : ActiveController
    {
        /**
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.RadioButtonForm-1")) > 0)
                    return;

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.RadioButtonForm-1";

                MetaForm.Node n_0 = new MetaForm.Node();
                n_0.Name = "c-1";

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
                    n_2.Value = "span-20";
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
                        n_3.Value = "Magix.MetaForms.Plugins.Panel";
                        n_2.Children.Add(n_3);

                        n_3 = new MetaForm.Node();
                        n_3.Name = "Properties";

                        n_2.Children.Add(n_3);
                        {
                            MetaForm.Node n_4 = new MetaForm.Node();
                            n_4.Name = "CssClass";
                            n_4.Value = "span-5";
                            n_3.Children.Add(n_4);

                            n_4 = new MetaForm.Node();
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
                                n_5.Name = "box-shadow";
                                n_5.Value = "5px 5px 5px #1E2722";
                                n_4.Children.Add(n_5);

                                n_5 = new MetaForm.Node();
                                n_5.Name = "background-image";
                                n_5.Value = "linear-gradient(#F9F9F8 0%, #DCD5D1 100%)";
                                n_4.Children.Add(n_5);

                                n_5 = new MetaForm.Node();
                                n_5.Name = "border-radius";
                                n_5.Value = "25px 25px 25px 25px ";
                                n_4.Children.Add(n_5);
                            }
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
                                n_5.Value = "Magix.MetaForms.Plugins.RadioButton";
                                n_4.Children.Add(n_5);

                                n_5 = new MetaForm.Node();
                                n_5.Name = "Properties";

                                n_4.Children.Add(n_5);
                                {
                                    MetaForm.Node n_6 = new MetaForm.Node();
                                    n_6.Name = "CssClass";
                                    n_6.Value = "span-1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "GroupName";
                                    n_6.Value = "radio_grp_1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "ID";
                                    n_6.Value = "rdo1";
                                    n_5.Children.Add(n_6);
                                }
                            }
                            n_4 = new MetaForm.Node();
                            n_4.Name = "c-1";

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
                                    n_6.Value = "span-4 last";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Tag";
                                    n_6.Value = "label";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Text";
                                    n_6.Value = "Milk";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "For";
                                    n_6.Value = "rdo1";
                                    n_5.Children.Add(n_6);
                                }
                            }
                            n_4 = new MetaForm.Node();
                            n_4.Name = "c-2";

                            n_3.Children.Add(n_4);
                            {
                                MetaForm.Node n_5 = new MetaForm.Node();
                                n_5.Name = "TypeName";
                                n_5.Value = "Magix.MetaForms.Plugins.RadioButton";
                                n_4.Children.Add(n_5);

                                n_5 = new MetaForm.Node();
                                n_5.Name = "Properties";

                                n_4.Children.Add(n_5);
                                {
                                    MetaForm.Node n_6 = new MetaForm.Node();
                                    n_6.Name = "CssClass";
                                    n_6.Value = "span-1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "GroupName";
                                    n_6.Value = "radio_grp_1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "ID";
                                    n_6.Value = "rdo2";
                                    n_5.Children.Add(n_6);
                                }
                            }
                            n_4 = new MetaForm.Node();
                            n_4.Name = "c-3";

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
                                    n_6.Value = "span-4 last";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Text";
                                    n_6.Value = "Steak";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Tag";
                                    n_6.Value = "label";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "For";
                                    n_6.Value = "rdo2";
                                    n_5.Children.Add(n_6);
                                }
                            }
                            n_4 = new MetaForm.Node();
                            n_4.Name = "c-4";

                            n_3.Children.Add(n_4);
                            {
                                MetaForm.Node n_5 = new MetaForm.Node();
                                n_5.Name = "TypeName";
                                n_5.Value = "Magix.MetaForms.Plugins.RadioButton";
                                n_4.Children.Add(n_5);

                                n_5 = new MetaForm.Node();
                                n_5.Name = "Properties";

                                n_4.Children.Add(n_5);
                                {
                                    MetaForm.Node n_6 = new MetaForm.Node();
                                    n_6.Name = "CssClass";
                                    n_6.Value = "span-1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "GroupName";
                                    n_6.Value = "radio_grp_1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "ID";
                                    n_6.Value = "rdo3";
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
                                    n_6.Value = "span-4 last";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Tag";
                                    n_6.Value = "label";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Text";
                                    n_6.Value = "Lettuce";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "For";
                                    n_6.Value = "rdo3";
                                    n_5.Children.Add(n_6);
                                }
                            }
                            n_4 = new MetaForm.Node();
                            n_4.Name = "c-7";

                            n_3.Children.Add(n_4);
                            {
                                MetaForm.Node n_5 = new MetaForm.Node();
                                n_5.Name = "TypeName";
                                n_5.Value = "Magix.MetaForms.Plugins.RadioButton";
                                n_4.Children.Add(n_5);

                                n_5 = new MetaForm.Node();
                                n_5.Name = "Properties";

                                n_4.Children.Add(n_5);
                                {
                                    MetaForm.Node n_6 = new MetaForm.Node();
                                    n_6.Name = "CssClass";
                                    n_6.Value = "span-1";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "ID";
                                    n_6.Value = "rdo4";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "GroupName";
                                    n_6.Value = "radio_grp_1";
                                    n_5.Children.Add(n_6);
                                }
                            }
                            n_4 = new MetaForm.Node();
                            n_4.Name = "c-8";

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
                                    n_6.Value = "span-4 last";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "For";
                                    n_6.Value = "rdo4";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Text";
                                    n_6.Value = "Pepsi";
                                    n_5.Children.Add(n_6);

                                    n_6 = new MetaForm.Node();
                                    n_6.Name = "Tag";
                                    n_6.Value = "label";
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
                            n_4.Value = "To the left is an example of a 'single choice RadioButton' set of widgets, which allows the user to select only 'one out of many'. Basically, if you can ask questions such as 'do you want to drive or walk'. Meaning, questions which have mutually exclusive answers, then RadioButtons are what you're looking for ...";
                            n_3.Children.Add(n_4);
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
                            n_4.Name = "CssClass";
                            n_4.Value = "span-10 push-1 prepend-top";
                            n_3.Children.Add(n_4);

                            n_4 = new MetaForm.Node();
                            n_4.Name = "Text";
                            n_4.Value = "Notice how the 'GroupName' property are the same for all the RadioButtons in the Panel. That's what makes them 'mutually exclusive choices'. If two of them had the same GroupName, and two others had the same GroupName, but different from the two first, you would become able to choose any one of the first two, AND any one of the second two, meaning questions such as; 'Soup or Salad. Plus Steak or Fish' ...";
                            n_3.Children.Add(n_4);
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
