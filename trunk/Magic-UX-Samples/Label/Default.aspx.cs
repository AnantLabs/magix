using System;
using Magix.UX.Widgets;

public partial class LabelSample : System.Web.UI.Page 
{
    protected void lbl_Click(object sender, EventArgs e)
    {
        (sender as Label).Text = "Clicked at: " + DateTime.Now;
    }
}
