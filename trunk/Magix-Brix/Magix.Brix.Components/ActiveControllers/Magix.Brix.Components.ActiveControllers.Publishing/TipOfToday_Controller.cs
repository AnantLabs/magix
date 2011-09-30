﻿/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
    /**
     * Level2: Creats our default Tooltips or Tutorial rather
     */
    [ActiveController]
    public class TipOfToday_Controller : ActiveController
    {
        #region [ -- Creation of a 'gazillion' tooltips during ApplicationStartup ... -- ]

        /**
         * Level2: Creates our default tooltips if there exists none
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            if (TipOfToday.Instance.Count == 0)
            {
                TipOfToday.Instance.CreateTip(@"
<h1>Welcome to Magix!</h1>
<p>Truly a Strange and Wonderful World ...</p>
<p>A place where you can become literate in regards to computers. A place where you can express yourself, creating what you want out of your computer. A place where you are in charge, a place which is fun!</p>
<p>This is your First tip of the day. <em>Leave These Tips On</em> to make sure you get Useful Tips and Tricks as you proceed deeper and deeper into the Rabbit Hole ...</p>
<p>In fact, your first Tip is to use the Arrow Keys in the Top/Right corner of this window to navigate forward and fast read the next upcoming 5-10 tips. They are all crucial for getting started with Magix ...</p>
<p>If you need to re-read a previous tip, you can click the previous button ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>4 sec Intro</h2>
<p>Magix consists of two majorly important pre-installed modules; The <em>'Publishing'</em> System and the <em>'MetaType'</em> System.</p>
<p>Publishing is where you go to create your WebSite, and MetaTypes is where you go to create Applications. Though, really the difference is more blurry than you think. For instance; It is difficult to show any Applications to your end users, if you do not have any Pages to 'host' your Application ... </p>
<p>So Pages can be thought of as Views in your Applications if you want to, or your entire hierarchy of Pages can be viewed as a gigantic plugin Application, which it actually in fact is ... ;)</p>
<p>It is actually quite useful to <em>stop separating</em> between 'Old-Time Constructs' such as 'code', 'data', 'input' and 'output'. </p>
<p>Old World thinking, trying to categorize things into different types, are much less useful in Magix than what you think. In Magix, everything is kind of 'mushy'. Or <em>'everything is everything'</em> I guess you can say. This is what makes it possible for you to Stay in Control and deliver Secure and Stable Systems, regardless of the Complexity of your Domain Problem ...</p>
<p>We recommend people to <em>Start with Learning Publishing</em> and how the WebPages work. Then later, only when a firm grasp of Pages and Templates are understood, we recommend moving onto Applications ...</p>");
                TipOfToday.Instance.CreateTip(@"
<h2>Basics ...</h2>
<p>But before we can do anything else, we need to learn the <em>Basics</em> ...</p>
<p>Most of Magix is made up of 'Basic Components', which are tied together to create a whole.</p>
<p>For instance, a button will mostly look the same everywhere. By default a button will be Gray and use Bold, Black and Big Fonts ...</p>
<p>A good example of a Button is the Top/Right corner of this tooltip, which has two Buttons. One Paging forward, and another paging Backwards in the Hierarchy of Tips and Tricks ...</p>
<p>A 'Grid' is when you see a list of items. A good example of a Grid would be MetaTypes/Meta Actions ...</p>
<p>There are many types of 'Basic Components' like these in Magix. Over the next couple of pages, we'll be walking through some of them which you'll need to understand to be able to get the <em>most out of Magix</em> ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Grids</h2>
<p>Most Grids in Magix have tons of features. These features includes; Paging, In Place Editing of Values, Filtering, and so on ...</p>
<p>To Filter according to a Column, all you've got to do is to click the header of your Grid, choose which type of Filter you want to apply, type in its value, and click OK ...</p>
<p>The arrow buttons, normally at the bottom of your grids makes it possible to traverse forward and backwards in your list of items. The double arrows takes you <em>'all the way'</em> in its direction ...</p>
<p><em>Open up Meta Types/Meta Actions</em> and play around with that grid by filtering, creating a couple of new items and so on.</p>
<p>Make sure you <em>don't change any</em> of the existing items, since some things are dependent upon 'System Actions' which must be defined for your system to properly work ...!</p>
<p>To see Paging you'll normally need to have more than 10 items in your grid. To see 'all the way paging', you'll normally need more than 20 items ...</p>
<p>Make sure you also click the 'Edit' column. Sometimes this column will say 'Edit' while sometimes it'll show a number like in the Action view. However, clicking the Edit Column, will always somehow bring you to a View where that object can be edited in 'full version' ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Some things should be very similar for mostly all grids like this in Magix. For instance ...</p>
<p>Clicking the '+' button will almost always create a new object of that type ...</p>
<p>If the Text of a Grid Cell is Blue, this means that you can edit the value directly by <em>clicking the Blue Text</em>, which will exchange it with a 'textbox', from where you can edit its value ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Publishing ...</h2>
<p>A Website consists of Pages. Every Page is the equivalent of one 'URL'. Although URLs doesn't really exist in Magix, it helps to think of a page as such. Beside, creating a URL based Navigation Plugin would be piece of cake anyway, due to the Architectural Principles Magix is built on ...</p>
<p>Anyway ...</p>
<p>You create your pages according to 'Templates', which can be seen as <em>'Recipes'</em> for your pages. You have to have at least <em>One Template</em> in your system before you can start creating Pages. Every Page is based upon a Template, and no page can exist without its Template ...</p>
<p>Click <em>'Publishing-&gt;Templates ...'</em> now!</p>
<p>If you click 'Dashboard' at the root of your menu you will return back here. To access the root of your menu, click the left arrow at the top of your Sliding Menu ...</p>
<p>PS!<br />Click the Edit links to view any specific templates ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Every Template contains a bunch of WebPart Templates. These WebPart Templates can be positioned exactly as you wish on your page. When editing a Template, use the <em>Arrow Buttons</em> to position your WebParts ...</p>
<p>Go check out <em>'Publishing-&gt;Templates ...'</em> one more time, and see how you can move stuff around on your 'surface' by clicking the Arrow Buttons ... </p>
");
                TipOfToday.Instance.CreateTip(@"
<p>When positioning your WebPart Templates, realize that you're not really 'positioning' them, but rather you are changing their <em>width, height and margins</em>. In the beginning this might feel a little bit cumbersome, though after some time you'll hopefully appreciate this 'floating layout' and become used to it ...</p>
<p>Realize also that especially the bottom and right margins might create funny looking WebParts since they're not really visible while editing. If you're having weird results, make sure your right and bottom margins are 0 by <em>double clicking</em> them, which should set them back to zero ... ;)</p>
<p>Double clicking any of the arrows will either maximize or minimize their associated property ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>A WebPage Template must contain  at least one WebPart Template. A WebPart Template is also a 'Type Definition' for your WebParts. WebPart Templates have names such as 'Content' and 'Header', which are publishing modules for showing large letters and rich text fragments. Every WebPart Template is based upon one plugin type. </p>
<p>Meaning if you have one page, based upon a WebPage Template, with 5 WebPart Templates, you'll have a WebPage with 5 WebParts where each WebPart can be different '<em>Applications'</em>. This might be any combination of Applications, such as Text Fragments, Headers, CRUD Forms and such ...</p>
<p>However, we'll stick to 'Publishing' as we promised in the beginning, and focus on the Publishing Modules ...</p>
<p>If you edit the default Template created by the system for you, you can see how it has Menu, Header and Content as <em>'Module Names'</em></p>
<p>Go check it out while I hang around here ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>The Menu is similar to the Sliding Menu to the left which you're using yourself, while the Header will show an H1 HTML element [Header Element] and the Content module will show Rich Editable Content.</p>
<p>If you have more types of Modules in your installation of Magix, these might also show up as selections in the DropDownBoxes visible while editing your Templates ...</p>
<p>No go to Templates and change the Name of Template 'M+H+C'. Change it to 'Testing'. This can be done by clicking directly on the text where it says 'M+H+C'. Then open up 'Pages ...' and click your root page.</p>
<p>Do you see how the name of the module in the DropDown box, roughly at the middle of the screen has changed now to 'Testing'. This is because that DropDown box is being used to select a Template for your WebPage.</p>
<p>Then try to change the Name of your Templates by clicking e.g. Header while editing your Template, and type in 'Header2'. Now edit your Page and see how this change reflects from the Template and to the Page.</p>
<p>Go take a look, while I chill ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p><em>Important:</em> If you change the number of WebParts in your Template, or you change the type of Module of your WebPart Template, then <em>all pages</em> built upon that Template will have to be resaved, and you'll probably loose data. </p>
<p>It is therefor important that you create your templates first, and then don't edit these two properties while they're already in 'Production' ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Try to Create several different Templates, and have slightly different values for their width, height, margins and such.</p>
<p>Be certain of that you've added different widths of your Menu Containers and different <em>Left Margins</em></p>
<p>Make sure they've got the same type of modules in the same container</p>
<p>Then use these different Templates for different pages which you create in your Pages hierarchy</p>
<p>If you now access the root of your website, and try to browse around by clicking different buttons, you can see how the WebPart Containers are 'jumping around' on the screen ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Init. of WebParts</h2>
<p>WebParts initializes differently depending upon where you're coming from, and from which Template type you're coming from dependent upon to which Template type you're entering. And also according to which type of WebParts, or Modules they are ... </p>
<p>For instance the sliding menu will not reload as long as the container it is within on one page template, is the same container it is within on the next page template ...</p>
<p>Too confusing ...?</p>
<p>Just remember; <em>always have a Menu</em> positioned in the same container [first one for instance?] on all of your templates, unless you really know what you're doing ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p><em>Play around</em> with the system by creating some new Templates, copying them, change their weparts type between Header, Content and SliderMenu. Then when you come back, we'll start diving into Applications ... ;)</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Applications ...</h2>
<p>Most applications will be WebParts. This means that you can inject them into any WebPart onto any WebPage you wish.</p>
<p>Let's create an Application ... :)</p>
<p>First make sure you have one Template in use in one of your pages which has one WebPart Template with the Module Type of <em>'MetaView_Single'</em> ...</p>
<p>Then click on <em>'MetaTypes/Meta Views'</em> ...</p>
<p>Create a new View called 'CollectEmails' ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Add three properties to your form, name them</p>
<ul>
    <li>Name</li>
    <li>Email</li>
    <li>Subscribe</li>
</ul>
<p>Change their description to something meaningful ...</p>
<p>Make sure you change the 'Type Name' of your object to <em>'EmailSubscription'</em></p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Attach two Actions to your 'Save' property.</p>
<ul>
\t<li>Magix.DynamicEvent.SaveActiveForm</li>
\t<li>Magix.DynamicEvent.EmptyActiveForm</li>
</ul>
<p>The first Action will save your form, while the second one will empty it.</p>
<p>Now try to <em>View your form in preview</em> mode, and test it out by typing in your email and name, and clicking Submit to save your Object ...</p>
<p>PS!<br/>
Obviously it's crucial that the 'Save' action runs before the 'Empty' action, in case you wondered ... ;)</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>TypeNames ...</h2>
<p>If you take a look at your <em>Meta Objects</em> now you will see a new object with the Type Name of 'EmailSubscription'. The <em>'Type Name'</em> property from your MetaView decides the Type Name of your Objects ...</p>
<p>These 'Type Names' are important to distinguish from different types of Objects.</p>
<p>One Object might be of type 'Customer', while another object might be of type 'Email', and so on. What names you give your Types is <em>crucial</em>! Name clashes here might create very hard to track down bugs and such ...</p>
<p>Take some care when naming your Types!</p>
<p>It's probably a good practice to some how make sure they've got unique names, also across your organization if you want to use plugins made by others.</p>
<p>We encourage people to use type names such as; ""CompanyName.Department.Customer"", and never 'Customer' directly. In fact, your <em>homework</em> for this lesson is to go and rename your 'Customer' TypeName, and rename the Type Name to; 'CompanyName.Department.Customer'. Where Company Name and Department are <em>your</em> company name and your department ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>OpenID</h2>
<p>Did you know that you can use Magix as both a Relying Party and OpenID Provider?</p>
<p>[ Read more about Open ID <a href=""http://openid.net/"" target=""_blank"">here</a> ... ]</p>
<p>If you need an OpenID token to log into some website somewhere, then you can append ?openID=admin after the root URL to your website, if the admin user is the User you'd like to log in with.</p>
<p>[ Full example; yourdomain.com/?openID=admin ]</p>
<p>This will redirect from the website you're trying to log into, and back to your website, which in turn will ask you for admin's password.</p>
<p>Once successfully logged into your own Magix website, your website will redirect back to the website you're trying to log into, and tell it that it 'has prof of that you are who you claimed you were'.</p>
<p>You can also associate other OpenID tokens, such as your Yahoo account or Blogger account, with your Magix User internally. This will allow you to log into Magix with that OpenID Account.</p>
<p>If your blogger account is 'magix' for instance, then your OpenID 'username' [claim] would become 'magix.blogspot.com'</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Talk to CEO</h2>
<p>If you make something cool with Magix, that you want to share, then we'd love to get to know about it. It can be an instructional YouTube video about how to Get Started, Tips and Tricks etc. It can be a Tutorial Blog you've written about how to install Magix on your server. It can be a book you have written about the O2 Architecture. It can be an Action, or a collection of Actions, which you think is awesome. Anything really! </p>
<p>As long as it has Value for our Community somehow, we'd love to know about it, so that we could help you promote it, and more importantly; Helping our Community out ... :)</p>
<p>Our CEO's Name and Email address is; Lissa Millspaugh - <a href=""mailto:lissa.millspaugh@winergyinc.com"">lissa.millspaugh@winergyinc.com</a></p>
<p>Our CTO's Name and Email address is; Thomas Hansen - <a href=""mailto:thomas.hansen@winergyinc.com"">thomas.hansen@winergyinc.com</a></p>
<p>If your stuff is of 'geeky nature', it's probably best to send it to our CTO, or at least CC him in ... ;)</p>");
            }
        }

        #endregion
    }
}
