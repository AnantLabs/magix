using System;
using Magix.UX.Widgets;

public partial class ButtonSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        (sender as Button).Text = "Clicked at: " + DateTime.Now;
    }
}
