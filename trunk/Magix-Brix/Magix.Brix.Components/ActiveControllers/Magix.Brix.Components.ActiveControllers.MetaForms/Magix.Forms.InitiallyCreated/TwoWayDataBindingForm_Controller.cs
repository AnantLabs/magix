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
    public class TwoWayDataBindingForm_Controller : ActiveController
    {
        /**
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.2-Way-DataBinding")) > 0)
                    return; // Don't want duals here ...

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.2-Way-DataBinding";

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
                        n_2.Value = "span-18 last";
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
                            n_3.Value = "Magix.MetaForms.Plugins.Repeater";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-16";
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
                                    n_5.Name = "float";
                                    n_5.Value = "left";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "clear";
                                    n_5.Value = "both";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "display";
                                    n_5.Value = "block";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "box-shadow";
                                    n_5.Value = "2px 2px 3px #000000";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-image";
                                    n_5.Value = "linear-gradient(#FBF9F9 0%, #D4C6C6 100%)";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "border-radius";
                                    n_5.Value = "10px 10px 10px 10px ";
                                    n_4.Children.Add(n_5);
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "Info";
                                n_4.Value = "{DataSource[Objects]}";
                                n_3.Children.Add(n_4);
                            }
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Actions";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "InitiallyLoaded";
                                n_4.Value = "Magix.Demo.GetRatingObjects|Magix.DynamicEvent.DataBindForm";
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
                                        n_6.Value = "span-14";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Tag";
                                        n_6.Value = "h2";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "{[Properties][FullName][Value].Value}";
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
                                        n_6.Value = "span-2 last";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "{[Properties][PetName][Value].Value}";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Style";

                                        n_5.Children.Add(n_6);
                                        {
                                            MetaForm.Node n_7 = new MetaForm.Node();
                                            n_7.Name = "font-style";
                                            n_7.Value = "italic";
                                            n_6.Children.Add(n_7);

                                            n_7 = new MetaForm.Node();
                                            n_7.Name = "color";
                                            n_7.Value = "#A1A1A1";
                                            n_6.Children.Add(n_7);
                                        }
                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "ToolTip";
                                        n_6.Value = "The name of the Person's Pet ...";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-2";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.Stars";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-11";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Value";
                                        n_6.Value = "{[Properties][Rating][Value].Value}";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Info";
                                        n_6.Value = "Rating";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Enabled";
                                        n_6.Value = "False";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "ToolTip";
                                        n_6.Value = "Lovin for Magix ...";
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
                                        n_6.Value = "span-9 clear-both prepend-top";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Style";

                                        n_5.Children.Add(n_6);
                                        {
                                            MetaForm.Node n_7 = new MetaForm.Node();
                                            n_7.Name = "font-style";
                                            n_7.Value = "italic";
                                            n_6.Children.Add(n_7);

                                            n_7 = new MetaForm.Node();
                                            n_7.Name = "color";
                                            n_7.Value = "#A3A2A3";
                                            n_6.Children.Add(n_7);
                                        }
                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "Amount of Love this person has for Magix is shown above ...";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-4";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.LinkButton";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-3 clear-both push-13 last";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "Edit ...";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "ToolTip";
                                        n_6.Value = "Allows you to Edit the Rating ...";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Info";
                                        n_6.Value = "{[ID].Value}";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Style";

                                        n_5.Children.Add(n_6);
                                        {
                                            MetaForm.Node n_7 = new MetaForm.Node();
                                            n_7.Name = "font-family";
                                            n_7.Value = "Comic Sans MS";
                                            n_6.Children.Add(n_7);

                                            n_7 = new MetaForm.Node();
                                            n_7.Name = "font-size";
                                            n_7.Value = "30px";
                                            n_6.Children.Add(n_7);

                                            n_7 = new MetaForm.Node();
                                            n_7.Name = "height";
                                            n_7.Value = "36px";
                                            n_6.Children.Add(n_7);
                                        }
                                    }
                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Actions";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "Click";
                                        n_6.Value = "Magix.DynamicEvent.Transform|Magix.DynamicEvent.LoadMetaForm";
                                        n_5.Children.Add(n_6);
                                    }
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "c-3";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "TypeName";
                                    n_5.Value = "Magix.MetaForms.Plugins.Ruler";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "Properties";

                                    n_4.Children.Add(n_5);
                                    {
                                        MetaForm.Node n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-16 last down-1 bottom-2";
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
                                n_4.Value = "span-12  last prepend-top push-1";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "The above Repeater is DataBound towards your 'Magix.Demo.Rating' Meta Objects. By default there are none of these types of objects in your Magix installation, however you can create as many as you wish using the 'Data Collection' example ...";
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
