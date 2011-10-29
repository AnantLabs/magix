/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using System.Globalization;
using Magix.Brix.Components.ActiveTypes.MetaViews;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    /**
     * Level2: Contains Application Startup code to create the defaul Actions unless they're already there
     */
    [ActiveController]
    public class CreateDefaultInitialActions_Controller : ActiveController
    {
        #region [ -- Application Startup. Creation of default, 'built-in' Actions ... -- ]

        /**
         * Level2: Will create some default installation Actions for the End User to consume in his own
         * Meta Applications. These are all prefixed with 'Magix.DynamicEvent'
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Settings.Instance.Set("DBAdmin.MaxItemsToShow-" + typeof(Action).FullName, 8);

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SaveActiveForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.SaveActiveForm";
                    a.EventName = "Magix.MetaView.CreateSingleViewMetaObject";
                    a.Description = @"Will save the currently active Single-View Form.
Will determine which form raised the event originally, and explicitly save the field values
from that Form into a new Meta Object with the TypeName from the View ...";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ReInitializeOverriddenActions")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ReInitializeOverriddenActions";
                    a.EventName = "Magix.MetaAction.ReInitializeOverriddenActions";
                    a.Description = @"Will Re-Initialize the Overridden Actions. Basically 
destroy all existing overrides, that comes from Actions, and re-run the overriding 
Initialization of whatever new values exists. By waiting with the remapping until you're done with 
your entire change of logic, you can rewire as many Actions as you wish, create new, and such. And when 
you're done, you can rewire all your Actions in one Swoop";
                    a.StripInput = true;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ClearOverriddenActions")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ClearOverriddenActions";
                    a.EventName = "Magix.MetaAction.ClearOverriddenActions";
                    a.Description = @"Will Clear all Overridden Actions. Basically 
destroy all existing overrides, that comes from Actions";
                    a.StripInput = true;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ScrollClientToTop")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ScrollClientToTop";
                    a.EventName = "Magix.MetaView.ScrollClientToTop";
                    a.Description = @"Will scroll the browser all the way to the top. 
Useful for forms which are longer than the fold, when you submit them, and want to scroll to the 
top to display some message";
                    a.StripInput = true;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.CreateNodeFromMetaForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.CreateNodeFromMetaForm";
                    a.EventName = "Magix.MetaForms.CreateNodeFromMetaForm";
                    a.Description = @"Will put every Info field on your Meta Form 
into the Node structure underneath the 'Object' node";
                    a.StripInput = false;

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.LoadMetaForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.LoadMetaForm";
                    a.EventName = "Magix.MetaForms.LoadMetaForm([Expression])";
                    a.Description = @"Will load the given 'MetaFormName' into the 
given 'Container' WebPart";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "MetaFormName";
                    m.Value = "Magix.Forms.EditRating";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Container";
                    m.Value = "content3";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ChangeAllPropertyValues")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ChangeAllPropertyValues";
                    a.EventName = "Magix.Meta.ChangeAllPropertyValues";
                    a.Description = @"Will change the 'PropertyName' 
of every MetaObject of 'TypeName' to 'NewValue'";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "TypeName";
                    m.Value = "YourMetaObjectNamespace.YourMetaObjectTypeName";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "PropertyName";
                    m.Value = "Age";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "NewValue";
                    m.Value = "29";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.CreateNewMetaObject")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.CreateNewMetaObject";
                    a.EventName = "Magix.Meta.CreateNewMetaObject";
                    a.Description = @"Will create a new Meta Object 
with the 'Properties' properties and TypeName of 'TypeName' and parent Meta Object 
of ParentID";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "TypeName";
                    m.Value = "Magix.Demo.MagixIs";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Properties";
                    a.Params.Add(m);

                    Action.ActionParams m2 = new Action.ActionParams();
                    m2.Name = "Coolness";
                    m2.Value = "Supercool!";
                    m.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Usefulness";
                    m2.Value = "Ultrauseful!";
                    m.Children.Add(m2);

                    m = new Action.ActionParams();
                    m.Name = "ParentID";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.TransmitEventToExternalServer")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.TransmitEventToExternalServer";
                    a.EventName = "Magix.Core.TransmitEventToExternalServer";
                    a.Description = @"Will raise an external event to the given 'UrlEndPoint', 
raising the event with the 'EventName', with the parameters of 'Node'. The Remoting capacities of Magix, 
will by default store Authentication Cookies internally, associated with every unique base URL end-point, 
to keep your server Authenticated towards that specific server. 
Magix will store these cookies internally, and later re-transmit these Authentication 
cookies upon every future request, towards that same server. This means that you can for instance use 
the same state as you'd normally use in the Graphical rendering of your system. This facilitates 
for that you can do stuff such as first logging into the remote server raising LogIn or something, 
then to expect at that point, as a server to be authenticated forever towards that server, unless 
your Cookies are somehow destroyed, or your password somehow gets changed. Afterwards you can raise 
events which for instance requires you to be logged in, and expect them to be successfully executed";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "UrlEndPoint";
                    m.Value = "http://localhost/magix/";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "EventName";
                    m.Value = "Magix.Core.Log";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Node";
                    a.Params.Add(m);

                    Action.ActionParams m2 = new Action.ActionParams();
                    m2.Name = "LogItemType";
                    m2.Value = "Magix.Core.TestRemoting";
                    m.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Header";
                    m2.Value = "Remoting Test from Magix was Successful";
                    m.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Message";
                    m2.Value = @"If you can find this in your log somewhere, your server is setup 
correctly to handle remoting of events";
                    m.Children.Add(m2);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.LogInUser")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.LogInUser";
                    a.EventName = "Magix.Core.LogInUser([Object])";
                    a.Description = @"Will login the given 'Username'/'Password' user. 
Either get these fields from your Form/View fields, or set them some other way";
                    a.StripInput = false;

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SaveNodeSerializedFromMetaForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.SaveNodeSerializedFromMetaForm";
                    a.EventName = "Magix.MetaForms.SaveNodeSerializedFromMetaForm";
                    a.Description = @"Will serialize the 'Object' node in your DataSource 
down do a graph depending upon how the Object node looks like";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "TypeName";
                    m.Value = "Magix.Demo.Rating";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "CleanProperties";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetActiveTypeObjects")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.GetActiveTypeObjects";
                    a.EventName = "Magix.Common.GetActiveTypeObjects";
                    a.Description = @"Will return the Active Types of the given fully qualified name 
'FullTypeName', as list within 'Objects'. Every node within 'Objects' will have an 'ID' property 
in addition to a 'Properties' property, which will contain every property you are requesting";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "FullTypeName";
                    m.Value = typeof(MetaObject).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.DataBindForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.DataBindForm";
                    a.EventName = "Magix.MetaForms.DataBindForm";
                    a.Description = @"Will databind the form, which means it will evaluate 
every single expression, in every single property of every single widget on the Active Form 
such that every property that start with DataSource['xxx'] will become evaluated towards the 
DataSource of your WebForm using its expression. You can use expressions such as 
DataSource['Objects'][0]['Property'].Name or DataSource['FullTypeName'].Value, etc. 
The values of those nodes will be attempted converted into whatever the type of the property is 
according to the standard conversion logic in Magix";
                    a.StripInput = false;

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.LoadObjectIntoForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.LoadObjectIntoForm";
                    a.EventName = "Magix.MetaView.LoadObjectIntoForm";
                    a.Description = @"Will assume a Meta Object is just loaded correctly
into the Node structure somehow, and pass it on to any matching TypeName Single Views 
which will handle it and load up the values for the specific Meta Object into their specific 
editor controls";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetUserSetting")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.GetUserSetting";
                    a.EventName = "Magix.Common.GetUserSetting";
                    a.Description = @"Will return the 'Name' User Setting, or the 'Default' if it 
doesn't exist into the 'Value' return value. User settings are dependent upon the user being logged 
in and are unique per User";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "TheNameOfYourSetting";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Default";
                    m.Value = "TheDefaultValueOfYourSetting";
                    a.Params.Add(m);
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SetUserSetting")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.SetUserSetting";
                    a.EventName = "Magix.Common.SetUserSetting";
                    a.Description = @"Sets the 'Name' User setting to the 'Value' parameter's 
value";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "TheNameOfYourSetting";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Value";
                    m.Value = "The_NEW_DefaultValueOfYourSetting";
                    a.Params.Add(m);
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SetFocusToFirstTextBox")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.SetFocusToFirstTextBox";
                    a.EventName = "Magix.MetaView.SetFocusToFirstTextBox";
                    a.Description = @"Will set focus to the first control on the form if 
raised from within the same WebPart as the form it's trying to set focu to. Works only with 
Single Views. Explicitly set the 'OriginalWebPartID' to override to set focus to 
another WebPart's first TextBox on the Page";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ValidateObjectPropertyMandatory")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ValidateObjectPropertyMandatory";
                    a.EventName = "Magix.Common.ValidateObjectProperty";
                    a.Description = @"Will validate the given 'PropertyName' 
Value of the Active MetaView within the WebPart agains the 'Type' parameter. 
Will throw an exception if validation fissles with an error message back to user, 
meaning if you'd like to stop saving of a MetaObject due to validation fissling, 
you'll need to have this Action before the SaveActiveForm Action. The 'Type' parameter 
can be either 'email', 'mandatory', 'number', 'full-name' or 'url' - Plus any of the gazillion plugins
you might have other places in your system. For this Action the 'PropertyName' is 'Name', and 
the 'Type' is 'mandatory', which just means that if there's no value, or only spaces and 
non-alphabetical-characters, then it's considered empty, and an exception will be thrown";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "PropertyName";
                    m.Value = "Name";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Type";
                    m.Value = "mandatory";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ValidateObjectPropertyFullName")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ValidateObjectPropertyFullName";
                    a.EventName = "Magix.Common.ValidateObjectProperty";
                    a.Description = @"Will validate the given 'PropertyName' 
Value of the Active MetaView within the WebPart agains the 'Type' parameter. 
Will throw an exception if validation fissles with an error message back to user, 
meaning if you'd like to stop saving of a MetaObject due to validation fissling, 
you'll need to have this Action before the SaveActiveForm Action. The 'Type' parameter 
can be either 'email', 'mandatory', 'number', 'full-name' or 'url' - Plus any of the gazillion plugins
you might have other places in your system. For this Action the 'PropertyName' is 'Name', and 
the 'Type' is 'full-name', which means that at least two names need to be given, and the 
names will be normalized such as 'Hansen, Thomas' with the last name(s) first and all names 
automatically capitalized as a function of this Action. No automagic Capitalization will occur 
if Capital letters are found in the middle of a string though, such as 'McAngus'";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "PropertyName";
                    m.Value = "Name";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Type";
                    m.Value = "full-name";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ValidateObjectPropertyEmail")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ValidateObjectPropertyEmail";
                    a.EventName = "Magix.Common.ValidateObjectProperty";
                    a.Description = @"Will validate the given 'PropertyName' 
Value of the Active MetaView within the WebPart agains the 'Type' parameter. 
Will throw an exception if validation fissles with an error message back to user, 
meaning if you'd like to stop saving of a MetaObject due to validation fissling, 
you'll need to have this Action before the SaveActiveForm Action. The 'Type' parameter 
can be either 'email', 'mandatory', 'number', 'full-name' or 'url' - Plus any of the gazillion plugins
you might have other places in your system. For this Action the 'PropertyName' is 'Email', and 
the 'Type' is 'email'. Please note that if you wish to allow for _either_ a valid email 
address, _or_ an empty value, you need to pass in 'AcceptNull' being true";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "PropertyName";
                    m.Value = "Email";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Type";
                    m.Value = "email";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ValidateObjectPropertyNumber")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ValidateObjectPropertyNumber";
                    a.EventName = "Magix.Common.ValidateObjectProperty";
                    a.Description = @"Will validate the given 'PropertyName' 
Value of the Active MetaView within the WebPart agains the 'Type' parameter. 
Will throw an exception if validation fissles with an error message back to user, 
meaning if you'd like to stop saving of a MetaObject due to validation fissling, 
you'll need to have this Action before the SaveActiveForm Action. The 'Type' parameter 
can be either 'email', 'mandatory', 'number', 'full-name' or 'url' - Plus any of the gazillion plugins
you might have other places in your system. For this Action the 'PropertyName' is 'Age', and 
the 'Type' is 'number'. Please note that if you wish to allow for _either_ a valid number 
address, _or_ an empty value, you need to pass in 'AcceptNull' being true. The valid characters 
in a 'number' are; '0123456789,. '";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "PropertyName";
                    m.Value = "Age";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Type";
                    m.Value = "number";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.CreateGallery")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.CreateGallery";
                    a.EventName = "Magix.Common.CreateGallery";
                    a.Description = @"Will create a Gallery object from the given 
'Files' list within the 'Folder'";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.IncludeCSSFile")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.IncludeCSSFile";
                    a.EventName = "Magix.Core.AddCustomCssFile";
                    a.Description = @"Will include a CSS file onto the page, even 
in an Ajax Callback if you wish. Change the 'CSSFile' parameter to choose which CSS file 
you'd like to include";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "CSSFile";
                    m.Value = "media/your-css-file-and-path.css";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.CreateQRCode")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.CreateQRCode";
                    a.EventName = "Magix.QRCodes.CreateQRCode";
                    a.Description = @"Will create a QR Code with the given 'FileName' path 
and filename, which should end with .png. The QR Code will point to the given 'URL', and it will use 
the textures of 'BGImage' and 'FGImage' to render the code. The QR Code will have 'RoundedCorners' 
radius of rounded corners, and it will be 'AntiPixelated', and have the descriptive text of 
'Text'. The QR Code will use 'Scale' number of pixels per square to render";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "FileName";
                    m.Value = "Tmp/test-qr-code.png";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "URL";
                    m.Value = "http://code.google.com/p/magix";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "FGImage";
                    m.Value = "media/images/textures/leather-black.png";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "BGImage";
                    m.Value = "media/images/textures/marble-white.png";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "RoundedCorners";
                    m.TypeName = typeof(int).FullName;
                    m.Value = "125";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Scale";
                    m.TypeName = typeof(int).FullName;
                    m.Value = "12";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "AntiPixelated";
                    m.TypeName = typeof(bool).FullName;
                    m.Value = "True";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Text";
                    m.Value = "Magix!";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "FontName";
                    m.Value = "Comic Sans MS";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "FontSize";
                    m.TypeName = typeof(int).FullName;
                    m.Value = "32";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.PlaySound")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.PlaySound";
                    a.EventName = "Magix.Core.PlaySound";
                    a.Description = @"Will play the given 'File' sound. If the sample 
sound file doesn't work, you've probably got something wrong with the setup 
of your Web Server. Make sure the file extension .oga is associated with the 
MIME type of audio/ogg. PS! For the record; Cool-Breeze.oga and The-Last-Barfly.ogg 
are both songs composed by our CTO Thomas Hansen. They are both performed by Thomas Hansen and 
his wife Inger Hoeoeg, and are to be considered licensed to you under the terms of 
Creative Commons Attribution-ShareAlike 3.0";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "File";
                    m.Value = "media/Cool-Breeze.oga";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.PauseSound")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.PauseSound";
                    a.EventName = "Magix.Core.PauseSound";
                    a.Description = @"Stops Any sound or music currently being played";
                    a.StripInput = true;

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ResumeSound")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ResumeSound";
                    a.EventName = "Magix.Core.ResumeSound";
                    a.Description = @"Resumes any sound or music previously being halted through 'PauseSound' 
or other similar mechanisms. PS! Will throw exception if no sounds have been played, and hence 
no resuming can occur in any ways";
                    a.StripInput = true;

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.EmptyAndClearActiveForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.EmptyAndClearActiveForm";
                    a.EventName = "Magix.MetaView.EmptyForm";
                    a.Description = @"Will empty the currrently active Editable Form. 
Will determine which form raised the event originally, and explicitly empty that 
form only. Useful for things such as 'Clear Buttons' and such ...";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.RedirectClient")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.RedirectClient";
                    a.EventName = "Magix.Common.RedirectClient";
                    a.Description = @"Will redirect the client's browser to the
given URL parameter";

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "URL";
                    m.Value = "http://google.com";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.PutGETParameterIntoDataSource")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.PutGETParameterIntoDataSource";
                    a.EventName = "Magix.Common.PutGETParameterIntoDataSource";
                    a.Description = @"Will get the given parameter from the URL string and put it 
into the given node";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "ParamName";
                    m.Value = "ID";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "ConvertToType";
                    m.Value = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.Transform")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.Transform";
                    a.EventName = "Magix.Common.Transform";
                    a.Description = @"Will transform the given parameter node immutably [will not change the 
source] and return as a new Node collection according to the expression(s) in the 'Expression' parameter";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Expression";
                    m.Value = "";
                    a.Params.Add(m);

                    Action.ActionParams m2 = new Action.ActionParams();
                    m2.Name = "ID";
                    m2.Value = "{[WidgetInfo].Value}";
                    m.Children.Add(m2);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetMetaObjectGraph")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.GetMetaObjectGraph";
                    a.EventName = "Magix.Common.GetMetaObjectGraph";
                    a.Description = @"Will return the entire Graph [warning, might be HUGE!] for 
the MetaObject given, with all its Children Meta Objects too within the 'Object' node of your 
parameter";
                    a.StripInput = false;

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Demo.ViewCarsInPopup")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Demo.ViewCarsInPopup";
                    a.EventName = "Magix.MetaView.ViewMetaViewMultiMode";
                    a.Description = @"Will load up any Magix.Demo.Car objects 
into a MultiView form, within a popup window, using the Magix.Demo.ImportCars MetaView. 
If there are no items you can run the 'Magix.Demo.ImportCarsCSVFile' action, which 
by default will point towards a CSV file which contains 'Cars' data";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "NoIdColumn";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "IsDelete";
                    m.Value = "False";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "MetaViewName";
                    m.Value = "Magix.Demo.ViewCars";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "IsInlineEdit";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Container";
                    m.Value = "child";
                    a.Params.Add(m);

                    a.Save();

                    MetaView v = new MetaView();
                    v.Name = "Magix.Demo.ViewCars";
                    v.TypeName = "Magix.Demo.Car";

                    MetaView.MetaViewProperty p = new MetaView.MetaViewProperty();
                    p.Name = "Car";
                    p.ReadOnly = true;
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "MPG";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Horsepower";
                    p.ReadOnly = false;
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Acceleration";
                    p.ReadOnly = false;
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "select:Eco.Green:Eco";
                    v.Properties.Add(p);

                    v.Save();

                    MetaObject o = new MetaObject();
                    o.Reference = "Magix.Demo.Template";
                    o.TypeName = "Eco";

                    MetaObject.Property q = new MetaObject.Property();
                    q.Name = "Green";
                    q.Value = "Nope!";
                    o.Values.Add(q);

                    o.Save();

                    o = new MetaObject();
                    o.Reference = "Magix.Demo.Template";
                    o.TypeName = "Eco";

                    q = new MetaObject.Property();
                    q.Name = "Green";
                    q.Value = "OK'ish";
                    o.Values.Add(q);

                    o.Save();

                    o = new MetaObject();
                    o.Reference = "Magix.Demo.Template";
                    o.TypeName = "Eco";

                    q = new MetaObject.Property();
                    q.Name = "Green";
                    q.Value = "Good";
                    o.Values.Add(q);

                    o.Save();

                    o = new MetaObject();
                    o.Reference = "Magix.Demo.Template";
                    o.TypeName = "Eco";

                    q = new MetaObject.Property();
                    q.Name = "Green";
                    q.Value = "SUPERB!";
                    o.Values.Add(q);

                    o.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Demo.ImportCarsCSVFile")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.Demo.ImportCarsCSVFile";
                    a.EventName = "Magix.Common.ImportCSVFile";
                    a.Description = @"Will import the given 'FileName' from the given 'Folder' 
and transform to MetaObjects using the given 'MetaViewName'";

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "FileName";
                    m.Value = "exchange-file-name.csv";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Folder";
                    m.Value = "Tmp/";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "MetaViewName";
                    m.Value = "Magix.Demo.ImportCars";
                    a.Params.Add(m);

                    a.Save();

                    MetaView v = new MetaView();
                    v.Name = "Magix.Demo.ImportCars";
                    v.TypeName = "Magix.Demo.Car";

                    MetaView.MetaViewProperty p = new MetaView.MetaViewProperty();
                    p.Name = "Car";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Manufacturer";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "MPG";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Cylinders";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Displacement";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Horsepower";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Weight";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Acceleration";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Model-Year";
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Origin";
                    v.Properties.Add(p);

                    v.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ShowMessage")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ShowMessage";
                    a.EventName = "Magix.Core.ShowMessage";
                    a.Description = @"Will show a 'Message Box' to the User. If you add 'IsError' to true, 
the message box will be in error mode, meaning red probably, signifying a an error";

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Message";
                    m.Value = "Your operation was Successful";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Header";
                    m.Value = "Sucess!";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Milliseconds";
                    m.Value = "1000";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.TurnOnDebugging")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.TurnOnDebugging";
                    a.EventName = "Magix.Common.SetSessionVariable";
                    a.Description = @"Will turn on 'Debugging', meaning you'll have a wire-grid
covering your screen to see the 40x18 pixel 'grid-lock', plus you'll also get to see every single Action 
ever raised on the server shown in an 'Action Stack Trace' Window. This only affects your session, 
meaning it should be safe to do in production to track down errors and such in live software ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "Magix.Core.IsDebug";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Value";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.TurnOffDebugging")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.TurnOffDebugging";
                    a.EventName = "Magix.Common.SetSessionVariable";
                    a.Description = @"Will turn _OFF_ 'Debugging', meaning you'll no longer 
have a wire-grid covering your screen, plus the stack tracing of actions on the 
server will disappear. Only affects your session, and no other logged on users ability to 
see debugging information ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "Magix.Core.IsDebug";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Value";
                    m.Value = "False";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ViewMetaViewMultiMode")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.ViewMetaViewMultiMode";
                    a.EventName = "Magix.MetaView.ViewMetaViewMultiMode";
                    a.Description = @"Will load a grid of all Meta Objects of type already loaded
in current activating WebPart. If you want to load a specific type, then you can override the 
type being loaded by adding 'MetaViewTypeName' as a parameter, containing the name of the view";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "NoIdColumn";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "IsDelete";
                    m.Value = "False";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "IsInlineEdit";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SendEmail")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.SendEmail";
                    a.EventName = "Magix.Common.SendEmail";
                    a.Description = @"Will send an email to the 'To' address";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Header";
                    m.Value = "Hello World";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Body";
                    m.Value = "Hello there stranger ...";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Email";
                    m.Value = "your-email-adr-goes-here@your-email-adr-goes-here.com";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "From";
                    m.Value = "Marvin Magix";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "To";
                    m.Value = "the-email-address-you-wish-to-send-to-goes-here@qwertyuiopasdfgzzxxqq.com";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SendEmailFromForm")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.SendEmailFromForm";
                    a.EventName = "Magix.Common.SendEmailFromForm";
                    a.Description = @"Helper to further assist on creating 'Send Email Forms'. 
Will send an email with the given 'Body' and 
'Header' to the email address within the 'Email' field of the Active SingleView Form View.
It will also replace every instance of [x] with the MetaView's property with the same named 
value. Meaning if you've got a MetaView property called 'Name' it will replace all occurances 
of [Name] with the value from the MetaObject's property of 'Name' in both the body and the header.
PS! This Action can be seen in 'Action' in the 'Magix.Demo.SendEmail' MetaView";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Header";
                    m.Value = "Hello [Name] ...";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Body";
                    m.Value = "... and thanx for asking me to send you an email. You're [Age] years old ...";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Email";
                    m.Value = "your-email-adr-goes-here@your-email-adr-goes-here.com";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "From";
                    m.Value = "Marvin Magix";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    a.Save();

                    MetaView v = new MetaView();
                    v.Name = "Magix.Demo.SendEmail";
                    v.TypeName = "Magix.Demo.Email";

                    MetaView.MetaViewProperty q = new MetaView.MetaViewProperty();
                    q.Name = "Email";
                    q.Description = "Email Address ...";
                    v.Properties.Add(q);

                    q = new MetaView.MetaViewProperty();
                    q.Name = "Age";
                    q.Description = "Age ...";
                    v.Properties.Add(q);

                    q = new MetaView.MetaViewProperty();
                    q.Name = "linkedE2M:Email:Name";
                    q.Description = "Full Name ...";
                    v.Properties.Add(q);

                    q = new MetaView.MetaViewProperty();
                    q.Name = "init-actions:FocusForm";
                    q.Description = "Actions done during first initial loading of form ...";
                    q.Action = "Magix.DynamicEvent.SetFocusToFirstTextBox";
                    v.Properties.Add(q);

                    q = new MetaView.MetaViewProperty();
                    q.Name = "Send";
                    q.Description = "Send yourself a Test Email from Marvin Magix ...";
                    q.Action = "Magix.DynamicEvent.ValidateObjectPropertyFullName|Magix.DynamicEvent.ValidateObjectPropertyEmail|Magix.DynamicEvent.ValidateObjectPropertyNumber|Magix.DynamicEvent.SaveActiveForm|Magix.DynamicEvent.ShowMessage|Magix.DynamicEvent.SendEmailFromForm|Magix.DynamicEvent.EmptyAndClearActiveForm";
                    v.Properties.Add(q);

                    q = new MetaView.MetaViewProperty();
                    q.Name = "Clear";
                    q.Description = "Clears the form ...";
                    q.Action = "Magix.DynamicEvent.EmptyAndClearActiveForm";
                    v.Properties.Add(q);

                    v.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ReplaceStringValue")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.ReplaceStringValue";
                    a.EventName = "Magix.Common.ReplaceStringValue";
                    a.Description = @"Will transform every entity of 'OldString' 
found in 'Source' into the contents of 'NewString' and return as a 'Result', output node. You 
can also use 'SourceNode', 'NewStringNode' and 'ResultNode' to override which Nodes to 
read values and store result into";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Source";
                    m.Value = "Howdy doodie woodie, my name is [0]";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "OldString";
                    m.Value = "[0]";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "NewString";
                    m.Value = "Marvin";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.MultiAction")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.MultiAction";
                    a.EventName = "Magix.Common.MultiAction";
                    a.Description = @"Will raise several Actions consecutively, in the order they're defined
in the 'Actions' node. Each Action needs a 'Name' and its own set of parameters through its 'Params' node.
All 'Params' nodes will be copied into the root node before every event is raised. This means that your
Root node will become VERY large after subsequent actions. Be warned ...";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Actions";
                    a.Params.Add(m);

                    Action.ActionParams ar = new Action.ActionParams();
                    ar.Name = "act-1";
                    m.Children.Add(ar);

                    Action.ActionParams m2 = new Action.ActionParams();
                    m2.Name = "ActionName";
                    m2.Value = "Magix.DynamicEvent.ShowMessage";
                    m2.TypeName = typeof(string).FullName;
                    ar.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Params";
                    ar.Children.Add(m2);

                    Action.ActionParams m3 = new Action.ActionParams();
                    m3.Name = "Message";
                    m3.Value = @"Sure Marvin can MultiTask, I can talk and dance at the SAME TIME! Watch Marvin go; JAZZ !! ;)";
                    m2.Children.Add(m3);

                    m3 = new Action.ActionParams();
                    m3.Name = "Milliseconds";
                    m3.Value = "5000";
                    m3.TypeName = typeof(int).FullName;
                    m2.Children.Add(m3);

                    m3 = new Action.ActionParams();
                    m3.Name = "Header";
                    m3.Value = "Blame it on the Boogie ... !! ;)";
                    m2.Children.Add(m3);

                    ar = new Action.ActionParams();
                    ar.Name = "act-2";
                    m.Children.Add(ar);

                    m2 = new Action.ActionParams();
                    m2.Name = "ActionName";
                    m2.Value = "Magix.DynamicEvent.PlaySound";
                    ar.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Params";
                    ar.Children.Add(m2);

                    m3 = new Action.ActionParams();
                    m3.Name = "File";
                    m3.Value = "media/Cool-Breeze.oga";
                    m2.Children.Add(m3);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetSingleMetaObject")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.GetSingleMetaObject";
                    a.EventName = "Magix.Common.GetSingleMetaObject";
                    a.Description = @"Will put every property from the given Meta Object, 
into the given Node, with the name/value pair as the node name/value parts, 
assuming they're all strings. Copy this Action, and make sure you _CHANGE_ its ID
towards pointing to the ID of a real existing Meta Object, to fill in values from one of
your Meta Objects into a Node, maybe before sending the node into another even, using
MultiActions or something ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "ID";
                    m.Value = "-1";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ReplaceStringLiteral")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.ReplaceStringLiteral";
                    a.EventName = "Magix.Common.ReplaceStringLiteral";
                    a.Description = @"Expects three parameters, 'Replace', 'Replacement' and 
'Source'. Will replace every occurency of 'Replace' in the 'Source' Node Expression 
with either the expression or the value in the 'Replacement' parameter";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Source";
                    m.Value = "{[MyText].Value}";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Replace";
                    m.Value = "Who's there?";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Replacement";
                    m.Value = "{[HelloWorld].Value}";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "HelloWorld";
                    m.Value = "Magix!";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "MyText";
                    m.Value = "Who's there? Who's there? Who's there? I'm here !! :)";
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetMetaObjects")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.GetMetaObjects";
                    a.EventName = "Magix.MetaObjects.GetMetaObjects";
                    a.Description = @"Will return from 'Start' to 'End' MetaObjects 
sorted according to newest first of Type Name 'MetaTypeName'. If you set 'Ascending' to false, 
it will sort according to oldest first. Modify the 'Start' and 'End' parameters to 
change your data extraction";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "MetaTypeName";
                    m.Value = "Eco";
                    a.Params.Add(m);// TODO: Implement Set Focus to Meta Forms ...

                    m = new Action.ActionParams();
                    m.Name = "Start";
                    m.Value = "0";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "End";
                    m.Value = "2";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Demo.GetRatingObjects")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Demo.GetRatingObjects";
                    a.EventName = "Magix.MetaObjects.GetMetaObjects";
                    a.Description = @"Will return the 5 newest Magix.Demo.Rating Meta Objects";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "MetaTypeName";
                    m.Value = "Magix.Demo.Rating";
                    a.Params.Add(m);// TODO: Implement Set Focus to Meta Forms ...

                    m = new Action.ActionParams();
                    m.Name = "Start";
                    m.Value = "0";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "End";
                    m.Value = "5";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetNextMetaObjects")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.GetNextMetaObjects";
                    a.EventName = "Magix.MetaObjects.GetNextMetaObjects";
                    a.Description = @"Will return from 'Start' to 'End' MetaObjects 
sorted according to newest first of Type Name 'MetaTypeName'. But only AFTER both 
'Start' and 'End' have been incremented by 'End' - 'Start'. If you set 'Ascending' to false, 
it will sort according to oldest first. Modify the 'Start' and 'End' parameters to 
change your data extraction. Useful for chaining together with its Previous counterpart, and its 
get specific extraction counterpart";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "MetaTypeName";
                    m.Value = "Eco";
                    a.Params.Add(m);// TODO: Implement Set Focus to Meta Forms ...

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetPreviousMetaObjects")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.GetPreviousMetaObjects";
                    a.EventName = "Magix.MetaObjects.GetPreviousMetaObjects";
                    a.Description = @"Will return from 'Start' to 'End' MetaObjects 
sorted according to newest first of Type Name 'MetaTypeName'. But only AFTER both 
'Start' and 'End' have been decremented by 'End' - 'Start'. If you set 'Ascending' to false, 
it will sort according to oldest first. Modify the 'Start' and 'End' parameters to 
change your data extraction. Useful for chaining together with its Previous counterpart, and its 
get specific extraction counterpart";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "MetaTypeName";
                    m.Value = "Eco";
                    a.Params.Add(m);// TODO: Implement Set Focus to Meta Forms ...

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SetMetaObjectValue")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.DynamicEvent.SetMetaObjectValue";
                    a.EventName = "Magix.MetaType.SetMetaObjectValue";
                    a.Description = @"Will set the value of the given MetaObject 
[ID] to the Value of your 'Value' node at the 'Name' property.";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "ID";
                    m.Value = "-1";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Value";
                    m.Value = "New Value of property ...";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "PropertyName";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SerializeActiveSingleViewForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.SerializeActiveSingleViewForm";
                    a.EventName = "Magix.MetaView.SerializeSingleViewForm";
                    a.Description = @"Will find the Form that raised the current event
chain, and query it to put all its data flat out into the current Node structure
with the Name/Value as the Node Name/Value pair. Note; Can mostly only be raised from within 
a Single View MetaView, unless you're doing other overrides";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.LoadSignatureModule")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.LoadSignatureModule";
                    a.EventName = "Magix.Common.LoadSignatureForCurrentMetaObject";
                    a.Description = @"Will load the Signature Module in
whatever container its being raised from. And set the 'Value' property of the given
MetaObject Column Property Name to the signature signed on the Signature module ...";
                    a.StripInput = false;

                    Action.ActionParams  m = new Action.ActionParams();
                    m.Name = "Width";
                    m.Value = "710";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Height";
                    m.Value = "360";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ExportMetaView2CSV")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ExportMetaView2CSV";
                    a.EventName = "Magix.Common.ExportMetaView2CSV";
                    a.StripInput = false;
                    a.Description = @"Will render the 'Currently Viewed' MetaView into
a CSV file [Microsoft Excel or Apple Numbers etc] and redirect the users client [Web Browser] 
to the newly rendered CSV file. 'Currently Viewed' meaning the view that contained the 
control that initiated this Action somehow. PS! If you explicitly create a 'MetaViewName' parameter, 
and set its name to another MetaView, then that MetaView will be rendered instead";

                    a.Save();
                }

                tr.Commit();
            }
        }

        #endregion
    }
}
