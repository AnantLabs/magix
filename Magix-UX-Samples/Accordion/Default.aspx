<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="AccordionSample" %>

<!DOCTYPE html>

<html>
    <head runat="server">
        <title>Untitled Page</title>
        <link rel="stylesheet" href="../media/blueprint/screen.css" type="text/css" media="screen, projection" />
        <link rel="stylesheet" href="../media/blueprint/print.css" type="text/css" media="print" />
        <!--[if lt IE 8]>
        <link rel="stylesheet" href="../media/blueprint/ie.css" type="text/css" media="screen, projection" />
        <![endif]-->

        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="container prepend-top">
                <div class="span-10 append-1">
                    <mux:Accordion 
                        runat="server" 
                        CssClass="mux-accordion"
                        ID="acc">
                        <mux:AccordionView 
                            runat="server" 
                            Caption="Random latin text"
                            ID="acc1">
                            <Content>
                                <div class="mux-insert">
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                                    Donec fermentum dignissim cursus. Duis eu velit eu magna 
                                    adipiscing posuere. Sed sit amet magna velit. Sed vel 
                                    nisl eu nibh tincidunt posuere. Vivamus quis lacus suscipit 
                                    velit egestas interdum. Sed scelerisque, quam ullamcorper 
                                    vestibulum vestibulum, eros nisl ornare metus, non laoreet 
                                    purus diam ut neque. Ut erat augue, malesuada ac cursus ac, 
                                    porta a nulla. Nulla ultricies orci ut ligula facilisis id 
                                    tempus turpis vestibulum. Morbi faucibus elit vitae nisi 
                                    condimentum ornare. Donec aliquet convallis pretium. Donec 
                                    dui enim, placerat nec ultricies eget, pellentesque ut magna. 
                                    Etiam vel quam et metus varius tristique. Proin eget ipsum 
                                    nec felis blandit semper eu nec felis. Morbi eget rhoncus quam. 
                                    Phasellus arcu sapien, semper a consectetur nec, iaculis 
                                    sit amet urna.
                                </div>
                            </Content>
                        </mux:AccordionView>
                        <mux:AccordionView 
                            runat="server" 
                            Caption="Controls inside"
                            ID="acc2">
                            <Content>
                                <div class="mux-insert">
                                    <mux:Button 
                                        runat="server" 
                                        ID="btn" 
                                        Text="Click me..." 
                                        OnClick="btn_Click" />
                                </div>
                            </Content>
                        </mux:AccordionView>
                        <mux:AccordionView 
                            runat="server" 
                            Caption="Some English Text"
                            ID="AccordionView1">
                            <Content>
                                <div class="mux-insert">
                                    <p>
                                        Hello there, this is an accordion. It is entirely 
                                        created out of textual CSS, and no images.
                                    </p>
                                    <p>
                                        Here are a couple of Paragraphs just to make sure
                                        we have some text here that uses space.
                                    </p>
                                </div>
                            </Content>
                        </mux:AccordionView>
                    </mux:Accordion>
                </div>
                <div class="span-13 last">
                    <h2>Accordion Sample</h2>
                    <p>
                        To the left you can see an "Accordion". Accordions are useful for creating
                        groupings, for instance within your navigation. Or when you need some sort
                        of ordered process, like for instance a wizard - at which an Accordion might 
                        be a viable alternative.
                    </p>
                </div>
            </div>
        </form>
    </body>
</html>
