using Firebase.Auth;
using HomeCookingWebPanel.Model;
using System;
using System.Threading.Tasks;

namespace HomeCookingWebPanel
{
    public partial class Login : System.Web.UI.Page
    {
        private const string API_KEY = "AIzaSyDnJpPnzSKUkng6EBCamRu0cH-Ij2azGEk";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected async void LoginButton_Click(object sender, EventArgs e)
        {
            string email = Txt_Email.Text;
            string password = Txt_Password.Text;
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                // Kullanıcı girişi başarılı ise
                if (auth != null && !string.IsNullOrEmpty(auth.FirebaseToken))
                {
                    Data.Instance.UserInfo = auth.User.Email;
                    Response.Redirect("OrderPage.aspx");
                }
                else
                {
                    // Kullanıcı girişi başarısız ise, hata mesajı gösterebilirsiniz
                    // Örneğin:
                    // lblErrorMessage.Text = "Kullanıcı girişi başarısız.";
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Kullanıcı girişi sırasında bir hata oluştu ise, hata mesajını gösterebilirsiniz
                // Örneğin:
                // lblErrorMessage.Text = "Bir hata oluştu: " + ex.Reason.ToString();
            }
        }
    }
}