using System;
using ITMService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;

/* this controller class will access all the builds and provide back a list of them
 * 
 * */

namespace ITMService.Controllers
{

    public class BuildController : ApiController
    {

        Build[] builds = new Build[]{// an array of Builds
            new Build {buildID = "1", title = "first one", tags = "tag 1, tag 2", manifestPath = "1.json", previewImagePath = "1_preview.png"},
            new Build {buildID = "2", title = "2nd one", tags = "tag 1, tag 2", manifestPath = "2.json", previewImagePath = "2_preview.png"},
            new Build {buildID = "3", title = "third", tags = "tag 1, tag 2", manifestPath = "3.json", previewImagePath = "3_preview.png"},
        };

        public List<Build> GetAllBuilds()// will return all builds in the database
        {


            // setup the reader and DB connections
            SqlDataReader rdr = null;//
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();
            string commStr = null;

            commStr = "SELECT * FROM Builds WHERE (active = 'true')";// get all of them
            if (User.Identity.IsAuthenticated)
            {
                Dictionary<string, string> newDic = getUser();
                string role = newDic["role"].Trim();
                if (role == "admin")
                {
                    commStr = "SELECT * FROM Builds";// get all of them
                }
            }


            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            List<Build> returnBuilds = new List<Build>();

            buildsDB.Open();//open the db
            try
            {
                rdr = buildsConn.ExecuteReader();
                if (rdr.HasRows)// get all the builds 
                {

                    while (rdr.Read())
                    {
                        Build b = new Build();
                        b.buildID = rdr["buildID"].ToString();
                        b.title = rdr["title"].ToString();
                        b.tags = rdr["tags"].ToString();
                        b.previewImagePath = "previewImagePath";
                        b.manifestPath = rdr["manifestPath"].ToString();
                        //DateTime.Parse(rdr["dateCreated"].ToString());
                        b.applicationID = (int)rdr["applicationID"];
                        b.email = rdr["email"].ToString();
                        b.active = rdr["active"].ToString();
                        returnBuilds.Add(b);
                    }
                    rdr.Close();
                    buildsDB.Close();

                }

            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                rdr.Close();
                rdr = null;
                buildsDB.Close();
            }

            return returnBuilds;// return the set
        }
        // checks to see if a Build exists, if so, returns true
        [NonAction]
        private Boolean CheckExisting(int id)
        {

            SqlConnection buildsDB = new SqlConnection();

            SqlDataReader rdr = null;

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("SELECT applicationID FROM Builds WHERE applicationID = {0}", Convert.ToString(id));// get all of them

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            buildsDB.Open();

            Boolean doesExist = false;
            rdr = buildsConn.ExecuteReader();
            while (rdr.Read())
            {
                if (!DBNull.Value.Equals(rdr["applicationID"]))
                {
                    doesExist = true;
                }
            }


            buildsDB.Close();

            return doesExist;

        }

        [NonAction]
        public Build GetBuildById(int id) // will get one build based on the id and return it 
        {

            // setup the reader and DB connections
            SqlDataReader rdr = null;//
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("SELECT * FROM Builds WHERE applicationID = {0}", Convert.ToString(id));// get all of them

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            Build b = null;// set to null for now

            buildsDB.Open();//open the db
            try
            {
                rdr = buildsConn.ExecuteReader();

                if (rdr.HasRows)// get all the builds 
                {
                    while (rdr.Read())
                    {
                        b = new Build();

                        b.buildID = rdr["buildID"].ToString();
                        b.title = rdr["title"].ToString();
                        b.tags = rdr["tags"].ToString();
                        b.previewImagePath = "previewImagePath";
                        b.manifestPath = rdr["manifestPath"].ToString();
                        b.applicationID = (int)rdr["applicationID"];
                        b.email = rdr["email"].ToString();
                        b.active = rdr["active"].ToString();
                        //DateTime.Parse(rdr["dateCreated"].ToString());
                    }


                }

                if (b == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);// let user know it wasn't found
                }


            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                rdr.Close();
                rdr = null;
                buildsDB.Close();
            }
            return b;
        }



        // this method will take on the whole build
        [HttpPost]
        [ActionName("media")]// create the build and post it in the database
        [AllowAnonymous]
        public Dictionary<string, string> PostMedia(Build build)
        {

            Dictionary<string, string> appIDDic = new Dictionary<string, string>();
            string returnMsg = null;
            if (ModelState.IsValid && build == null)
            {
                // return it with zero so the app knows it didn't go through
               
                appIDDic.Add("error", "Uploaded items are not valid");
                return appIDDic;
            }

            // setup the reader and DB connections

            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();


            // set the parameters and the command string
            string commStr = String.Format("UPDATE Builds SET title=@title,tags=@tags WHERE applicationID = {0}", Convert.ToString(build.applicationID));
            int newID = 0;// used to capture the new appID when inserting a new one
            if (!CheckExisting(build.applicationID))
            {
               newID = insertNew(build);
               returnMsg = String.Format("INSERTED - {0}",newID);
                
            }else{// delete and set the newID
                deleteBuildItems(build.buildID);
                newID = build.applicationID;
                try
                {
                    // make the updates
                    SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

                    cmdBuild.Parameters.Add("@title", System.Data.SqlDbType.Char);
                    cmdBuild.Parameters["@title"].Value = build.title;
                    cmdBuild.Parameters.Add("@tags", System.Data.SqlDbType.Char);
                    cmdBuild.Parameters["@tags"].Value = build.tags;

                    buildsDB.Open();//open the db
                    int i = cmdBuild.ExecuteNonQuery();
                    returnMsg = String.Format("UPDATED - {0}",newID);

                }
                catch (Exception e)
                {
                    // return it with zero so the app knows it didn't go through
                    Dictionary<string, string> retDic = new Dictionary<string, string>();
                    retDic.Add("failureNoAdd", "-2");
                    retDic.Add("errorMsg", e.Message);

                    return retDic;
                }
                finally
                {
                    buildsDB.Close();
                }
            }
            
            
            
            appIDDic.Add("status", returnMsg);

            appIDDic.Add("ID", Convert.ToString(newID));

            return appIDDic;// returns a dictionary with the appID in it
        }

