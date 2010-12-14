using System;
using Magix.UX.Widgets;
using Magix.UX.Effects;

public partial class TimerSample : System.Web.UI.Page 
{
    protected override void OnLoad(EventArgs e)
    {
        wnd.MouseOverEffect = new EffectSize(wnd, 200, 250, 300);
        wnd.MouseOutEffect = new EffectSize(wnd, 200, 150, 250);
        base.OnLoad(e);
    }

    protected void timer_Tick(object sender, EventArgs e)
    {
        lbl.Text = "Timer kicked in at; " + DateTime.Now;
    }

    protected void btn_Click(object sender, EventArgs e)
    {
        wnd.Visible = true;
    }
}
