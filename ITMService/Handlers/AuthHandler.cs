using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace ITMService.Handlers
{
    public class AuthHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           
            // Call the inner handler.

            HttpRequestHeaders h = request.Headers;
            Log.LogIt("WE at least got here");
            if (h.Authorization != null)
            {
                Log.LogIt("AUTHORIZATION is THERE!!!");
                Log.LogIt("value: " + h.Authorization);
            }
            else
            {
                Log.LogIt("no Authorization passed --- ");
            }

            var response =  base.SendAsync(request, cancellationToken);
            
            return response;
        }
    }
}