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
    public class UsersController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> GetUserByEmail(string email)
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void AddUser(User value)
        {
            if (ModelState.IsValid)
            {
                string test = "";
            }
        }

    }
}