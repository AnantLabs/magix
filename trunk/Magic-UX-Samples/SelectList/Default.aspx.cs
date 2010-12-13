using System;
using Magic.UX.Widgets;

public partial class SelectListSample : System.Web.UI.Page 
{
    protected void lst_SelectedIndexChanged(object sender, EventArgs e)
    {
        lbl.Text = lst.SelectedItem.Text + " " + lst.SelectedItem.Value;
    }

    protected void btn_Click(object sender, EventArgs e)
    {
        lst.SelectedIndex = 2;
    }
}
