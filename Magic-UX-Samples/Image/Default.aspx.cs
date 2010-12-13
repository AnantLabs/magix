using System;
using Magic.UX.Widgets;

public partial class ImageSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        if (img.ImageUrl.Contains("yahoo"))
            img.ImageUrl = "../media/images/google.png";
        else
            img.ImageUrl = "../media/images/yahoo.jpg";
        btn.Text = "change again...";
    }
}
