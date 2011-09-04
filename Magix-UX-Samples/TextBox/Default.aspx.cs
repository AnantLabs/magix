using System;
using Magix.UX.Widgets;
using Magix.UX.Effects;

public partial class TextBoxSample : System.Web.UI.Page 
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        txt.FocusedEffect = new EffectHighlight(lbl, 500);
    }

    protected void btn_Click(object sender, EventArgs e)
    {
        lbl.Text = txt.Text;
    }
}
