using Google.Cloud.Firestore;
using System;

namespace HomeCookingWebPanel
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            LoginUserInfo();
        }

        async void LoginUserInfo()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"firestore.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            FirestoreDb database = FirestoreDb.Create("project-management-22705");
            Query Qref = database.Collection("recipes");
            QuerySnapshot snap = await Qref.GetSnapshotAsync();
            Response.Redirect("OrderPage.aspx");
            foreach (DocumentSnapshot docsnap in snap)
            {
                if (docsnap.Exists)
                {
             
                    
                }
            }
        }
    }
}