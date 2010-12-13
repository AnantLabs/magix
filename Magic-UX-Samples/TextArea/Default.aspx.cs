using System;
using Magic.UX.Widgets;

public partial class TextAreaSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        lbl.Text = txt.Text;
    }
}
