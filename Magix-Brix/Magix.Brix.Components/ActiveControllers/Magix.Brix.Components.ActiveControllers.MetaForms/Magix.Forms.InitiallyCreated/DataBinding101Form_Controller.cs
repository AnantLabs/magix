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
    public class DataBinding101_Controller : ActiveController
    {
        /**
         * Level2: Overridden to actually create our Form
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.DataBinding101")) > 0)
                    return;

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.DataBinding101";

                MetaForm.Node n_0 = new MetaForm.Node();
                n_0.Name = "c-1";

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
                            n_3.Value = "Magix.MetaForms.Plugins.Label";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "WebPage ID:";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-3";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "label";
                                n_3.Children.Add(n_4);
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
                                n_4.Value = "span-5";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "{DataSource[OriginalWebPartID].Value}";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "label";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "font-family";
                                    n_5.Value = "Arial Black";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "font-style";
                                    n_5.Value = "italic";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "font-size";
                                    n_5.Value = "24px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "height";
                                    n_5.Value = "36px";
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
                                n_4.Name = "CssClass";
                                n_4.Value = "span-10 push-2 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Above is a DataBound Label, which has its Text Property DataBound towards the DataSource[OriginalWebPartID].Value ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-3";

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
                                n_4.Value = "span-10 prepend-top push-2";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Notice especially the Expression Parentheses ( These guys; {} ), which tells the Magix 'run time' that this is a Data Bound Expression. To DataBind a Property, the property needs to start with a '{' and end with a '}'. And that specific Node needs to exist in the DataSource somehow BEFORE you Raise your DataBind Action. If you turned ON Debugging for instance, while loading any WebPage which contains this Form, you'd be able to see exactly how the DataSource would look like when the 'LoadModule' AtiveEvent is handled, if you copy that event. Then you will see that the LoadModule Active Event contains a Node called 'OriginalWebPartID'. All of these nodes which are being sent into the 'LoadModule' basically 'becomes' the DataSource property. Meaning when you say; 'DataSource[OriginalWebPartID].Value', you're telling Magix that you'd like to display the 'Value' of the 'Node' that was passed into the 'LoadModule' method as its 'Text' Property, using an 'Expression' [these guys; {}]";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-4";

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
                                n_4.Value = "span-10 push-2 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "You must Raise the Action; 'DataBindForm' on the Form you are using 'Data Binding Expressions', since otherwise the form won't be DataBinded, and it'll just show its default Text of 'Label'. This exact same technique is also use for DataBinding lists of items and such, but that's 'another' sample form ... ;)";
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
                                n_4.Value = "span-10 push-2 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "You can DataBind any property you wish, and you can databind your properties towards either 'Value' or 'Name' of your Nodes in your DataSource. There are also tons of 'fetcher Actions' in Magix, which you can use to 'populate your DataSource' one way or the other.";
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
