using System;
using Magix.UX.Widgets;

public partial class PanelSample : System.Web.UI.Page 
{
    protected void pnl_MouseOver(object sender, EventArgs e)
    {
        lbl.Text = "Mouse Over";
    }

    protected void pnl_MouseOut(object sender, EventArgs e)
    {
        lbl.Text = "Mouse Out";
    }
}
