using System;
using Magix.UX.Widgets;

public partial class RadioButtonSample : System.Web.UI.Page 
{
    protected void chk_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton rdo = sender as RadioButton;
        lbl.Text = rdo.Info + " " + rdo.Checked;
    }
}
