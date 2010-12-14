using System;
using Magix.UX.Widgets;

public partial class MenuSample : System.Web.UI.Page 
{
    protected void MenuItemClicked(object sender, EventArgs e)
    {
        lbl.Text = (sender as MenuItem).Text;
    }
}
