using System;
using Magic.UX.Widgets;

public partial class CheckBoxSample : System.Web.UI.Page 
{
    protected void chk_CheckedChanged(object sender, EventArgs e)
    {
        lbl.Text = "Changed at: " + DateTime.Now + " to " + chk.Checked;
    }
}
