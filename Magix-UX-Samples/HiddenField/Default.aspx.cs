using System;
using Magix.UX.Widgets;

public partial class HiddenFieldSample : System.Web.UI.Page 
{
    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            lbl.Text = hid.Value;
        }
        base.OnLoad(e);
    }

    protected void btn_Click(object sender, EventArgs e)
    {
        hid.Value = txt.Text;
        txt.Text = "";
        lbl.Text = "Value saved to hidden field";
    }

    protected void btn2_Click(object sender, EventArgs e)
    {
        lbl.Text = "Value: " + hid.Value + " fetched from hidden field";
    }
}
