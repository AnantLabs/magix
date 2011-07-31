/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Web;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes.Logging;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class AdministratorDashboardController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            // TODO: Throw away while testing ToolTip functionality ....
            //foreach (ToolTip.Tip idx in ToolTip.Tip.Select())
            //{
            //    idx.Delete();
            //}
            //foreach (ToolTip.TipPosition idx in ToolTip.TipPosition.Select())
            //{
            //    idx.Delete();
            //}
            if (ToolTip.Instance.Count == 0)
            {
                ToolTip.Instance.CreateTip(@"
<h2>Welcome to Magix!</h2>
<p>Truly a Strange and Wonderful World ...</p>
<p>A place where you can become literate in regards to computers. A place where you can express yourself, creating what you want out of your computer. A place where you are in charge, a place which is fun!</p>
<p>This is your First tip of the day. <em>Leave These Tips On</em> to make sure you get Useful Tips and Tricks as you proceed deeper and deeper into the Rabbit Hole ...</p>
<p>In fact, your first Tip is to use the Arrow Keys in the Top/Right corner of this window to navigate forward and fast read the next upcoming 5-10 tips. They are all crucial for getting started with Magix ...</p>
<p>If you need to re-read a previous tip, you can click the previous button ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Magix consists of two majorly important pre-installed modules; The <em>'Publishing'</em> System and the <em>'MetaType'</em> System.</p>
<p>Publishing is where you go to create your WebSite, and MetaTypes is where you go to create Applications. Though, really the difference is more blurry than you think. For instance; It is difficult to show any Applications to your end users, if you do not have any Pages to 'host' your Application ... </p>
<p>So Pages can be thought of as Views in your Applications if you want to, or your entire hierarchy of Pages can be viewed as a gigantic plugin Application, which it actually in fact is ... ;)</p>
<p>It is actually quite useful to <em>stop separating</em> between 'Old-Time Constructs' such as 'code', 'data', 'input' and 'output'. </p>
<p>Old World thinking, trying to categorize things into different types, are much less useful in Magix than what you think. In Magix, everything is kind of 'mushy'. Or <em>'everything is everything'</em> I guess you can say. This is what makes it possible for you to Stay in Control and deliver Secure and Stable Systems, regardless of the Complexity of your Domain Problem ...</p>
<p>We recommend people to <em>Start with Learning Publishing</em> and how the WebPages work. Then later, only when a firm grasp of Pages and Templates are understood, we recommend moving onto Applications ...</p>");
                ToolTip.Instance.CreateTip(@"
<h2>Basics ...</h2>
<p>But before we can do anything else, we need to learn the <em>Basics</em> ...</p>
<p>Most of Magix is made up of 'Basic Components', which are tied together to create a whole.</p>
<p>For instance, a button will mostly look the same everywhere. By default a button will be Gray and use Bold, Black and Big Fonts ...</p>
<p>A good example of a Button is the Top/Right corner of this tooltip, which has two Buttons. One Paging forward, and another paging Backwards in the Hierarchy of Tips and Tricks ...</p>
<p>A 'Grid' is when you see a list of items. A good example of a Grid would be MetaTypes/Meta Actions ...</p>
<p>There are many types of 'Basic Components' like these in Magix. Over the next couple of pages, we'll be walking through some of them which you'll need to understand to be able to get the <em>most out of Magix</em> ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Most Grids in Magix have tons of features. These features includes; Paging, In Place Editing of Values, Filtering, and so on ...</p>
<p>To Filter according to a Column, all you've got to do is to click the header of your Grid, choose which type of Filter you want to apply, type in its value, and click OK ...</p>
<p>The arrow buttons, normally at the bottom of your grids makes it possible to traverse forward and backwards in your list of items. The double arrows [""&lt;&lt;""] takes you <em>'all the way'</em> in its direction ...</p>
<p><em>Open up Meta Types/Meta Actions</em> and play around with that grid by filtering, creating a couple of new items and so on.</p>
<p>Make sure you <em>don't change any</em> of the existing items, since some things are dependent upon 'System Actions' which must be defined for your system to properly work ...!</p>
<p>To see Paging you'll normally need to have more than 10 items in your grid. To see 'all the way paging', you'll normally need more than 20 items ...</p>
<p>Make sure you also click the 'Edit' column. Sometimes this column will say 'Edit' while sometimes it'll show a number like in the Action view. However, clicking the Edit Column, will always somehow bring you to a View where that object can be edited in 'full version' ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Some things should be very similar for mostly all grids like this in Magix. For instance ...</p>
<p>Clicking the '+' button will almost always create a new object of that type ...</p>
<p>If the Text of a Grid Cell is Blue, this means that you can edit the value directly by <em>clicking the Blue Text</em>, which will exchange it with a 'textbox', from where you can edit its value ...</p>
");
                ToolTip.Instance.CreateTip(@"
<h2>Publishing ...</h2>
<p>A Website consists of Pages. Every Page is the equivalent of one 'URL'. Although URLs doesn't really exist in Magix, it helps to think of a page as such. Beside, creating a URL based Navigation Plugin would be piece of cake anyway, due to the Architectural Principles Magix is built on ...</p>
<p>Anyway ...</p>
<p>You create your pages according to 'Templates', which can be seen as <em>'Recipes'</em> for your pages. You have to have at least <em>One Template</em> in your system before you can start creating Pages. Every Page is based upon a Template, and no page can exist without its Template ...</p>
<p>Click <em>'Publishing-&gt;Templates ...'</em> now!</p>
<p>If you click 'Dashboard' at the root of your menu you will return back here. To access the root of your menu, click the left arrow at the top of your Sliding Menu ...</p>
<p>PS!<br />Click the Edit links to view any specific templates ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Every Template contains a bunch of WebPart Templates. These WebPart Templates can be positioned exactly as you wish on your page. When editing a Template, use the <em>Arrow Buttons</em> to position your WebParts ...</p>
<p>Go check out <em>'Publishing-&gt;Templates ...'</em> one more time, and see how you can move stuff around on your 'surface' by clicking the Arrow Buttons ... </p>
");
                ToolTip.Instance.CreateTip(@"
<p>When positioning your WebPart Templates, realize that you're not really 'positioning' them, but rather you are changing their <em>width, height and margins</em>. In the beginning this might feel a little bit cumbersome, though after some time you'll hopefully appreciate this 'floating layout' and become used to it ...</p>
<p>Realize also that especially the bottom and right margins might create funny looking WebParts since they're not really visible while editing. If you're having weird results, make sure your right and bottom margins are 0 by <em>double clicking</em> them, which should set them back to zero ... ;)</p>
<p>Double clicking any of the arrows will either maximize or minimize their associated property ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>A WebPage Template must contain  at least one WebPart Template. A WebPart Template is also a 'Type Definition' for your WebParts. WebPart Templates have names such as 'Content' and 'Header', which are publishing modules for showing large letters and rich text fragments. Every WebPart Template is based upon one plugin type. </p>
<p>Meaning if you have one page, based upon a WebPage Template, with 5 WebPart Templates, you'll have a WebPage with 5 WebParts where each WebPart can be different '<em>Applications'</em>. This might be any combination of Applications, such as Text Fragments, Headers, CRUD Forms and such ...</p>
<p>However, we'll stick to 'Publishing' as we promised in the beginning, and focus on the Publishing Modules ...</p>
<p>If you edit the default Template created by the system for you, you can see how it has Menu, Header and Content as <em>'Module Names'</em></p>
<p>Go check it out while I hang around here ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>The Menu is similar to the Sliding Menu to the left which you're using yourself, while the Header will show an H1 HTML element [Header Element] and the Content module will show Rich Editable Content.</p>
<p>If you have more types of Modules in your installation of Magix, these might also show up as selections in the DropDownBoxes visible while editing your Templates ...</p>
<p>No go to Templates and change the Name of Template 'M+H+C'. Change it to 'Testing'. This can be done by clicking directly on the text where it says 'M+H+C'. Then open up 'Pages ...' and click your root page.</p>
<p>Do you see how the name of the module in the DropDown box, roughly at the middle of the screen has changed now to 'Testing'. This is because that DropDown box is being used to select a Template for your WebPage.</p>
<p>Then try to change the Name of your Templates by clicking e.g. Header while editing your Template, and type in 'Header2'. Now edit your Page and see how this change reflects from the Template and to the Page.</p>
<p>Go take a look, while I chill ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p><em>Important:</em> If you change the number of WebParts in your Template, or you change the type of Module of your WebPart Template, then <em>all pages</em> built upon that Template will have to be resaved, and you'll probably loose data. </p>
<p>It is therefor important that you create your templates first, and then don't edit these two properties while they're already in 'Production' ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Try to Create several different Templates, and have slightly different values for their width, height, margins and such.</p>
<p>Be certain of that you've added different widths of your Menu Containers and different <em>Left Margins</em></p>
<p>Make sure they've got the same type of modules in the same container</p>
<p>Then use these different Templates for different pages which you create in your Pages hierarchy</p>
<p>If you now access the root of your website, and try to browse around by clicking different buttons, you can see how the WebPart Containers are 'jumping around' on the screen ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>WebParts Initialization...</p>
<p>WebParts initializes differently depending upon where you're coming from, and from which Template type you're coming from dependent upon to which Template type you're entering. And also according to which type of WebParts, or Modules they are ... </p>
<p>For instance the sliding menu will not reload as long as the container it is within on one page template, is the same container it is within on the next page template ...</p>
<p>Too confusing ...?</p>
<p>Just remember; <em>always have a Menu</em> positioned in the same container [first one for instance?] on all of your templates, unless you really know what you're doing ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p><em>Play around</em> with the system by creating some new Templates, copying them, change their weparts type between Header, Content and SliderMenu. Then when you come back, we'll start diving into Applications ... ;)</p>
");
                ToolTip.Instance.CreateTip(@"
<h2>Applications ...</h2>
<p>Most applications will be WebParts. This means that you can inject them into any WebPart onto any WebPage you wish.</p>
<p>Let's create an Application ... :)</p>
<p>First make sure you have one Template in use in one of your pages which has one WebPart Template with the Module Type of <em>'MetaView_Single'</em> ...</p>
<p>Then click on <em>'MetaTypes/Meta Views'</em> ...</p>
<p>Create a new View called 'CollectEmails' ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Add three properties to your form, name them</p>
<ul>
<li>Name</li>
<li>Email</li>
<li>Subscribe</li>
</ul>
<p>Change their description to something meaningful ...</p>
<p>Make sure you change the 'Type Name' of your object to <em>'EmailSubscription'</em></p>
");
                ToolTip.Instance.CreateTip(@"
<p>Attach two Actions to your 'Save' property.</p>
<ul>
<li>Magix.DynamicEvent.SaveActiveForm</li>
<li>Magix.DynamicEvent.EmptyActiveForm</li>
</ul>
<p>The first Action will save your form, while the second one will empty it.</p>
<p>Now try to <em>View your form in preview</em> mode, and test it out by typing in your email and name, and clicking Submit to save your Object ...</p>
<p>PS!<br/>
Obviously it's crucial that the 'Save' action runs before the 'Empty' action, in case you wondered ... ;)</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Meta Objects and Type Names ...</p>
<p>If you take a look at your <em>Meta Objects</em> now you will see a new object with the Type Name of 'EmailSubscription'. The <em>'Type Name'</em> property from your MetaView decides the Type Name of your Objects ...</p>
<p>These 'Type Names' are important to distinguish from different types of Objects.</p>
<p>One Object might be of type 'Customer', while another object might be of type 'Email', and so on. What names you give your Types is <em>crucial</em>! Name clashes here might create very hard to track down bugs and such ...</p>
<p>Take some care when naming your Types!</p>
<p>It's probably a good practice to some how make sure they've got unique names, also across your organization if you want to use plugins made by others.</p>
<p>We encourage people to use type names such as; ""CompanyName.Department.Customer"", and never 'Customer' directly. In fact, your <em>homework</em> for this lesson is to go and rename your 'Customer' TypeName, and rename the Type Name to; 'CompanyName.Department.Customer'. Where Company Name and Department are <em>your</em> company name and your department ...</p>
");
                ToolTip.Instance.CreateTip(@"
<h3>OpenID</h3>
<p>Did you know that you can use Magix as both a Relying Party and OpenID Provider?</p>
<p>[ Read more about Open ID <a href=""http://openid.net/"" target=""_blank"">here</a> ... ]</p>
<p>If you need an OpenID token to log into some website somewhere, then you can append ?openID=admin after the root URL to your website, if the admin user is the User you'd like to log in with.</p>
<p>[ Full example; yourdomain.com/?openID=admin ]</p>
<p>This will redirect from the website you're trying to log into, and back to your website, which in turn will ask you for admin's password.</p>
<p>Once successfully logged into your own Magix website, your website will redirect back to the website you're trying to log into, and tell it that it 'has prof of that you are who you claimed you were'.</p>
<p>You can also associate other OpenID tokens, such as your Yahoo account or Blogger account, with your Magix User internally. This will allow you to log into Magix with that OpenID Account.</p>
<p>If your blogger account is 'magix' for instance, then your OpenID 'username' [claim] would become 'magix.blogspot.com'</p>
");
                ToolTip.Instance.CreateTip(@"
<h3>Talk to CEO</h3>
<p>If you make something cool with Magix, that you want to share, then we'd love to get to know about it. It can be an instructional YouTube video about how to Get Started, Tips and Tricks etc. It can be a Tutorial Blog you've written about how to install Magix on your server. It can be a book you have written about the O2 Architecture. It can be an Action, or a collection of Actions, which you think is awesome. Anything really! </p>
<p>As long as it has Value for our Community somehow, we'd love to know about it, so that we could help you promote it, and more importantly; Helping our Community out ... :)</p>
<p>Our CEO's Name and Email address is; Lissa Millspaugh - <a href=""mailto:lissa.millspaugh@winergyinc.com"">lissa.millspaugh@winergyinc.com</a></p>
<p>Our CTO's Name and Email address is; Thomas Hansen - <a href=""mailto:thomas.hansen@winergyinc.com"">thomas.hansen@winergyinc.com</a></p>
<p>If your stuff is of 'geeky nature', it's probably best to send it to our CTO, or at least CC him in ... ;)</p>");
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.LoadAdministratorDashboard")]
        protected void Magix_Publishing_LoadAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            RaiseEvent("Magix.Publishing.LoadHeader");
            RaiseEvent("Magix.Publishing.LoadAdministratorMenu");

            LoadDashboard();

            LoadToolTip();
        }

        private void LoadToolTip()
        {
            Node node = new Node();
            node["Append"].Value = true;
            if (Page.Session["HasLoadedTooltipOfToday"] == null)
            {
                node["Text"].Value = ToolTip.Instance.Next(User.Current.Username);
                Page.Session["HasLoadedTooltipOfToday"] = true;
            }
            else
            {
                node["Text"].Value = ToolTip.Instance.Current(User.Current.Username);
            }
            node["ChildCssClass"].Value = "tool-tip";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.ToolTip",
                "content3",
                node);
        }

        private void LoadDashboard()
        {
            Node node = new Node();

            node["Container"].Value = "content3";
            node["FullTypeName"].Value = "Dashboard-Type-META";
            node["ReuseNode"].Value = true;
            node["ID"].Value = -1;
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "dashboard";

            node["WhiteListColumns"]["PagesCount"].Value = true;
            node["WhiteListColumns"]["TemplatesCount"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Name"]["Header"].Value = "Objects";
            node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["Header"].Value = "Count";
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 2;

            node["Type"]["Properties"]["PagesCount"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["PagesCount"]["Header"].Value = "Pages";
            node["Type"]["Properties"]["PagesCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditPages";
            node["Type"]["Properties"]["TemplatesCount"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["TemplatesCount"]["Header"].Value = "Templates";
            node["Type"]["Properties"]["TemplatesCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditTemplates";

            node["Object"]["ID"].Value = -1;
            node["Object"]["Properties"]["PagesCount"].Value = WebPage.Count.ToString();
            node["Object"]["Properties"]["TemplatesCount"].Value = WebPageTemplate.Count.ToString();

            node["DoNotRebind"].Value = true;

            // Getting plugins ...
            RaiseEvent(
                "Magix.Publishing.GetDataForAdministratorDashboard",
                node);

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        // This method has every single change to its input Node structure being overridable ...
        [ActiveEvent(Name = "Magix.Publishing.LoadAdministratorMenu")]
        protected void Magix_Publishing_LoadAdministratorMenu(object sender, ActiveEventArgs e)
        {
            if (!e.Params.Contains("Top"))
                e.Params["Top"].Value = 4;

            if (!e.Params.Contains("Height"))
                e.Params["Height"].Value = 17;

            if (!e.Params.Contains("Width"))
                e.Params["Width"].Value = 6;

            if (!e.Params.Contains("Caption"))
                e.Params["Caption"].Value = "Menu";

            if (!e.Params.Contains("CssClass"))
                e.Params["CssClass"].Value = "administrator-menu";

            if (!e.Params["Items"].Contains("Home"))
            {
                e.Params["Items"]["Home"]["Caption"].Value = "Dashboard ...";
                e.Params["Items"]["Home"]["Selected"].Value = true;
                e.Params["Items"]["Home"]["Event"]["Name"].Value = "Magix.Publishing.LoadAdministratorDashboard";
            }
            if (!e.Params["Items"].Contains("Publishing"))
            {
                e.Params["Items"]["Publishing"]["Caption"].Value = "Publishing";
            }
            if (!e.Params["Items"]["Publishing"]["Items"].Contains("Pages"))
            {
                e.Params["Items"]["Publishing"]["Items"]["Pages"]["Caption"].Value = "Pages ...";
                e.Params["Items"]["Publishing"]["Items"]["Pages"]["Event"]["Name"].Value = "Magix.Publishing.EditPages";
            }
            if (!e.Params["Items"]["Publishing"]["Items"].Contains("Templates"))
            {
                e.Params["Items"]["Publishing"]["Items"]["Templates"]["Caption"].Value = "Templates ...";
                e.Params["Items"]["Publishing"]["Items"]["Templates"]["Event"]["Name"].Value = "Magix.Publishing.EditTemplates";
            }

            // Putting plugins just beneath Publishing menu item ...
            RaiseEvent(
                "Magix.Publishing.GetPluginMenuItems",
                e.Params);

            if (!e.Params["Items"].Contains("Admin"))
            {
                e.Params["Items"]["Admin"]["Caption"].Value = "Admin";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("DBAdmin"))
            {
                e.Params["Items"]["Admin"]["Items"]["DBAdmin"]["Caption"].Value = "Database ...";
                e.Params["Items"]["Admin"]["Items"]["DBAdmin"]["Event"]["Name"].Value = "Magix.Publishing.ViewClasses";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("Explorer"))
            {
                e.Params["Items"]["Admin"]["Items"]["Explorer"]["Caption"].Value = "File system ...";
                e.Params["Items"]["Admin"]["Items"]["Explorer"]["Event"]["Name"].Value = "Magix.Publishing.ViewFileSystem";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("Roles"))
            {
                e.Params["Items"]["Admin"]["Items"]["Roles"]["Caption"].Value = "Roles ...";
                e.Params["Items"]["Admin"]["Items"]["Roles"]["Event"]["Name"].Value = "Magix.Publishing.EditRoles";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("Users"))
            {
                e.Params["Items"]["Admin"]["Items"]["Users"]["Caption"].Value = "Users ...";
                e.Params["Items"]["Admin"]["Items"]["Users"]["Event"]["Name"].Value = "Magix.Publishing.EditUsers";
            }
            if (!e.Params["Items"]["Admin"]["Items"].Contains("Log"))
            {
                e.Params["Items"]["Admin"]["Items"]["Log"]["Caption"].Value = "Log ...";
                e.Params["Items"]["Admin"]["Items"]["Log"]["Event"]["Name"].Value = "Magix.Publishing.ViewLog";
            }

            if (!e.Params["Items"].Contains("LogOut"))
            {
                e.Params["Items"]["LogOut"]["Caption"].Value = "Logout!";
                e.Params["Items"]["LogOut"]["Event"]["Name"].Value = "Magix.Core.UserLoggedOut";
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Menu.Slider",
                "content1",
                e.Params);
        }

        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["LogItemCount"].Value = true;
            e.Params["Type"]["Properties"]["LogItemCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["LogItemCount"]["Header"].Value = "Log Items";
            e.Params["Type"]["Properties"]["LogItemCount"]["ClickLabelEvent"].Value = "Magix.Publishing.ViewLog";
            e.Params["Object"]["Properties"]["LogItemCount"].Value = LogItem.Count.ToString();

            e.Params["WhiteListColumns"]["UserCount"].Value = true;
            e.Params["Type"]["Properties"]["UserCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["UserCount"]["Header"].Value = "Users";
            e.Params["Type"]["Properties"]["UserCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditUsers";
            e.Params["Object"]["Properties"]["UserCount"].Value = User.Count.ToString();

            e.Params["WhiteListColumns"]["RoleCount"].Value = true;
            e.Params["Type"]["Properties"]["RoleCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["RoleCount"]["Header"].Value = "Roles";
            e.Params["Type"]["Properties"]["RoleCount"]["ClickLabelEvent"].Value = "Magix.Publishing.EditRoles";
            e.Params["Object"]["Properties"]["RoleCount"].Value = Role.Count.ToString();
        }

        [ActiveEvent(Name = "Magix.Publishing.ViewLog")]
        private void Magix_Publishing_ViewLog(object sender, ActiveEventArgs e)
        {
            // Resetting counter
            HttpContext.Current.Session.Remove("LogCount");

            Node node = new Node();

            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["FullTypeName"].Value = typeof(LogItem).FullName;
            node["IsCreate"].Value = false;
            node["IsDelete"].Value = false;
            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "View";
            node["IDColumnValue"].Value = "View";
            node["IDColumnEvent"].Value = "Magix.Publishing.EditLogItem";

            node["WhiteListColumns"]["LogItemType"].Value = true;
            node["WhiteListColumns"]["LogItemType"]["ForcedWidth"].Value = 6;
            node["WhiteListColumns"]["Header"].Value = true;
            node["WhiteListColumns"]["Header"]["ForcedWidth"].Value = 10;

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "When";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["LogItemType"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Header"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Header"]["MaxLength"].Value = 60;

            node["GetObjectsEvent"].Value = "Magix.Publishing.GetLogItems";

            // 'Passing through' ...
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.Publishing.GetLogItems")]
        private void Magix_Publishing_GetLogItems(object sender, ActiveEventArgs e)
        {
            // All we need to do is to inject our own little sorting order ...
            e.Params["Criteria"]["C1"]["Name"].Value = "Sort";
            e.Params["Criteria"]["C1"]["Value"].Value = "When";
            e.Params["Criteria"]["C1"]["Ascending"].Value = false;

            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "DBAdmin.Data.GetContentsOfClass",
                e.Params);
        }

        [ActiveEvent(Name = "Magix.Publishing.EditLogItem")]
        protected void Magix_Publishing_EditLogItem(object sender, ActiveEventArgs e)
        {
            // Passing directly through if user is Admin and is browsing the
            // Object Database Manager ...
            int id = e.Params["ID"].Get<int>();
            RaiseEvent("Magix.Publishing.ViewLogItem", e.Params);
        }

        [ActiveEvent(Name = "Magix.Publishing.ViewLogItem")]
        protected void Magix_Publishing_ViewLogItem(object sender, ActiveEventArgs e)
        {
            // Getting the requested User ...
            LogItem l = LogItem.SelectByID(e.Params["ID"].Get<int>());

            e.Params["Width"].Value = 24;
            e.Params["Last"].Value = true;
            e.Params["ClearBoth"].Value = true;

            // First filtering OUT columns ...!
            e.Params["WhiteListColumns"]["LogItemType"].Value = true;
            e.Params["WhiteListColumns"]["When"].Value = true;
            e.Params["WhiteListColumns"]["Header"].Value = true;
            e.Params["WhiteListColumns"]["Message"].Value = true;
            e.Params["WhiteListColumns"]["ObjectID"].Value = true;
            e.Params["WhiteListColumns"]["ParentID"].Value = true;
            e.Params["WhiteListColumns"]["StackTrace"].Value = true;
            e.Params["WhiteListColumns"]["IPAddress"].Value = true;
            e.Params["WhiteListColumns"]["User"].Value = true;
            e.Params["WhiteListColumns"]["UserID"].Value = true;

            e.Params["WhiteListProperties"]["Name"].Value = true;
            e.Params["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 2;
            e.Params["WhiteListProperties"]["Value"].Value = true;
            e.Params["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 4;

            e.Params["Type"]["Properties"]["LogItemType"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["LogItemType"]["Bold"].Value = true;
            e.Params["Type"]["Properties"]["Header"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["UserID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["When"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["Message"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["ObjectID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["UserID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["ParentID"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["StackTrace"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["IPAddress"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["User"]["ReadOnly"].Value = true;

            if (!e.Params.Contains("Append"))
            {
                if (HttpContext.Current.Session["LogCount"] == null)
                {
                    HttpContext.Current.Session["LogCount"] = 0;
                }
                int val = (int)HttpContext.Current.Session["LogCount"];
                if (val % 2 != 0)
                {
                    e.Params["ChildCssClass"].Value = "mux-rounded mux-shaded span-10 prepend-top mux-paddings last";
                }
                else
                {
                    e.Params["ChildCssClass"].Value = "mux-rounded mux-shaded span-10 prepend-top mux-paddings";
                }
                e.Params["Append"].Value = val % 2 != 0;
                HttpContext.Current.Session["LogCount"] = (val + 1) % 2;
            }
            e.Params["Container"].Value = "content4";
            e.Params["Caption"].Value =
                string.Format(
                    "Editing LogItem: {0}",
                    l.Header);

            ActiveEvents.Instance.RaiseActiveEvent(
                sender,
                "DBAdmin.Form.ViewComplexObject",
                e.Params);
        }

        [ActiveEvent(Name = "Magix.Publishing.ViewClasses")]
        private void Magix_Publishing_ViewClasses(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["Caption"].Value = "Browsing Database of '" +
                Adapter.Instance.GetConnectionString() + "'";
            node["FontSize"].Value = 18;
            node["Lock"].Value = true;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.SetFormCaption",
                node);

            node = new Node();

            node["container"].Value = "child";
            node["Width"].Value = 19;
            node["WindowCssClass"].Value = "mux-shaded mux-rounded browser";
            node["Caption"].Value = "Browse classes";
            node["NoHeader"].Value = true;
            node["CloseEvent"].Value = "Magix.Publishing.ViewClassesClosed";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClasses",
                node);
        }
    }
}
