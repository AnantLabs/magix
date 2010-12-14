using System;
using Magix.UX.Widgets;

public partial class HyperLinkSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        hpl.Text = "Now pointing to Yahoo!";
        hpl.URL = "http://yahoo.com";
    }
}
