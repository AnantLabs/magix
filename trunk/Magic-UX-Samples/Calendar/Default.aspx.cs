using System;
using Magic.UX.Widgets;

public partial class CalendarSample : System.Web.UI.Page 
{
    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            cal.Value = DateTime.Now.AddDays(-5);
        }
        base.OnLoad(e);
    }

    protected void cal_DateChanged(object sender, EventArgs e)
    {
        lbl.Text = cal.Value.ToString("ddd d. MMMM yyyy");
    }
}
