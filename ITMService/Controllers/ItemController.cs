using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ITMService.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using System.IO;
using ITMService.Filters;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ITMService.Controllers
{
    [ExcFilter]
    public class ItemController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpRequestException("Not MultiPart");// pass on the exception, letting the catcher filter know that it's not multipart
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            string medDir = HttpContext.Current.Server.MapPath("~/Media");

            // Read the form data and return an async task. 
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.BadRequest, t.Exception);
                    }

                    try
                    {

                        BuildItem newB = new BuildItem();
                        newB.status = provider.FormData["status"];
                        newB.caption = provider.FormData["caption"];
                        newB.orderNumber = Convert.ToInt32(provider.FormData["orderNumber"]);

                        newB.title = provider.FormData["title"];
                        newB.type = provider.FormData["type"];
                        newB.buildID = provider.FormData["buildID"];
                        newB.buildItemIDString = provider.FormData["buildItemIDString"];
                        newB.timeStamp = new DateTime(2013, 3, 21, 21, 34, 0);
                        string ext = (newB.type == "image") ? ".jpg" : ".mp4";
                        string newFileName = Convert.ToString(newB.buildID) + "_" + Convert.ToString(newB.orderNumber) + ext;
                        newB.fileName = newFileName;
                        newB.thumbnailPath = String.Format("{0}_thumb.jpg", newB.buildID);


                        int newBuildID = insertBuildItems(newB);
                        //This illustrates how to get the file names. 
                        foreach (MultipartFileData file in provider.FileData)
                        {
                            Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                            Trace.WriteLine("Server file path: " + file.LocalFileName);

                            string fileName = file.Headers.ContentDisposition.FileName;
                            fileName = file.Headers.ContentDisposition.FileName;//System.Net.WebUtility.HtmlDecode(fileName);
                            medDir = medDir + @"\";
                            string newPath = String.Format("{0}{1}", medDir, newFileName); //file.Headers.ContentDisposition.FileName.Substring(1, file.Headers.ContentDisposition.FileName.LastIndexOf(".")),ext);
                            Trace.WriteLine(newPath);
                            // check if the file already exists and then delete it before moving 
                            if (File.Exists(newPath))
                            {
                                File.Delete(newPath);
                            }
                            // move the file to the new location. 
                            File.Move(file.LocalFileName, newPath);
                            //if (newB.orderNumber == 0)
                            //{
                            //    Bitmap newThumbImg = (newB.type == "image") ? buildThumb(newPath, newB.buildID) : null;

                            //    if(newB.type == "video")
                            //        GetVideoThumbnail(newPath, medDir);
                                
                            //    if (newThumbImg != null)
                            //    {
                            //        ImageCodecInfo myImageCodecInfo;
                            //        Encoder myEncoder;
                            //        EncoderParameter myEncoderParameter;
                            //        EncoderParameters myEncoderParameters;

                            //        myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            //        myEncoder = Encoder.Quality;

                            //        myEncoderParameters = new EncoderParameters(1);

                            //        myEncoderParameter = new EncoderParameter(myEncoder, 75L);
                            //        myEncoderParameters.Param[0] = myEncoderParameter;

                            //        string newThumbPath = String.Format("{0}{1}", medDir, newB.thumbnailPath);
                                   
                            //        FileStream stream = new FileStream(newThumbPath, FileMode.Create);

                            //        newThumbImg.Save(stream, myImageCodecInfo, myEncoderParameters);
                            //        newThumbImg.Dispose();
                            //        stream.Dispose();// release the stream
                            //    }

                            //}

                        }

                        Dictionary<string, int> retDic = new Dictionary<string, int>();
                        retDic.Add("newBuildID", newBuildID);
                        retDic.Add("orderNumber", newB.orderNumber);


                        HttpResponseMessage hr = Request.CreateResponse(HttpStatusCode.OK, retDic);
                        hr.ReasonPhrase = Convert.ToString(newBuildID);
                        return hr;

                    }
                    catch (ArgumentNullException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Null object supplied by app" + e.Message, e);
                        return r;
                    }
                    catch (ArgumentException a)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "failed to save " + a.Message, a);
                        return r;
                    }
                    catch (NullReferenceException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Null object supplied by app" + e.Message, e);
                        return r;
                    }
                    catch (FormatException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Invalid Format for Field" + e.Message, e);
                        return r;
                    }
                    catch (InvalidCastException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Cast" + e.Message, e);
                        return r;
                    }
                    catch (SqlException sE)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "SQL Insert Failed " + sE.Message, sE);
                        log(sE.Message);
                        return r;
                    }
                    catch (Exception e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "failed to save " + e.Message, e);
                        return r;
                    }
                    finally
                    {

                    }


                });

            return task;
        }

        
        // this matches the 'api/Item/preview/{id}' uri because any other reference
        // to post is not functioning properly, so I added the id
        [HttpPost]
        [ActionName("preview")]
        public Task<HttpResponseMessage> Preview(int id)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpRequestException("Not MultiPart");// pass on the exception, letting the catcher filter know that it's not multipart
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            string medDir = HttpContext.Current.Server.MapPath("~/Media");

            // Read the form data and return an async task. 
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.BadRequest, t.Exception);
                    }

                    try
                    {

                        string newFileName = String.Format("{0}_preview.jpg", Convert.ToString(id));

                        //This illustrates how to get the file names. 
                        foreach (MultipartFileData file in provider.FileData)
                        {
                            Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                            Trace.WriteLine("Server file path: " + file.LocalFileName);

                            string fileName = file.Headers.ContentDisposition.FileName;
                            fileName = file.Headers.ContentDisposition.FileName;//System.Net.WebUtility.HtmlDecode(fileName);
                            medDir = medDir + @"\";
                            string newPath = String.Format("{0}{1}", medDir, newFileName); //file.Headers.ContentDisposition.FileName.Substring(1, file.Headers.ContentDisposition.FileName.LastIndexOf(".")),ext);
                            Trace.WriteLine(newPath);
                            // check if the file already exists and then delete it before moving 
                            if (File.Exists(newPath))
                            {
                                File.Delete(newPath);
                            }
                            // move the file to the new location. 
                            File.Move(file.LocalFileName, newPath);

                        }


                        HttpResponseMessage hr = Request.CreateResponse(HttpStatusCode.OK);
                        hr.ReasonPhrase = "Added the preview image";
                        return hr;

                    }
                    catch (ArgumentNullException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Null object supplied by app" + e.Message, e);
                        return r;
                    }
                    catch (ArgumentException a)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "failed to save " + a.Message, a);
                        return r;
                    }
                    catch (NullReferenceException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Null object supplied by app" + e.Message, e);
                        return r;
                    }
                    catch (FormatException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Invalid Format for Field" + e.Message, e);
                        return r;
                    }
                    catch (InvalidCastException e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Cast" + e.Message, e);
                        return r;
                    }
                    catch (SqlException sE)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "SQL Insert Failed " + sE.Message, sE);
                        log(sE.Message);
                        return r;
                    }
                    catch (Exception e)
                    {
                        HttpResponseMessage r = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "failed to save " + e.Message, e);
                        return r;
                    }
                    finally
                    {

                    }


                });

            return task;

        }


        [NonAction]
        private Bitmap buildThumb(string filePath, string buildID)
        {
            Bitmap retImg;
            try
            {
                Bitmap image = new Bitmap(filePath);

                //ImageCodecInfo myImageCodecInfo;
                //Encoder myEncoder;
                //EncoderParameter myEncoderParameter;
                //EncoderParameters myEncoderParameters;

                //myImageCodecInfo = GetEncoderInfo("image/jpeg");
                //myEncoder = Encoder.Quality;

                //myEncoderParameters = new EncoderParameters(1);

                //myEncoderParameter = new EncoderParameter(myEncoder, 75L);
                //myEncoderParameters.Param[0] = myEncoderParameter;


                int newwidthimg = 160;
                float AspectRatio = (float)image.Size.Width / (float)image.Size.Height;
                int newHeight = Convert.ToInt32(newwidthimg / AspectRatio);
                Bitmap thumbnailBitmap = new Bitmap(newwidthimg, newHeight);
                Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newwidthimg, newHeight);
                thumbnailGraph.DrawImage(image, imageRectangle);
                //string thumbPath = filePath.Split('.')[0];
                //// need to figure out how to map the path to the server from here

                //thumbPath = String.Format("{0}_thumb.jpg",thumbPath);
                //thumbnailBitmap.Save(thumbPath, ImageFormat.Jpeg);
                retImg = thumbnailBitmap;
                thumbnailGraph.Dispose();
                //thumbnailBitmap.Dispose();
                //image.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }

            return retImg;

        }
         [NonAction]
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();

            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }
         [NonAction]
        public bool GetVideoThumbnail(string currentVid, string mediaDirectory)
        {
           

            //divide the duration in 3 to get a preview image in the middle of the clip
            //instead of a black image from the beginning.
            int secs;
            //secs = (int)Math.Round(TimeSpan.FromTicks(input.Duration.Ticks / 3).TotalSeconds, 0);
            //if (secs.Equals(0)) secs = 1;
            secs = 1;
            string medDir = mediaDirectory;
            string saveTo = String.Format("{0}_{1}.jpg", medDir, "test");
            string Params = string.Format("-i \"{0}\" -s 90x60 \"{1}\" -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", currentVid, saveTo, secs);//(("-i \"{0}\" \"{1}\" -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, saveThumbnailTo, secs);
            string output = RunProcess(Params);

            if (File.Exists(currentVid))
            {
                return true;
            }
            else
            {
                //try running again at frame 1 to get something
                Params = string.Format("-i \"{0}\" -s 90x60 \"{1}\" -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", currentVid, saveTo, 1);//("-i \"{0}\" \"{1}\" -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, saveThumbnailTo, 1)
                output = RunProcess(Params);

                if (File.Exists(saveTo))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }




        [NonAction]// inserts a new builditem and returns the integer of the new buildItem
        private int insertBuildItems(BuildItem b)
        {
            SqlDataReader rdr = null;// setup the sql reader
            SqlConnection buildItemsDB = new SqlConnection();// get the conn
            buildItemsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            buildItemsDB.Open();// open the database

            string[] keys = new string[10];
            keys[0] = "title";
            keys[1] = "caption";
            keys[2] = "status";
            keys[3] = "thumbnailPath";
            keys[4] = "timeStamp";
            keys[5] = "orderNumber";
            keys[6] = "buildID";
            keys[7] = "type";
            keys[8] = "buildItemID";
            keys[9] = "buildItemIDString";
            string[] ss = new string[10];

            ss[0] = b.title;
            ss[1] = b.caption;
            ss[2] = b.status;
            ss[3] = b.thumbnailPath;
            ss[4] = b.timeStamp.ToLongTimeString();
            ss[5] = Convert.ToString(b.orderNumber);
            ss[6] = b.buildID;
            ss[7] = b.type;
            ss[8] = Convert.ToString(b.buildItemID);
            ss[9] = b.buildItemIDString;
            string logStr = "";
            for (int i = 0; i < ss.Length; i++)
            {
                logStr += keys[i] + " : " + ss[i] + " ";

            }

            log(logStr);

            try
            {
                // check to see if it exists. If this one already exists, then update it with the new content, else insert it
                string itemExists = "INSERT INTO BuildItems (caption, fileName, orderNumber, status, thumbnailPath, timeStamp, title, type, buildID, buildItemIDString) VALUES (@caption,  @fileName, @orderNumber, @status, @thumbnailPath, @timeStamp, @title, @type, @buildID, @buildItemIDString)";// "," + b.fileName + "," + b.orderNumber + "," + b.status + "," + b.thumbnailPath + "," + b.timeStamp + "," + b.title + "," + b.type + "," + b.buildID + ")";
                SqlCommand cmdBuildItems = new SqlCommand(itemExists, buildItemsDB);

                // add the parameters
                cmdBuildItems.Parameters.Add("@caption", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@caption"].Value = b.caption;
                cmdBuildItems.Parameters.Add("@fileName", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@fileName"].Value = b.fileName;
                cmdBuildItems.Parameters.Add("@orderNumber", System.Data.SqlDbType.Int);
                cmdBuildItems.Parameters["@orderNumber"].Value = b.orderNumber;
                cmdBuildItems.Parameters.Add("@status", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@status"].Value = b.status;
                cmdBuildItems.Parameters.Add("@thumbnailPath", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@thumbnailPath"].Value = b.thumbnailPath;
                cmdBuildItems.Parameters.Add("@timeStamp", System.Data.SqlDbType.DateTime);
                cmdBuildItems.Parameters["@timeStamp"].Value = b.timeStamp;
                cmdBuildItems.Parameters.Add("@title", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@title"].Value = b.title;
                cmdBuildItems.Parameters.Add("@type", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@type"].Value = b.type;
                cmdBuildItems.Parameters.Add("@buildID", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@buildID"].Value = b.buildID;
                cmdBuildItems.Parameters.Add("@buildItemIDString", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@buildItemIDString"].Value = b.buildItemIDString;


                rdr = cmdBuildItems.ExecuteReader();// need to return the value we just created

                rdr.Close();

                string selectLatestString = "SELECT buildItemID FROM BuildItems WHERE buildID=@buildID AND orderNumber=@orderNumber";
                cmdBuildItems = new SqlCommand(selectLatestString, buildItemsDB);
                cmdBuildItems.Parameters.Add("@buildID", System.Data.SqlDbType.Char);
                cmdBuildItems.Parameters["@buildID"].Value = b.buildID;
                cmdBuildItems.Parameters.Add("@orderNumber", System.Data.SqlDbType.Int);
                cmdBuildItems.Parameters["@orderNumber"].Value = b.orderNumber;
                long returnID = 0;
                rdr = cmdBuildItems.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        // read the rows and return the ID

                      

                    }
                }
                rdr.Close();
                return (int)returnID;

            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                buildItemsDB.Close();
            }
        }
        [HttpGet]
        public BuildItem GetBuildItem(int buildID, int orderNumber)// gets and returns the buildItem by finding through the order number and buildID
        {
            SqlDataReader rdr = null;
            SqlConnection buildItemsDB = new SqlConnection();// get the conn
            buildItemsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            buildItemsDB.Open();
            // select if it has the right buildID and orderNumber
            string getItemCommStr = String.Format("SELECT * FROM BuildItems WHERE buildID = {0} AND orderNumber = {1}", buildID, orderNumber);
            SqlCommand getItemComm = new SqlCommand(getItemCommStr, buildItemsDB);

            try
            {

                rdr = getItemComm.ExecuteReader();
                BuildItem returnItem = new BuildItem();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {

                        returnItem.status = Convert.ToString(rdr["status"]);
                        returnItem.caption = Convert.ToString(rdr["caption"]);
                        returnItem.orderNumber = Convert.ToInt32(rdr["orderNumber"]);
                        returnItem.thumbnailPath = Convert.ToString(rdr["thumbnailPath"]);
                        returnItem.title = Convert.ToString(rdr["title"]);
                        returnItem.type = Convert.ToString(rdr["type"]);
                        returnItem.buildID = Convert.ToString(rdr["buildID"]);
                        returnItem.timeStamp = Convert.ToDateTime(rdr["timeStamp"]);
                        returnItem.fileName = Convert.ToString(rdr["fileName"]);
                        long bIID = (long)rdr["buildItemID"];
                        returnItem.buildItemID = Convert.ToInt32(bIID);

                    }


                }
                return returnItem;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                rdr.Close();
                rdr = null;
                buildItemsDB.Close();

            }
        }

        [HttpGet]
        public List<BuildItem> GetBuildItemsForId(int id)
        {

            List<BuildItem> items = new List<BuildItem>();
            SqlDataReader rdr = null;
            SqlConnection buildItemsDB = new SqlConnection();// get the conn
            buildItemsDB.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BuildsConnectionString"].ToString();

            buildItemsDB.Open();
            // select if it has the right buildID and orderNumber
            string getBuild = String.Format("SELECT buildID FROM Builds WHERE applicationID = {0}", id);



            SqlCommand getBuildComm = new SqlCommand(getBuild, buildItemsDB);

            try
            {

                string testID = getBuildComm.ExecuteScalar().ToString();

                if (testID != null)
                {
                    string getItems = String.Format("SELECT * FROM BuildItems WHERE buildID LIKE '{0}'", testID);
                    SqlCommand getItemsComm = new SqlCommand(getItems, buildItemsDB);
                    rdr = getItemsComm.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            BuildItem b = new BuildItem();
                            b.status = Convert.ToString(rdr["status"]);
                            b.caption = Convert.ToString(rdr["caption"]);
                            b.orderNumber = Convert.ToInt32(rdr["orderNumber"]);
                            b.thumbnailPath = Convert.ToString(rdr["thumbnailPath"]);
                            b.title = Convert.ToString(rdr["title"]);
                            b.type = Convert.ToString(rdr["type"]);
                            b.buildID = Convert.ToString(rdr["buildID"]);
                            b.timeStamp = Convert.ToDateTime(rdr["timeStamp"]);
                            b.fileName = Convert.ToString(rdr["fileName"]);
                            long bIID = (long)rdr["buildItemID"];
                            b.buildItemID = Convert.ToInt32(bIID);
                            items.Add(b);
                        }
                        rdr.Close();
                    }
                }





            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                rdr = null;
                buildItemsDB.Close();
            }
            return items;
        }

        [NonAction]
        private void log(string s)// log the requests to the file
        {
            try
            {
                string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "requests_log.txt");

                TextWriter tw = new StreamWriter(filePath, true);

                tw.WriteLine(s);
                tw.Close();
            }
            catch (ArgumentNullException b)
            {
                string tester = "test";
            }
            catch (System.Web.HttpException h)
            {
                string tester = "test";
            }
            catch (Exception e)
            {
                string tester = "test";
            }


        }

        private string RunProcess(string Parameters)
        {
            //create a process info
            string path = @"C:\Users\537845\Documents\DevWork\ITMService\ITMService\ITMService\FFM\ffmpeg.exe";
            ProcessStartInfo oInfo = new ProcessStartInfo(path, Parameters);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = false;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //Create the output
            string output = null;

            //try the process
            try
            {
                //run the process
                Process proc = System.Diagnostics.Process.Start(oInfo);

                //now put it in a string
                //This needs to be before WaitForExit() to prevent deadlock, for details: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput%28v=VS.80%29.aspx
                output = proc.StandardError.ReadToEnd();

                //Wait for exit
                proc.WaitForExit();

                //Release resources
                proc.Close();
            }
            catch (Exception)
            {
                output = string.Empty;
            }

            return output;
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
        [ExcFilter]
        public BuildItem GetAllItems()
        {
            throw new NotImplementedException("This method is not supported");
        }

    }
}