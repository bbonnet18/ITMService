using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITMService.Models
{
    public class BuildItem
    {
        public int buildItemID { get; set; }
        public string caption { get; set; }
        public string fileName { get; set; }
        public int orderNumber { get; set; }
        public string status { get; set; }
        public string thumbnailPath { get; set; }
        public DateTime timeStamp { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string buildID { get; set; }
        public string buildItemIDString { get; set; }
        /*
         *  type - str
            orderNumber - str (should be number)
            title - str
            caption - str
            buildID - str
            file - data
            applicationID - str (should be number), but not sure if you can represent a number in form data


         * */
    }
}