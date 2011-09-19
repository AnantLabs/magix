<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.QRGenerator" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.QRGenerator.Generator" %>

<link href="media/modules/qr-generator.css" rel="stylesheet" type="text/css" />

<mux:Panel
    runat="server"
    id="whole"
    CssClass="last span-18 height-17 qr-generator">
    <div class="mux-multi-panel-wrapper">
        <mux:TabStrip 
            runat="server" 
            MultiPanelID="mp"
            OnMultiButtonClicked="mp_MultiButtonClicked"
            ID="mub">
            <mux:TabButton 
                runat="server" 
                Text="1. Data"
                ID="mb1" />
            <mux:TabButton
                runat="server" 
                Text="2. Design"
                ID="mb2" />
            <mux:TabButton
                runat="server" 
                Text="3. Download"
                ID="mb3" />
        </mux:TabStrip>
        <mux:MultiPanel 
            runat="server" 
            AnimationMode="Slide"
            ID="mp">
            <Content>
                <mux:MultiPanelView 
                    runat="server"
                    DefaultWidget="next1"
                    ID="mpv1">
                    <div class="mux-insert first">
                        <mux:Panel
                            runat="server"
                            style="display:none;"
                            id="mecardPnl">
                            <div class="span-7">
                                Description
                            </div>
                            <div class="span-7 last">
                                Name
                            </div>
                            <div class="span-7">
                                <mux:TextBox
                                    runat="server"
                                    id="desc2"
                                    style="margin-bottom:0;"
                                    PlaceHolder="Description ..."
                                    CssClass="span-7 last" />
                            </div>
                            <div class="span-7 last">
                                <mux:TextBox
                                    runat="server"
                                    id="name"
                                    style="margin-bottom:0;"
                                    PlaceHolder="Full Name ..."
                                    CssClass="span-7 last" />
                            </div>
                            <div class="span-7">
                                Phone No.
                            </div>
                            <div class="span-7 last">
                                Email
                            </div>
                            <div class="span-7">
                                <mux:TextBox
                                    runat="server"
                                    id="phone"
                                    style="margin-bottom:0;"
                                    PlaceHolder="Phone ..."
                                    CssClass="span-7 last" />
                            </div>
                            <div class="span-7 last">
                                <mux:TextBox
                                    runat="server"
                                    id="email"
                                    style="margin-bottom:0;"
                                    PlaceHolder="Email ..."
                                    CssClass="span-7 last" />
                            </div>
                            <div class="span-7">
                                WebSite
                            </div>
                            <div class="span-7 last">
                                Address
                            </div>
                            <div class="span-7">
                                <mux:TextBox
                                    runat="server"
                                    id="website"
                                    PlaceHolder="URL ..."
                                    CssClass="span-7 last" />
                            </div>
                            <div class="span-7 last">
                                <mux:TextBox
                                    runat="server"
                                    id="address"
                                    PlaceHolder="Address ..."
                                    CssClass="span-7 last" />
                            </div>
                            <mux:LinkButton
                                runat="server"
                                id="urlGo"
                                CssClass="span-5"
                                OnClick="urlGo_Click"
                                Text="URL ..." />
                        </mux:Panel>
                        <mux:Panel
                            runat="server"
                            id="urlPnl">
                            <div class="label">
                                URL
                            </div>
                            <div class="last">
                                <mux:TextBox
                                    runat="server"
                                    id="url"
                                    CssClass="span-12 last"
                                    Text="http://code.google.com/p/magix" />
                            </div>
                            <div class="label">
                                Description
                            </div>
                            <div class="last">
                                <mux:TextBox
                                    runat="server"
                                    id="description"
                                    CssClass="span-12 last"
                                    Text="Magix!" />
                                <mux:LinkButton
                                    runat="server"
                                    id="vCard"
                                    style="margin-bottom:36px;"
                                    OnClick="vCard_Click"
                                    Text="MECARD ..." />
                            </div>
                        </mux:Panel>
                        <mux:Button
                            runat="server"
                            id="next1"
                            CssClass="span-4 last btn"
                            OnClick="next1_Click"
                            Text="Next >>" />
                    </div>
                </mux:MultiPanelView>
                <mux:MultiPanelView 
                    runat="server" 
                    DefaultWidget="next2"
                    ID="mpv2">
                    <div class="mux-insert">
                        <mux:Panel
                            runat="server"
                            CssClass="span-5 height-8 texture-panel"
                            OnClick="fgText_Click"
                            ToolTip="Click to Change Color or Texture used in rendering the QR Code's Foreground Color"
                            id="fgText">
                            &nbsp;
                        </mux:Panel>
                        <mux:Panel
                            runat="server"
                            CssClass="span-5 height-8 texture-panel"
                            OnClick="bgText_Click"
                            ToolTip="Click to Change Color or Texture used in rendering the QR Code's Background Color"
                            id="bgText">
                            &nbsp;
                        </mux:Panel>
                        <div class="span-4 last" style="margin-bottom:18px;">
                            <mux:CheckBox
                                runat="server"
                                CssClass="mux-qr-burn"
                                id="burn" /><label for='<%=burn.ClientID %>'>Burn</label>
                        </div>
                        <div class="span-4 last label">
                            Clockwise Rotation
                        </div>
                        <div class="last">
                            <mux:TextBox
                                runat="server"
                                id="rotate"
                                CssClass="span-4 last auto-width"
                                Text="0" />
                        </div>
                        <div class="span-4 last label">
                            Border Radius
                        </div>
                        <div class="last">
                            <mux:TextBox
                                runat="server"
                                id="borderRadius"
                                CssClass="span-4 last auto-width"
                                Text="25" />
                        </div>
                        <mux:Button
                            runat="server"
                            id="next2"
                            CssClass="span-4 last btn"
                            OnClick="next2_Click"
                            Text="Next >>" />
                    </div>
                </mux:MultiPanelView>
                <mux:MultiPanelView 
                    runat="server" 
                    DefaultWidget="next4"
                    ID="mpv3">
                    <div class="mux-insert">
                        <mux:Image
                            runat="server"
                            id="qrCode"
                            CssClass="qr-code-generated"
                            ImageUrl="media/images/wait-hourglass.png" />
                        <div class="span-10 last label">
                            URL to QR Code
                        </div>
                        <mux:Panel
                            runat="server"
                            DefaultWidget="next4">
                            <mux:TextBox
                                runat="server"
                                id="urlCode"
                                CssClass="last download-qr-code" />
                            <mux:Button
                                runat="server"
                                id="next3"
                                CssClass="span-4 last btn"
                                OnClick="next3_Click"
                                style="margin-top:108px;"
                                Text="New Code!" />
                            <mux:Button
                                runat="server"
                                id="next4"
                                CssClass="span-4 last btn"
                                OnClick="next4_Click"
                                style="margin-top:108px;margin-right:10px;"
                                Text="<< Back" />
                        </mux:Panel>
                    </div>
                </mux:MultiPanelView>
            </Content>
        </mux:MultiPanel>
    </div>
</mux:Panel>
