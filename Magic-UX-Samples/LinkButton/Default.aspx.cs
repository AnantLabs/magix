using System;
using Magix.UX.Widgets;

public partial class LinkButtonSample : System.Web.UI.Page 
{
    protected void lnk_Click(object sender, EventArgs e)
    {
        (sender as LinkButton).Text = "Clicked at: " + DateTime.Now;
    }
}