        [ActionName("delete")]
        public HttpResponseMessage DeleteBuild(int id)// delete the build
        {
            //deleteBuildItems(id);// delete all these build items
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("DELETE FROM Builds WHERE applicationID = {0}", Convert.ToString(id));

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            buildsDB.Open();//open the db
            try
            {
                buildsConn.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                buildsDB.Close();

            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [NonAction]
        private int insertNew(Build b)
        {
            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();
           //INSERT INTO BuildItems (caption, fileName, orderNumber, status, thumbnailPath, timeStamp, title, type, buildID, buildItemIDString) VALUES (@caption,  @fileName, @orderNumber, @status, @thumbnailPath, @timeStamp, @title, @type, @buildID, @buildItemIDString)";
            string commStr = "INSERT INTO Builds (title,tags,manifestPath,buildID,email) VALUES (@title,@tags,@manifestPath,@buildID,@email); SELECT SCOPE_IDENTITY();";
                
            // set the parameters and the command string

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@title", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@title"].Value = b.title;
            cmdBuild.Parameters.Add("@tags", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@tags"].Value = b.tags;
            cmdBuild.Parameters.Add("@manifestPath", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@manifestPath"].Value = b.manifestPath;
            cmdBuild.Parameters.Add("@buildID", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@buildID"].Value = b.buildID;
            string emailToUse = (b.email == null) ? "anonymous" : b.email;
            cmdBuild.Parameters.Add("@email", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@email"].Value = emailToUse;
            int newIntTest = -1;
            buildsDB.Open();
            try
            {
                newIntTest = Convert.ToInt32(cmdBuild.ExecuteScalar());
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                buildsDB.Close();
            }

            return newIntTest;
        }


        [NonAction]
        private void deleteBuildItems(string buildID)// this will delete all the build items for the buildID provided
        {// setup the db connection

            SqlConnection buildsDB = new SqlConnection();

            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = String.Format("DELETE FROM BuildItems WHERE buildID LIKE '{0}'", buildID);

            SqlCommand buildsConn = new SqlCommand(commStr, buildsDB);

            buildsDB.Open();//open the db
            try
            {
                int i = buildsConn.ExecuteNonQuery();
                string test = "test";
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                buildsDB.Close();
            }
        }


        [NonAction]
        private Dictionary<string, string> getUser()
        {
            SqlConnection buildsDB = new SqlConnection();
            SqlDataReader rdr = null;
            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "SELECT userID, userName, role, email, fName, lName FROM Users  WHERE  (userName LIKE @userName)";

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@userName", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@userName"].Value = User.Identity.Name;

            int userID = 0;
            Dictionary<string, string> returnDic = new Dictionary<string, string>();
            try
            {
                buildsDB.Open();//open the db
                rdr = cmdBuild.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        returnDic.Add("lName", rdr["lName"].ToString());
                        returnDic.Add("fName", rdr["fName"].ToString());
                        returnDic.Add("role", rdr["role"].ToString());
                        returnDic.Add("userName", rdr["userName"].ToString());
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

        [HttpGet]
        public string GetBuildActive(int id)
        {
            Build b = GetBuildById(id);
            if (b != null)
            {
                b.active = b.active.Trim();
                if (b.active == "false")
                {
                    setActiveForBuild(id, "true");
                }
                else
                {
                    setActiveForBuild(id, "false");
                }
                return "status set";

            }
            else
            {
                return "error: couldn't set status";
            }

        }
        // this will swap out the values for active
        [NonAction]
        private void setActiveForBuild(int id, string status)
        {

            SqlConnection buildsDB = new SqlConnection();
            SqlDataReader rdr = null;
            buildsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            string commStr = "UPDATE Builds SET active=@active WHERE applicationID=@applicationID";

            SqlCommand cmdBuild = new SqlCommand(commStr, buildsDB);

            cmdBuild.Parameters.Add("@active", System.Data.SqlDbType.Char);
            cmdBuild.Parameters["@active"].Value = status;

            cmdBuild.Parameters.Add("@applicationID", System.Data.SqlDbType.Int);
            cmdBuild.Parameters["@applicationID"].Value = id;
            buildsDB.Open();
            try
            {
                cmdBuild.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));

            }
            finally
            {
                buildsDB.Close();
            }

        }



    }
}
