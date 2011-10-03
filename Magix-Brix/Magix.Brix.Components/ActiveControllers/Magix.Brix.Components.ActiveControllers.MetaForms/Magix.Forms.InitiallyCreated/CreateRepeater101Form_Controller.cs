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
    public class Repeaters101_Controller : ActiveController
    {
        /**
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.Repeaters101")) > 0)
                    return;

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.Repeaters101";

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
                        n_2.Value = "span-16";
                        n_1.Children.Add(n_2);

                        n_2 = new MetaForm.Node();
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
                            n_3.Name = "box-shadow";
                            n_3.Value = "5px 5px 5px #64614A";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "background-image";
                            n_3.Value = "linear-gradient(#FAF9F8 0%, #D8D5CC 100%)";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "border-radius";
                            n_3.Value = "25px 25px 25px 25px ";
                            n_2.Children.Add(n_3);
                        }
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
                                n_4.Name = "Tag";
                                n_4.Value = "h1";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-16";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Repeater Example ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-1";

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
                                n_4.Value = "span-15 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "margin-bottom";
                                    n_5.Value = "36px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-left";
                                    n_5.Value = "20px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-top";
                                    n_5.Value = "18px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-right";
                                    n_5.Value = "20px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "padding-bottom";
                                    n_5.Value = "18px";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "background-image";
                                    n_5.Value = "linear-gradient(#FEFFFE 0%, #D3D4D4 100%)";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "border-radius";
                                    n_5.Value = "25px 25px 25px 25px ";
                                    n_4.Children.Add(n_5);
                                }
                                n_4 = new MetaForm.Node();
                                n_4.Name = "Info";
                                n_4.Value = "{DataSource[Objects]}";
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
                                        n_6.Value = "span-15";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Tag";
                                        n_6.Value = "h2";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Text";
                                        n_6.Value = "{[Properties][Green][Value].Value}";
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
                                        n_6.Name = "Text";
                                        n_6.Value = "ID of Meta Object:";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "CssClass";
                                        n_6.Value = "span-3";
                                        n_5.Children.Add(n_6);
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
                                        n_6.Value = "{[ID].Value}";
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
                                        n_6.Value = "span-15";
                                        n_5.Children.Add(n_6);

                                        n_6 = new MetaForm.Node();
                                        n_6.Name = "Style";

                                        n_5.Children.Add(n_6);
                                        {
                                            MetaForm.Node n_7 = new MetaForm.Node();
                                            n_7.Name = "margin-top";
                                            n_7.Value = "36px";
                                            n_6.Children.Add(n_7);
                                        }
                                    }
                                }
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-2";

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
                                n_4.Value = "span-4 push-4";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "<<";
                                n_3.Children.Add(n_4);
                            }
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Actions";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Click";
                                n_4.Value = "Magix.DynamicEvent.GetPreviousMetaObjects|Debug-Copy-Magix.MetaForms.DataBindForm";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-3";

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
                                n_4.Value = "span-4";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = ">>";
                                n_3.Children.Add(n_4);
                            }
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Actions";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Click";
                                n_4.Value = "Magix.DynamicEvent.GetNextMetaObjects|Debug-Copy-Magix.MetaForms.DataBindForm";
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
                                n_4.Value = "span-16 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Above is a  Repeater [Grey] which is Data Bound towards all objects og type 'Eco' in your Meta Object Storage. The arrow buttons encapsulates how to page back and forward in your Repeaters.";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-13";

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
                                n_4.Name = "Tag";
                                n_4.Value = "label";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "This specific Repeater is dependent upon that you run these Actions in consecutive order as 'Form Init Actions'; 'Magix.DynamicEvent.GetMetaObjects' and 'Magix.DynamicEvent.DataBindForm'";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-13 push-1";
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
                                n_4.Value = "span-15 prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "The most crucial thing about a Repeater is that its 'Info' property needs to be pointing towards a 'list of nodes' in your DataSource Meta Form property. Repeater is one of very few times you CANNOT 'DataBind' towards neither the 'Value' nor the 'Name' of your Nodes, but they always should end with ']}' in fact, to signify that you want to DataBind towards the list of items that is within whatever your Expression is traversing.";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-6";

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
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Then another crucial thing about Repeaters is that the Widgets you have inside of them must be Data Bound 'relatively'. In the DataBinding101 form, we basically were DataBinding towards a 'static path' in your DataSource, but this time our Repeater's DataSource is returning a Node List, and we are supposed to DataBind towards that _specific_ item, and not some 'absolute path within your tree'. Which means that the DataBinding Expression of whatever property you're DataBinding must start with '{[', instead of '{DataSource['. This to signify that you want to DataBind towards the 'current list item' within the Repeater's Collection of Nodes, which it is being DataBound towards.";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-16";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-7";

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
                                n_4.Value = "If you had turned on Debugging you would see that the Node structure we're DataBinding towards would look roughly like this; 'Objects' containing two items; 'o-XX' and 'o-YY'. These again having 'ID', 'TypeName', 'Created' and 'Properties'. 'Properties' are the ones that contains our actual 'Meta Object Property Values'. Inside of these you'll find 'Green' nodes, which again contains 'ID', 'Name' and 'Value'. The Value's 'Value' is what we're DataBinding our Header towards for instance.";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-8";

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
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "So our Repeater's DataSource becomes; '{DataSource[Objects]}', which will return a Node List [important! Node lists ALWAYS ends with ']}' and no '.Value' or '.Name' !!], while the items inside of the Repeater will be DataBound each and every one of them against one item beneath that level ['o-XX' or 'o-YY' that is], meaning to gain access to the 'Green' property value of the 'current item' you'll need to go 'from o-XX' and into; '{[Properties][Green][Value].Value}', because that's where the string we're looking for is; 'Superb!' and 'Good', which are shown by default by our Repeater is just copied and pasted into a WebForm which has two Init Actions executed; 'Magix.DynamicEvent.GetMetaObjects' and 'Magix.DynamicEvent.DataBindForm' ...";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-9";

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
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Below you can see a complete screenshot of how the DataSource would look like using the 'GetMetaObjects' Action, other Actions that populates your DataSource might produce other types of DataSource structures, but this too can be figured out visually, like below, by 'Clicking' the Action you wonder about which types of parameters are being passed into while having turned 'Debugging' on ...";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-10";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.Image";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "AlternateText";
                                n_4.Value = "Screenshot of how Actions looks lika after you've Copied the 'DataBind' Action to look at actually what gets 'passed into it' ...";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "ImageUrl";
                                n_4.Value = "media/images/debugged-node-list-action.png";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = " ";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-11";

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
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Above you can see how the node 'Objects' contains two items, o-71 and o-73, which basically becomes our 'Two Initial Items' in our Repeater. while [Properties][Green][Value].Value, inside of these 'o-71' and 'o-73' contains the actual text we're looking for to DataBind our h2 Label against; 'SUPERB!' and 'Good' ...";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15 prepend-top";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-12";

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
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Now, of course, other Actions might produce different DataSource structures for you, which means you'll need to DataBind towards DIFFERENT 'structures' than what you're looking at here. But if you read the above text about three times, once partially loud for yourself, while you look at the image, check out the properties of the Repeater and its 'children Widgets', and look at how the Result looks like, if you attach this Form to a Web Page and look at it in the front-web, you just might actually 'get it' faster than what you might currently think ... - Because really, it's actually quite easy, once you just get it I guess ... Once you do get it, feel free to 'improve' on this text, such that the next guy or gal could 'get it faster' ... ;)";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15";
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
