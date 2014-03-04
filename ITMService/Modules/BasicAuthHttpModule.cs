using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Net.Http.Headers;

namespace ITMService.Modules
{
    public class BasicAuthHttpModule : IHttpModule
    {

        private const string Realm = "www.itmgo.com";

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
            Log.LogIt("this is logging that this thing even gets created");
        }

        private static void SetPrincipal(IPrincipal principal)
        {
           
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
                Log.LogIt("current principal: " + principal.Identity.Name);
            }
        }

        // TODO: Here is where you would validate the username and password.
        private static bool CheckPassword(string username, string password)
        {
            string passHash = AuthUser.GetUserPassword(username);

            if (passHash == null)
            {
                return false;
            }
            if (PasswordHash.ValidatePassword(password, passHash))
            {
                return true;
            }
            else
            {
                return false;
            }
            //return username == "user" && password == "password";
        }

        private static bool AuthenticateUser(string credentials)
        {
            bool validated = false;
            Log.LogIt("authenticating on: " + credentials);
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                validated = CheckPassword(name, password);
                Log.LogIt("isValidated: " + validated);
                if (validated)
                {
                    var identity = new GenericIdentity(name);
                  
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                validated = false;
                Log.LogIt("not validated");

            }
            return validated;
        }

        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            Log.LogIt("Beginning Authentication");
            if (HttpContext.Current != null)
            {
                Log.LogIt("current is not null");
            }
            else
            {
                Log.LogIt("current IS NULL");
            }
            try
            {
                var requestA = HttpContext.Current.Request;
               if(requestA == null){
                   Log.LogIt("THE REQUEST is NULL");
               }else{
                   Log.LogIt("the request is not null");
                   foreach (string s in requestA.Headers.Keys)
                   {
                       Log.LogIt(" --- -- "+s+ " :" +requestA.Headers[s]);
                   }
               }
                var authHeaderB = requestA.Headers["Authorization"];
                if (authHeaderB == null)
                {
                    Log.LogIt("THE AUTHORIZATION HEADER is NULL");
                }
                else
                {
                    Log.LogIt("the AUTHORIZATION HEADER is not null");
                }
            }
            catch (HttpException ex)
            {
                Log.LogIt("running under IIS 7 - " + ex.Message);
            }
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                Log.LogIt("-->authHeader is not null");
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
                Log.LogIt("-->checking authHeaderVal");
                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    Log.LogIt("---->attempting to authenticate, running AuthenticateUser() with - "+authHeaderVal.Parameter);
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        // If the request was unauthorized, add the WWW-Authenticate header 
        // to the response.
        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            Log.LogIt("ending authentication");
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        public void Dispose()
        {
        }

       
    }
}