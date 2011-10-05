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
    public class CreateHelloWorldForm_Controller : ActiveController
    {
        /**
         * Level2: Overridden to actually create our Form
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (MetaForm.CountWhere(Criteria.Eq("Name", "Magix.Forms.HelloWorld")) > 0)
                    return; // Don't want duals here ...

                MetaForm f = new MetaForm();
                f.Name = "Magix.Forms.HelloWorld";

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
                        n_2.Value = "span-16 prepend-1 append-1 mux-rounded mux-shaded-orange";
                        n_1.Children.Add(n_2);

                        n_2 = new MetaForm.Node();
                        n_2.Name = "Style";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "padding-top";
                            n_3.Value = "36px";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "padding-bottom";
                            n_3.Value = "36px";
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
                                n_4.Value = "prepend-top flashlight-3";
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
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Actions";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Click";
                                n_4.Value = "Magix.DynamicEvent.PlaySound";
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
                                n_4.Value = "push-1 span-8 prepend-top last";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Try clicking the Eye of Horus to the Left ...";
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
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "push-1 span-8 last";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "What just happened was that you clicked a Button - [Yes, the Eye of Horus to the left of me is a Button]";
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
                                n_4.Value = "push-1 span-5 height-6";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Pause Music!";
                                n_3.Children.Add(n_4);
                            }
                            n_3 = new MetaForm.Node();
                            n_3.Name = "Actions";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "Click";
                                n_4.Value = "Magix.DynamicEvent.PauseSound";
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
                                n_4.Value = "span-15 clear-both prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "The Eye of Horus Button is subscribing to the 'Click' Event, at which raised, it will itself raise the 'PlaySound' Action, as you can see from the screenshots below ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-5";

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
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "ImageUrl";
                                n_4.Value = "media/images/action-meta-form-hello-world.png";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Style";

                                n_3.Children.Add(n_4);
                                {
                                    MetaForm.Node n_5 = new MetaForm.Node();
                                    n_5.Name = "box-shadow";
                                    n_5.Value = "5px 5px 5px #000000";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "border-radius";
                                    n_5.Value = "15px 15px 15px 15px ";
                                    n_4.Children.Add(n_5);
                                }
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
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15 last prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "The above Image is a screenshot of how it would look like roughly if you edited this Hello World Form in the Back-Web. Click the link below to go to the back web to check out your Forms to have a look at this specific form to see how it is created, while you at the same time keep this window open in another Browser Tab ...";
                                n_3.Children.Add(n_4);
                            }
                        }
                        n_2 = new MetaForm.Node();
                        n_2.Name = "c-9";

                        n_1.Children.Add(n_2);
                        {
                            MetaForm.Node n_3 = new MetaForm.Node();
                            n_3.Name = "TypeName";
                            n_3.Value = "Magix.MetaForms.Plugins.HyperLink";
                            n_2.Children.Add(n_3);

                            n_3 = new MetaForm.Node();
                            n_3.Name = "Properties";

                            n_2.Children.Add(n_3);
                            {
                                MetaForm.Node n_4 = new MetaForm.Node();
                                n_4.Name = "CssClass";
                                n_4.Value = "span-11 push-2";
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
                                    n_5.Name = "clear";
                                    n_5.Value = "both";
                                    n_4.Children.Add(n_5);

                                    n_5 = new MetaForm.Node();
                                    n_5.Name = "display";
                                    n_5.Value = "block";
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
                                n_4 = new MetaForm.Node();
                                n_4.Name = "URL";
                                n_4.Value = "?dashboard=true";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Magix Dashboard!";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Target";
                                n_4.Value = "_empty";
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
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15 last prepend-top";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "Click the Above Link to Open the Administrator Dashboard in a new window, then click 'Forms' and then find the HelloWorld form, and edit it to see what goes on for yourself. PS! The default administrator password is admin/admin ...";
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
                                n_4.Name = "CssClass";
                                n_4.Value = "span-15 last";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Tag";
                                n_4.Value = "p";
                                n_3.Children.Add(n_4);

                                n_4 = new MetaForm.Node();
                                n_4.Name = "Text";
                                n_4.Value = "The other Button, the one that says 'Pause Sound!' has also an 'Action' associated with it. This is the 'Pause Sound Action'. This way virtually any type of logic you wish, can be associated with any type of UI you wish, and your UIs and Logic is 100% separated, and know nothing about each other. And instead of working in terms of 'Objects' and 'Types', you work in terms of 'Actions' in regards to Logic, and 100% WYSIWYG when it comes to UI. WYSIWYG means What You See Is What You Get. And UI means User Interface. And your UI is 100% separated from your Logic. And if you have to dive into C#, which is probably highly unlikely for most of your 'common problems' such as Grids, Forms, etc. Though if you have to dive into code, you have a brilliant OO Framework, with all the traditional OO mechanisms, such as DDD, O/RM, Strongly Typing, etc, etc, etc to work from. Though you can create your code as long as you obey by the 'One Action Rule' such that your logic becomes 're-usable' through-out entirely different projects ...";
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
