using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ITMService.Models;
using System.Data.SqlClient;

namespace ITMService.Controllers
{
    public class UserController : ApiController
    {
        // GET api/<controller>
        //[HttpGet]
        //public Dictionary<string,string> GetUser()
        //{

        //  return getUser();
            
        //}
        // check to see that the user exists
        [HttpGet]
        [AllowAnonymous]
        public Dictionary<string,string> GetUser(String id)
        {
            return GetUserInfoByEmail(id);
        }
        // This will add a user to the system
        [HttpPost]
        [ActionName("register")]
        [AllowAnonymous]
        public Dictionary<string,string> AddUser(User user)
        {
            if (!ModelState.IsValid || user == null)
            {
                // throw an exception telling us that the user info wasn't provided
                    throw new ArgumentNullException("user", "User information missing");
            }

            Dictionary<string, string> tempuser = null;

            Log.LogIt("got new user: "+user.userName );
            try
            {
                tempuser = GetUserInfoByEmail(user.email);
                if (tempuser.Keys.Count <= 1)// there was an error finding the user, so add this user because they don't exist
                {
                    insertUser(user);
                    Dictionary<string, string> returnDic = new Dictionary<string, string>();
                     returnDic.Add("userName",user.userName);
                     returnDic.Add("lName",user.lName);
                     returnDic.Add("fName",user.fName);
                     returnDic.Add("email",user.email);
                     returnDic.Add("role", "builder");
                     tempuser = returnDic;
                }
                

            }
                // Report back on any errors
            catch (ArgumentNullException ex)
            {
                Dictionary<string, string> nullDic = new Dictionary<string, string>();
                nullDic.Add("error", "error adding user - user was null");
                return nullDic;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> nullDic = new Dictionary<string, string>();
                nullDic.Add("error", String.Format("error adding user - there was an exception: {0}",ex.Message));
                return nullDic;
            }
            finally
            {

            }
            // return the dictionary of the user that was originally provided because we know it's good to go
            //Dictionary<string, string> returnDic = new Dictionary<string, string>();
            //returnDic.Add("userName",user.userName);
            //returnDic.Add("lName",user.lName);
            //returnDic.Add("fName",user.fName);
            //returnDic.Add("email",user.email);
            return tempuser;

        }

        // this will register a user, it doesn't check to see if the user exists first, needs to do that. 
        [NonAction]
        private void insertUser(User user)
        {
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "INSERT INTO Users (userName,lName, fName, email, password) VALUES (@userName, @lName, @fName,@email, @password); SELECT SCOPE_IDENTITY();";

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@userName", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@userName"].Value = user.userName;

            cmdBuild.Parameters.Add("@fName", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@fName"].Value = user.fName;

            cmdBuild.Parameters.Add("@lName", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@lName"].Value = user.lName;

            cmdBuild.Parameters.Add("@email", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@email"].Value = user.email;

            string password = PasswordHash.CreateHash(user.password);

            cmdBuild.Parameters.Add("@password", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@password"].Value = password;


            try
            {
                buildsDB.Open();//open the db

                string newUserID = cmdBuild.ExecuteScalar().ToString();
               // int newID = Convert.ToInt32(newUserID);

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            finally
            {
                buildsDB.Close();
            }

           
           
        }

        // no two users can have the same email, so need to take that into consideration

        [NonAction]
        private Dictionary<string,string> GetUserInfoByEmail(string email)
        {
            SqlConnection buildsDB = new SqlConnection();
            SqlDataReader rdr = null;
            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "SELECT userID, userName, role, fName, lName FROM Users  WHERE  (userName LIKE @email)";

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@email", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@email"].Value = email;
           
            Dictionary<string, string> returnDic = new Dictionary<string,string>();
            try
            {
                buildsDB.Open();//open the db
                rdr = cmdBuild.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        returnDic.Add("userID", rdr["userID"].ToString());
                       returnDic.Add("lName",rdr["lName"].ToString());
                       returnDic.Add("fName", rdr["fName"].ToString());
                        returnDic.Add("role",rdr["role"].ToString());
                        returnDic.Add("userName",rdr["userName"].ToString());
                        returnDic.Add("email", email);
                    }
                }

                if (returnDic.Keys.Count == 0)// nothing was returned
                {
                    returnDic.Add("error", "Could not find a user with that email address.");
                }

                rdr.Close();

            }
            catch (Exception ex)
            {
                returnDic.Add("error", "Could not find a user with that email address.");
            }
            finally
            {
                buildsDB.Close();
            }

            return returnDic;
        }

        [NonAction]
         private Dictionary<string,string> getUser()
        {
            SqlConnection buildsDB = new SqlConnection();
            SqlDataReader rdr = null;
            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "SELECT userID, userName, role, email, fName, lName FROM Users  WHERE  (userName LIKE @userName)";

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@userName", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@userName"].Value =User.Identity.Name;

            int userID = 0;
            Dictionary<string, string> returnDic = new Dictionary<string,string>();
            try
            {
                buildsDB.Open();//open the db
                rdr = cmdBuild.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                       returnDic.Add("lName",rdr["lName"].ToString());
                       returnDic.Add("fName", rdr["fName"].ToString());
                        returnDic.Add("role",rdr["role"].ToString());
                        returnDic.Add("userName",rdr["userName"].ToString());
                        returnDic.Add("email", rdr["email"].ToString());
                    }
                }
                string newUserID = cmdBuild.ExecuteScalar().ToString();
                userID = Convert.ToInt32(newUserID);


            }
            catch (Exception ex)
            {
                returnDic.Add("error", "Could not find a user with that email address.");
            }
            finally
            {
                buildsDB.Close();
            }

            return returnDic;
        }

    
    }
}

/*
 string username = "bbonnet";
            string password = "myPassword!@#";

            string hashedPW = PasswordHash.CreateHash(password);// creates the actual hashed password, this should be stored

            bool isSame = PasswordHash.ValidatePassword("myPwssword!@#", hashedPW);
            string test = "";
*/