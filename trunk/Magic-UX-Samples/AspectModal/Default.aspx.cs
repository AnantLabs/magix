using System;
using Magic.UX.Widgets;
using System.Drawing;

public partial class AspectModalSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        Random rnd = new Random();
        modal.Color = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
        modal.BottomColor = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
        modal.Opacity = ((decimal)rnd.Next(25, 75) / 100);
    }
}
