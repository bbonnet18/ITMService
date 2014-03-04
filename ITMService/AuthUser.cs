using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;


namespace ITMService
{
    public class AuthUser
    {

        // this will return the user's password hash

        public static string GetUserPassword(string username)
        {
            SqlConnection buildsDB = new SqlConnection();
            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "SELECT password FROM Users WHERE userName LIKE @userName";

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@userName", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@userName"].Value = username;
            buildsDB.Open();
            Log.LogIt("checking the password");
            string passHash = null;
            try
            {
                passHash = cmdBuild.ExecuteScalar().ToString();
            }
            catch (NullReferenceException e)
            {
                passHash = null;
            }
            catch(Exception e)
            {
                passHash = null;
            }
            finally
            {

            }
            
            return passHash;
        }
    }

   
}