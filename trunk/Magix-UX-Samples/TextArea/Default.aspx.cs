using System;
using Magix.UX.Widgets;

public partial class TextAreaSample : System.Web.UI.Page 
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!IsPostBack)
        {
            txt.Text = @"
Hello there stranger ...?
Hello there stranger ...?
Hello there stranger ...?
Hello there stranger ...?
Hello there stranger ...?
Hello there stranger ...?
Hello there stranger ...?
Hello there stranger ...?";
        }
    }

    protected void btn_Click(object sender, EventArgs e)
    {
        lbl.Text = txt.Text;
    }
}
