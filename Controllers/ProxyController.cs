using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CrApiProxy.Controllers
{
    public class ProxyController : Controller
    {
        [HttpPost]
        [HttpGet]
        [Route("{*remaining}")]
        //public async Task<object> Get()
        public async Task<ActionResult> Get()
        {
            // Official API key.
            string API_KEY = Environment.GetEnvironmentVariable("OFFICIAL_API_KEY");

            // rewrite the request path.
            const string host = "api.clashroyale.com";
            string forwardUri = $"https://{host}{Request.GetEncodedPathAndQuery()}";

            // remove the host header from the original sender.
            Request.Headers.Remove("Host");

            // create the new request with headers.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_KEY);

            // forward the request to the official API.
            var response = await client.GetAsync(forwardUri);

            // convert the response to a JSON string.
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            
            // check for an error.
            if (response.IsSuccessStatusCode)
            {
                // no error, return the result.
                return Ok(result);
            }
            else
            {
                // error, pass it to the client to handle.
                return StatusCode((int)response.StatusCode, result);
            }
        }
    }
}
