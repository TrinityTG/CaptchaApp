using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CaptchaApp.Controllers
{
    public class myClass
    {
        public string g_recaptcha_response { get; set; }

    }


    public class GoogleResponse
    {
        public string success { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
        public List<string> errorCodes { get; set; }
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Register()
        {
            ViewData["googleResponse-success"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection formClass )
        {

            var recaptchaResponse = formClass["g-recaptcha-response"];
            var lastName = formClass["LastName"];
            var firstName = formClass["FirstName"];

            ViewData["googleResponse-success"] = "";

            // -- ----------------------------------------------------------------------------------
            // This is the google api that we want to call with our secret code
            // https://www.google.com/recaptcha/api/siteverify
            // -- ----------------------------------------------------------------------------------
            string secretCode = "--- you need to enter your secret code here ---";
            string baseAddress = "https://www.google.com";
            string requestUri = "/recaptcha/api/siteverify";

            GoogleResponse googleResponse = new GoogleResponse();
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(new[]  {
                    new KeyValuePair<string, string>("secret", secretCode),
                    new KeyValuePair<string, string>("response", recaptchaResponse),
                    new KeyValuePair<string, string>("remoteip", "")
                });

                // -- ----------------------------------------------------------------------------------
                // Post to Google
                // -- ----------------------------------------------------------------------------------
                HttpResponseMessage apiResponse = client.PostAsync(requestUri, content).Result;
                // -- ----------------------------------------------------------------------------------

                if (apiResponse.IsSuccessStatusCode)
                {
                    string jsonContent = apiResponse.Content.ReadAsStringAsync().Result;
                    
                    googleResponse = jsSerializer.Deserialize<GoogleResponse>(jsonContent);


                    // -- ----------------------------------------------------------------------------------
                    // We care about this: 
                    //      googleResponse.success
                    // it would be either true or false
                    // -- ----------------------------------------------------------------------------------
                    ViewData["googleResponse-success"] = googleResponse.success;

                }
                else
                {
                    // something bad happened
                }

                return View();
            }
        }

    }
}