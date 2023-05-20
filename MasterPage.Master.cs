using HomeCookingWebPanel.Model;
using System;

namespace HomeCookingWebPanel
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string email = Data.Instance.UserInfo;
            int atIndex = email.IndexOf("@");
            string username = "";
            if (atIndex != -1)
            {
                username = email.Substring(0, atIndex);
            }
            UserInfo.Text = username;
        }
    }
}