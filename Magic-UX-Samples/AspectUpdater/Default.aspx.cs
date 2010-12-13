using System;
using Magix.UX.Widgets;

public partial class BehaviorUpdaterSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        (sender as Button).Text = "Clicked at: " + DateTime.Now;
        System.Threading.Thread.Sleep(2000);
    }

    protected void change_Click(object sender, EventArgs e)
    {
        if (updater.Element == "updater1")
        {
            updater.Element = "updater2";
            updater.Opacity = 0.3M;
            updater.Delay = 1000;
            lbl.Text = "Red, .3 opacity, 1 second delay";
        }
        else
        {
            updater.Element = "updater1";
            updater.Opacity = 0.7M;
            updater.Delay = 500;
            lbl.Text = "Black, .7 opacity, .5 second delay";
        }
    }
}
