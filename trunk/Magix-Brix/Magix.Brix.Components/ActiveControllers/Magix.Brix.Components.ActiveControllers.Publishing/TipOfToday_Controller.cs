/*
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
<h1>Welcome to Magix</h1>
<p>Magix is a platform for Creating Web Applications, either by using C#, VB.NET or other .Net languages, or through using one of the <em>No-Code-Required Application Creators</em>, which you can use to create Web Applications without coding.</p>
<p>I am Heka, the Anthropomorphized Soul of Magix Illuminate I. By using me as your guide through Magix, you can become friend with Magix faster.</p>
<p>In fact, your first Tip, is to use the Arrow Keys in the Top/Right corner of this window, to navigate forward and fast read the next upcoming 5-10 tips. They are important for getting started with Magix ...</p>
<p>If you need to re-read a previous tip, you can click the previous button ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Magix is ...</h2>
<p>Magix Illuminate has many features, abd trying to list them all, would demand many books. Though already
before you need to create anything yourself, Magix Illuminate is already extremely useful. You can for instance use Magix ...</p>
<ul>
<li>As your own personal DropBox server - By using the File Manager</li>
<li>To Syncronize your files between work and home</li>
<li>File Sharing with Friends and Colleagues</li>
<li>Cloud-Backup of your Important Files, such as Images</li>
<li>MP3 player, which will let you play your music from every device you wish</li>
<li>Gallery for friends and family</li>
<li>Vanity QR Code Generator</li>
<li>CMS or Publishing System</li>
<li>Micro Website CMS</li>
<li>Excel CSV File Manipulator</li>
</ul>
<p>All these things comes in addition to the endless possibilities you have for creating your own Applications, either without code, or by using any .Net CLR language.</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Creating Stuff</h2>
<p>Magix consists of two important pre-installed modules; The <em>'Publishing'</em> System and the <em>'MetaType'</em> System.</p>
<p>Publishing is where you go to create WebSites or to publish text, images and videos. MetaTypes is where you go to create Applications.</p>
<p>We recommend you to <em>start with learning the Publishing concepts first</em>. Then later, only when a firm grasp of Pages and Templates are understood, moving onto Applications. Although, if you have had much experience with Publishing Systems before, you might want to rush this part. The basic understanding of Pages should be easily acquired for a Savvy website owner relatively fast.</p>");
                TipOfToday.Instance.CreateTip(@"
<h2>Basics ...</h2>
<p>Before we can do anything, we need to learn the <em>Basics</em> ...</p>
<p>Most of Magix is made up of 'Basic Components', which are tied together to create a whole. For instance, a button will mostly look the same everywhere. By default a button will be Gray and use Blue, Big Fonts. A good example of a Button is the Top/Right corner of this tooltip, which has two Buttons. One Paging forward, and another paging Backwards in the Hierarchy of Tips and Tricks ...</p>
<p>A 'Grid' is when you see a list of items. A good example of a Grid would be Actions or Views. Grids are useful for displaying lists of information.</p>
<p>There are many types of basic components like these in Magix. Over the next couple of pages, we'll be walking through some of them which you'll need to understand to be able to get the <em>most out of Magix</em> ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Grids</h2>
<p>Most Grids in Magix have many features. These features includes paging, in place editing, filtering, etc. Open up <em>Actions</em> by clicking the icon at 2 O'Clock on your Dashboard.</p>
<p>To Filter according to a column, all you've got to do is to click the header of your Grid, choose which type of filter you want to apply, type in its value, and click OK. Try to filter your actions according to 'debug' by clicking the name header column. This should give you two actions, turn on and turn off debugging. Though some screens, such as the action screen, also have a 'fast filter' textbox. Make sure you remove your filter before you leave this screen again, by clicking the name column and remove the text and click OK.</p>
<p>The arrow buttons, normally at the bottom of your grids makes it possible to traverse forward and backwards in your list of items. The double arrows takes you <em>'all the way'</em> in its direction</p>
<p><em>Open up Objects/Actions</em> and play around with that grid by filtering, creating a couple of new items and so on.</p>
<p>Paging normally turns on automagixally according to when it's needed. To see the paging buttons, you'll normally need to have more than 10 items in your grid. If you don't see them in your Action screen, make sure all Filters and filter queries are turned OFF. To see 'all the way paging', you'll normally need more than 20 items ...</p>
<p>Make sure you also click the 'Edit' column. Sometimes this column will say 'Edit' while sometimes it'll show a number. However, clicking the Edit Column, will always somehow bring you to a View where that object can be edited in 'full version' ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Some things should be very similar for mostly all grids like this in Magix. For instance ...</p>
<p>Clicking the '+' button will almost always create a new object of that type ...</p>
<p>If the Text of a Grid Cell is Green, this means that you can edit the value directly by <em>clicking the Blue Text</em>, which will exchange it with a 'textbox', from where you can edit its value, and save the updated value by either clicking Carriage Return or somehow moving focus away from the textbox/textarea itself. For textarea (multiple lines textboxes) hitting the TAB key to update is usually the most efficient ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Publishing ...</h2>
<p>A Website consists of Pages. Every Page is the equivalent of one 'URL'. Although URLs doesn't really exist in Magix, it helps to think of a page as such. Beside, creating a URL based Navigation Plugin would be piece of cake anyway, due to the Architectural Principles Magix is built on ...</p>
<p>Anyway ...</p>
<p>You create your pages according to 'Templates', which can be seen as <em>'Recipes'</em> for your pages. You have to have at least <em>One Template</em> in your system before you can start creating Pages. Every Page is based upon a Template, and no page can exist without its Template ...</p>
<p>Click <em>Templates</em> at 3 O'Clock on your Dashboard now.</p>
<p>PS!<br />Click the Edit links to view any specific templates ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>Every Template contains a list of WebPart Templates. These WebPart Templates can be positioned exactly as you wish on your page. When editing a Template, use the <em>Arrow Buttons</em> to position your WebParts ...</p>
<p>If you click the Blue Name of the WebPart, you can further edit more properties of your WebParts. The most important property here is the <em>Module</em> property, which basically is the type of Publishing Plugin Module this specific WebPart is based upon.</p>
<p>Some of the more important types of Modules are</p>
<ul>
<li><em>MetaView_Form</em> - A Publishing Plugin-Host for your Meta Forms. What allows you to inject Meta Forms into your Pages</li>
<li><em>Content</em> - A Rich Text content plugin</li>
<li><em>Header</em> - Basically just encapsulates an H1 element</li>
<li><em>SliderMenu</em> - The Sliding Menu, which can be seen by default in both the back-web and the front-web of Magix</li>
<li><em>TopMenu</em> - And Alternative and more conventional Menu for creating Menu Hierarchies. 100% compatible with the Sliding Menu's Data Foundation</li>
</ul>
<p>But exactly which modules you've got in your installation, may vary according to what modules you've installed, and how you've modified it yourself after you installed Magix Illuminate.</p>
<p>Another important property is the <em>WebPart CSS Class</em>, which also can be manipulated using the <em>CSS Template</em> SelectList. The <em>Last</em> property should only be set for WebParts which are supposed to expand all the way to the right of your Viewport. While the <em>Overflow</em> is for dynamic heights, where you don't know the final height of your module's content</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>When positioning your WebPart Templates, realize that you're not really 'positioning' them, but rather you are changing their <em>width, height and margins</em>. In the beginning this might feel a little bit cumbersome, though after some time you'll hopefully appreciate this 'floating layout' and become used to it ...</p>
<p>Realize also that especially the bottom and right margins might create funny looking WebParts since they're not really visible while editing. If you're having weird results, make sure your right and bottom margins are 0 by <em>double clicking</em> them, which should set them back to zero ...</p>
<p>Double clicking any of the arrows will either maximize or minimize their associated property ...</p>
<p>As previously said, realize that you're not really positioning your WebParts when you click the Arrow Keys. What you're doing, is that you are setting the <em>Margins</em> and the <em>Width</em> and <em>Height</em> of your WebPart. Though both the left margins and the top margins can have <em>negative values</em>, which basically is the facilitator to become virtually completely 100% enabled to set your exact positioning, without residing to absolute positioning.</p>
<p>A little bit more cumbersome, but definitely worth it.</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>The Grid Layout System</h2>
<p>When you move things around, either by using CSS or some other helper methods, such as the Arrow Buttons in the Template Editor, then you are basically conforming to a Grid system which Magix has inherited from Bluegrid.Css, which it uses at its core.</p>
<p>This system basically is 950 pixels wide, which happens to be 'the perfect' resolution for all monitors and devices, including phones and tablets.</p>
<p>These 950 pixels is divided into 24 columns, where every column is 30 pixels wide, and have a margin of 10 pixels. Meaning if you're covering one column, your WebPart is 30 pixel wide. If it's covering 2 columns it's 70 pixels, 3 columns 110 pixels, and so on. Pluss 40, all the way to 24 and 950 pixels.</p>
<p>Every row is 18 pixels tall. Some things such as Buttons, by default, will cover two rows, meaning 18x2, which becomes 36 pixels.</p>
<p>When you move your WebParts, what you are really doing, is that you're adding and removing left and top margins of 40 and 18 pixels for every 'movement' you create. This allows for the perfect reading layout that Magix Illuminate comes with out of the box. Also for your own apps and pages.</p>
<p>If you wish to override this, you can do so either using CSS on the WebPart and WebPage level, or you can change the absolute positioning by allowing Absolute Positioning in the Meta Form designer for instance. Though this is generally not 'encouraged', since it destroys your future Agility.</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>A WebPage Template must contain  at least one WebPart Template. A WebPart Template is also a 'Type Definition' for your WebParts. WebPart Templates have names such as 'Content' and 'Header', which are publishing modules for showing large letters and rich text fragments. Every WebPart Template is based upon one plugin type. </p>
<p>Meaning if you have one page, based upon a WebPage Template, with 5 WebPart Templates, you'll have a WebPage with 5 WebParts where each WebPart can be different '<em>Applications'</em>. This might be any combination of Applications, such as Text Fragments, Headers, CRUD Forms and such ...</p>
<p>However, we'll stick to 'Publishing' as we promised in the beginning, and focus on the Publishing Modules ...</p>
<p>If you edit the default Template created by the system for you, you can see how it has Menu, Header and Content as <em>'Module Names'</em>. You can also create as many new Templates as you wish.</p>
<p>If you open your Dashboard now, and click 'Pages' at 1 O'Clock, you'll come to your WebPages Editing Screen. Click the top one and see how it's built upon the Template called 'M+H+C', which means Menu + Header + Content btw.</p>
<p>Every time you create a new WebPage, which is a single adressable URL in your system, you'll have to decide which Template to base your Page upon. Which again defines your design, type of applications this specific WebPage can show, etc.</p>
");
                TipOfToday.Instance.CreateTip(@"
<p>The Menu is similar to the Sliding Menu to the left which you're using yourself, while the Header will show an H1 HTML element [Header Element] and the Content module will show Rich Editable Content.</p>
<p>If you have more types of Modules in your installation of Magix, these might also show up as selections in the DropDownBoxes visible while editing your Templates ...</p>
<p>Now go to Templates and change the Name of Template 'M+H+C'. Change it to 'Testing'. This can be done by clicking directly on the text where it says 'M+H+C'. Then open up 'Pages ...' and click your root page.</p>
<p>In your Pages view now, can you see how the name of the module in the DropDown box, roughly at the middle of the screen has changed now to 'Testing'. This is because that DropDown box is being used to select a Template for your WebPage.</p>
<p>Now try to change the Name of your Templates by clicking e.g. Header while editing your Template, and type in 'Header2'. Now edit your Page and see how this change reflects from the Template and to the Page.</p>
<p>So to conclude, one WebPage is based upon a Template, which again contains several WebParts, where every WebPart is based upon a Module Type. The Page defines your content, and the Template your design, and 'type' of WebPage.</p>
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
<h2>Initialization of WebParts</h2>
<p>WebParts initializes differently depending upon where you're coming from, and from which Template type you're coming from dependent upon to which Template type you're entering. And also according to which type of WebParts, or Modules they are ... </p>
<p>For instance the sliding menu will not reload as long as the container it is within on one page template, is the same container it is within on the next page template ...</p>
<p>Too confusing ...?</p>
<p>Just remember; <em>always have a Menu</em> positioned in the same container [first one for instance?] on all of your templates, unless you really know what you're doing ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<p><em>Play around</em> with the system by creating some new Templates, copying them, change their weparts type between Header, Content and SliderMenu. Then when you come back, we'll start diving into Applications ...</p>
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
    <li>Magix.DynamicEvent.SaveActiveForm</li>
    <li>Magix.DynamicEvent.EmptyAndClearActiveForm</li>
</ul>
<p>The first Action will save your form, while the second one will empty it.</p>
<p>Now try to <em>View your form in preview</em> mode, and test it out by typing in your email and name, and clicking Submit to save your Object ...</p>
<p>PS!<br/>
Obviously it's crucial that the 'Save' action runs before the 'Empty' action, in case you wondered ... ;)</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Type Names</h2>
<p>If you take a look at your <em>Meta Objects</em> now you will see a new object with the Type Name of 'EmailSubscription'. The <em>'Type Name'</em> property from your MetaView decides the Type Name of your Objects ...</p>
<p>These 'Type Names' are important to distinguish from different types of Objects.</p>
<p>One Object might be of type 'Customer', while another object might be of type 'Email', and so on. What names you give your Types is <em>crucial</em>! Name clashes here might create very hard to track down bugs and such ...</p>
<p>Take some care when naming your Types!</p>
<p>It's probably a good practice to some how make sure they've got unique names, also across your organization if you want to use plugins made by others.</p>
<p>We encourage people to use type names such as; ""CompanyName.Department.Customer"", and never 'Customer' directly. In fact, your <em>homework</em> for this lesson is to go and rename your 'Customer' TypeName, and rename the Type Name to; 'CompanyName.Department.Customer'. Where Company Name and Department are <em>your</em> company name and your department ...</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Expressions</h2>
<p>There are several places where you will need to understand Expressions. And expression starts out with '{' and ends with '}'.</p>
<p>The purpose of an Expression is to de-reference something within your existing Node structure. Every Module and Action has a Node associated with it, which can either be statically, or dynamically built. This Node can be seen as the action/module data source.</p>
<p>This statement; 'root[Customer][Name].Value' means you will de-references the Value of whatever is inside the Root node of either your action or your module, within its Name node, which should be within the Customer node on root level. If this value, or these nodes doesn't exist somehow, a null value will be returned.</p>
<p>If you start your expressions with 'root', it will traverse all the way upwards until it finds the top outer most node. Otherwise it will always expect to find its node relatively according to where it is while traversing</p>
");
                TipOfToday.Instance.CreateTip(@"
<h2>Drag and Drop Files</h2>
<p>There are many places in Magix where you can Drag and Drop files. One such example would be the File Manager. If you open up the File Manager, you can find the folder you wish to upload your file(s) to, then find the files on your Desktop System, and actually drag and drop the files you wish to upload to this folder on your server. Magix will take care of the rest.</p>
<p>If you want to use these features in your own apps, they are possible for you to use by for instance adding a WebPart to your Template and set its type to <em>Uploader</em>.</p>
<p></p>
<p></p>
<p></p>
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
