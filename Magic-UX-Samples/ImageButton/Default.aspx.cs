using System;
using Magic.UX.Widgets;

public partial class ImageButtonSample : System.Web.UI.Page 
{
    protected void btn_Click(object sender, EventArgs e)
    {
        if (btn.ImageUrl.Contains("yahoo"))
            btn.ImageUrl = "../media/images/google.png";
        else
            btn.ImageUrl = "../media/images/yahoo.jpg";
    }
}
