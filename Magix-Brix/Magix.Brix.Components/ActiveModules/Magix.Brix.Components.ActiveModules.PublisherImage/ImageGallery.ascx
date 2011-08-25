<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.PublisherImage" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.PublisherImage.ImageGallery" %>

<link href="media/modules/ImageGallery.css" rel="stylesheet" type="text/css" />

<script type="text/ecmascript">
(function() {
  var el = MUX.$('<%=scroller.ClientID %>');
  var idx = 0;
  el.observe('click', function(){
    el.className = 'mux-one-gallery-scroller mux-one-gallery-' + (++idx);
    if(idx == 30) {
      idx = 0;
    }
  }, el);
})();
</script>

<mux:Panel
    runat="server"
    id="wrp"
    CssClass="mux-one-gallery">
    <mux:Panel
        runat="server"
        CssClass="mux-one-gallery-scroller"
        id="scroller">
        <asp:Repeater
            runat="server"
            id="rep">
            <ItemTemplate>
                <div class="mux-one-gallery-image">
                    <img 
                        src='<%#Eval("Value") %>' 
                        alt='Image Gallery Image ... ' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </mux:Panel>
    <mux:AspectModal
        runat="server"
        id="modal" />
</mux:Panel>
