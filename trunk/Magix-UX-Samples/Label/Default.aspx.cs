using System;
using Magix.UX.Widgets;
using Magix.UX.Effects;

public partial class LabelSample : System.Web.UI.Page 
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        lbl.MouseOverEffect = new EffectHighlight(lbl, 500);
    }

    protected void lbl_Click(object sender, EventArgs e)
    {
        (sender as Label).Text = "Clicked at: " + DateTime.Now;
    }
}
