<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="AccordionSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <mux:Accordion 
                runat="server" 
                style="width:450px;display:table;margin-left:auto;margin-right:auto;"
                ID="acc">
                <mux:AccordionView 
                    runat="server" 
                    Caption="Random latin text"
                    ID="acc1">
                    <Content>
                        <div style="padding:15px;">
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
                        <div style="padding:15px;">
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
                        <div style="padding:15px;">
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
        </form>
    </body>
</html>
