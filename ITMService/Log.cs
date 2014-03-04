using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ITMService
{
    public class Log
    {
 
        public static void LogIt(string s)// log the requests to the file
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
    }
}