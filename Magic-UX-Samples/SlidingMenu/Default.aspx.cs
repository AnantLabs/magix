using System;
using Magic.UX.Widgets;
using Magic.UX.Effects;

public partial class SlidingMenu : System.Web.UI.Page 
{
    protected void sliding_LeafMenuItemClicked(object sender, EventArgs e)
    {
        lbl.Text = (sender as SlidingMenuItem).Text;
    }
}
