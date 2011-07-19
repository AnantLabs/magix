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

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class AdministratorDashboardController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            // TODO: Throw away while testing ToolTip functionality ....
            foreach (ToolTip.Tip idx in ToolTip.Tip.Select())
            {
                idx.Delete();
            }
            foreach (ToolTip.TipPosition idx in ToolTip.TipPosition.Select())
            {
                idx.Delete();
            }
            if (ToolTip.Instance.Count == 0)
            {
                ToolTip.Instance.CreateTip(@"
<h2>Welcome to Magix!</h2>
<p>Truly a Strange and Wonderful World ...</p>
<p>This is your First tip of the day. <em>Leave These Tips On</em> to make sure you get Useful Tips and Tricks as you proceed deeper and deeper into the Rabbit Hole ...</p>
<p>In fact, your first Tip is to use the Arrow Keys in the Top/Right corner of this window to navigate forward and fast read the next upcoming 3 tips. They are all crucial for getting started with Magix ...</p>
<p>If you need to re-read a previous tip, you can click the previous button ...</p>
");
                ToolTip.Instance.CreateTip(@"
<p>Magix consists of two majorly important pre-installed modules; The <em>'Publishing'</em> System and the <em>'MetaType'</em> System.</p>
<p>Publishing is where you go to create your WebSite, and MetaTypes is where you go to create Applications. Though, really the difference is more blurry than you think. For instance; It is difficult to show any Applications to your end users, if you do not have any Pages to 'host' your Application ... </p>
<p>So Pages can be thought of as Views in your Applications if you want to, or your entire hierarchy of Pages can be viewed as a gigantic plugin Application, which it actually in fact is ... ;)</p>
<p>It is actually quite useful to <em>stop separating</em> between 'Old-Time Constructs' such as 'code', 'data', 'input' and 'output'. </p>
<p>Old World thinking, trying to categorize things into different types, are much less useful in Magix than what you think. In Magix, everything is kind of 'mushy'. Or <em>'everything is everything'</em> I guess you can say. This is what makes it possible for you to Stay in Control and deliver Secure and Stable Systems, regardless of the Complexity of your Domain Problem ...</p>
<p>Anyway ...</p>
<p>We recommend people to <em>Start with Learning Publishing</em> and how the WebPages work. Then later, only when a firm grasp of Pages and Templates are understood, we recommend moving onto Applications ...</p>");
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
<p>When positioning your WebPart Templates, realize that you're not really 'positioning' them, but rather you are changing their <em>width, height and margins</em>. In the beginning this might feel a little bit cumbersome, though after some time you'll hopefully appreciate this 'floating layout' and become used to is ...</p>
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
<p>Make especially care of that you've added different widths of your Menu Containers and different <em>Left Margins</em></p>
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
<h1>Applications ...</h1>
<p></p>
<p></p>
<p></p>
<p></p>
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
            node["Type"]["Properties"]["TemplatesCount"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["TemplatesCount"]["Header"].Value = "Templates";

            node["Object"]["ID"].Value = -1;
            node["Object"]["Properties"]["PagesCount"].Value = WebPage.Count.ToString();
            node["Object"]["Properties"]["TemplatesCount"].Value = WebPageTemplate.Count.ToString();

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
