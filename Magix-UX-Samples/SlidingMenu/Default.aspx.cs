using System;
using Magix.UX.Widgets;
using Magix.UX.Effects;

public partial class SlidingMenu : System.Web.UI.Page 
{
    protected void sliding_LeafMenuItemClicked(object sender, EventArgs e)
    {
        lbl.Text = (sender as SlidingMenuItem).Text;
    }
}
