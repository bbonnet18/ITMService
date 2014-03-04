using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using UserModels;

namespace ITMService
{
    public partial class BasicTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //    HttpClientHandler handler = new HttpClientHandler()
            //    {
            //        Credentials = new System.Net.NetworkCredential("bbonnet18","october17")
            //    };
            //    HttpClient client = new HttpClient(handler);
            //    client.BaseAddress = new Uri("http://localhost:57635");
            //    var response = client.GetAsync("api/Build/").Result;
            //    string s = response.Content.ToString();
            //    string st = "";

            //    if (User.Identity.IsAuthenticated)
            //    {
            //        Page.Title = "is auth" + User.Identity.Name;
            //    }
            //

            List<User> testList = UsersDataAccess.GetAllUsers();

            string test = "";
        }
    }
}