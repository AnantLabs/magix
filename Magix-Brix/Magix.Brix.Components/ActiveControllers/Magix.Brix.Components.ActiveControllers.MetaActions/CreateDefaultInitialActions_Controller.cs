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
                    Criteria.Eq("Name", "Magix.DynamicEvent.EmptyActiveForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.EmptyActiveForm";
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
                    Criteria.Eq("Name", "Magix.DynamicEvent.ImportCSVFile")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ImportCSVFile";
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
                    Criteria.Eq("Name", "Magix.DynamicEvent.ShowDefaultMessage")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ShowDefaultMessage";
                    a.EventName = "Magix.Core.ShowMessage";
                    a.Description = @"Will show a 'Message Box' to the User. If you add 'IsError' to true, 
the message box will be in error mode, meaning red probably, signifying a an error";

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Message";
                    m.Value = "Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... Knock, Knock ... HEAD ACHE ...!!";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Header";
                    m.Value = "Who's there ...?";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Milliseconds";
                    m.Value = "3500";
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
                    q.Action = "Magix.DynamicEvent.ValidateObjectPropertyFullName|Magix.DynamicEvent.ValidateObjectPropertyEmail|Magix.DynamicEvent.ValidateObjectPropertyNumber|Magix.DynamicEvent.SaveActiveForm|Magix.DynamicEvent.ShowDefaultMessage|Magix.DynamicEvent.SendEmailFromForm|Magix.DynamicEvent.EmptyActiveForm";
                    v.Properties.Add(q);

                    q = new MetaView.MetaViewProperty();
                    q.Name = "Clear";
                    q.Description = "Clears the form ...";
                    q.Action = "Magix.DynamicEvent.EmptyActiveForm";
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
                    m2.Value = "Magix.DynamicEvent.ShowDefaultMessage";
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

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Demo.ViewCarsInPopup")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Demo.ViewCarsInPopup";
                    a.EventName = "Magix.MetaView.ViewMetaViewMultiMode";
                    a.Description = @"Will load up any Magix.Demo.Car objects 
into a MultiView form, within a popup window, using the Magix.Demo.ImportCars MetaView";
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
                    p.ReadOnly = true;
                    v.Properties.Add(p);

                    p = new MetaView.MetaViewProperty();
                    p.Name = "Acceleration";
                    p.ReadOnly = true;
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

                tr.Commit();
            }
        }

        #endregion
    }
}
