using System;
using Magic.UX.Widgets;

public partial class TreeViewSample : System.Web.UI.Page 
{
    protected void tree_SelectedItemChanged(object sender, EventArgs e)
    {
        lbl.Text = tree.SelectedItem.Text;
    }
}
