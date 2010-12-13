using System;
using Magic.UX.Widgets;

public partial class AccordionSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        btn.Text = "Button clicked " + DateTime.Now;
    }
}
