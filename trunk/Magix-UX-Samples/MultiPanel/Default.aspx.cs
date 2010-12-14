using System;
using Magix.UX.Widgets;

public partial class MultiPanelSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        btn.Text = "Button clicked at: " + DateTime.Now;
    }
}
