<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs" 
    Inherits="CarouselSample" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Untitled Page</title>
        <link href="../media/skins/default/default.css" rel="stylesheet" type="text/css" />
        <style type="text/css">
img
{
	display:block;
	margin:0px auto 5px auto;
}
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <div style="padding:15px;">
                <mux:Window
                    runat="server"
                    style="width:500px;position:absolute;top:25px;left:25px;height:350px;" 
                    CssClass="mux-shaded mux-rounded mux-window" 
                    Caption="Carousel Example"
                    id="wnd">
                    <mux:Carousel
                        runat="server"
                        style="height:200px;position:relative;width:470px;overflow:visible;font-size:8px;"
                        id="car">
                        <div class="item" id="car1">
                            <div style="padding:15px;">
                                <img src="../media/images/ajax.jpg" />
                                This is our first Carousel.
                            </div>
                        </div>
                        <div class="item" id="car2">
                            <div style="padding:15px;">
                                <img src="../media/images/god1.jpg" />
                                This is another Carousel.
                            </div>
                        </div>
                        <div class="item" id="car3">
                            <div style="padding:15px;">
                                <img src="../media/images/god2.jpg" />
                                Ipsum, lorim perfectum, braderium.
                            </div>
                        </div>
                        <div class="item" id="car4">
                            <div style="padding:15px;">
                                <img src="../media/images/god3.jpg" />
                                Visste du at det er flere tannkremtuber?
                            </div>
                        </div>
                        <div class="item" id="car5">
                            <div style="padding:15px;">
                                <img src="../media/images/god4.jpg" />
                                Hvor mange toefler gaar det paa ett soverom
                            </div>
                        </div>
                        <div class="item" id="car6">
                            <div style="padding:15px;">
                                <img src="../media/images/god5.jpg" />
                                Hvordan uttales oxo?
                            </div>
                        </div>
                        <div class="item" id="car7">
                            <div style="padding:15px;">
                                <img src="../media/images/god6.jpg" />
                                Det er flere multebaer i blaamyra!
                            </div>
                        </div>
                        <div class="item" id="car8">
                            <div style="padding:15px;">
                                <img src="../media/images/ajax.jpg" />
                                Kanskje kommer kongen?
                            </div>
                        </div>
                        <div class="item" id="car9">
                            <div style="padding:15px;">
                                <img src="../media/images/god1.jpg" />
                                There's a tree in our forrest...
                            </div>
                        </div>
                        <div class="item" id="car10">
                            <div style="padding:15px;">
                                <img src="../media/images/god2.jpg" />
                                Heisann paa Teisann ... :)
                            </div>
                        </div>
                        <div class="item" id="car11">
                            <div style="padding:15px;">
                                <img src="../media/images/god3.jpg" />
                                Titten tei er en all right fyr ...
                            </div>
                        </div>
                        <div class="item" id="car12">
                            <div style="padding:15px;">
                                <img src="../media/images/god4.jpg" />
                                Husker du; Husker du ...?
                            </div>
                        </div>
                        <div class="item" id="car13">
                            <div style="padding:15px;">
                                <img src="../media/images/god5.jpg" />
                                Det finnes flere morsomme fyrer i fem fyrer med ved
                            </div>
                        </div>
                        <div class="item" id="car14">
                            <div style="padding:15px;">
                                <img src="../media/images/god6.jpg" />
                                Hvor mange saapekopper er det plass til ...?
                            </div>
                        </div>
                        <div class="item" id="car15">
                            <div style="padding:15px;">
                                <img src="../media/images/ajax.jpg" />
                                Heisann
                            </div>
                        </div>
                        <div class="item" id="car16">
                            <div style="padding:15px;">
                                <img src="../media/images/god1.jpg" />
                                Tjobing
                            </div>
                        </div>
                        <div class="item" id="car17">
                            <div style="padding:15px;">
                                <img src="../media/images/god2.jpg" />
                                Inger er fin
                            </div>
                        </div>
                        <div class="item" id="car18">
                            <div style="padding:15px;">
                                <img src="../media/images/god3.jpg" />
                                Thomas er glad i Inger
                            </div>
                        </div>
                        <div class="item" id="car19">
                            <div style="padding:15px;">
                                <img src="../media/images/god4.jpg" />
                                Titten tei :)
                            </div>
                        </div>
                        <div class="item" id="car20">
                            <div style="padding:15px;">
                                <img src="../media/images/god5.jpg" />
                                Thomas var her ...
                            </div>
                        </div>
                        <div class="item" id="car21">
                            <div style="padding:15px;">
                                <img src="../media/images/god6.jpg" />
                                Somewhere, over the rainbow...
                            </div>
                        </div>
                        <div class="item" id="car22">
                            <div style="padding:15px;">
                                <img src="../media/images/ajax.jpg" />
                                There's a hole in my bucket!
                            </div>
                        </div>
                        <div class="item" id="car23">
                            <div style="padding:15px;">
                                <img src="../media/images/god1.jpg" />
                                Can't see the forrest for all the trees
                            </div>
                        </div>
                        <div class="item" id="car24">
                            <div style="padding:15px;">
                                <img src="../media/images/god2.jpg" />
                                iPads rock!
                            </div>
                        </div>
                        <div class="item" id="car25">
                            <div style="padding:15px;">
                                <img src="../media/images/god3.jpg" />
                                Better to die standing, than to live on your knees
                            </div>
                        </div>
                    </mux:Carousel>
                </mux:Window>
            </div>
        </form>
    </body>
</html>
