using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using JWT;

namespace AuthService.Controllers
{
    public class AuthController : Controller
    {
        private readonly Dictionary<string, string> PASSWORD_MAP = 
            new Dictionary<string, string>()
            {
                { "tea", "cup" },
                { "thymine", "dimers" }
            };
        
        private readonly string USERNAME_KEY = "username";
        private readonly string PASSWORD_KEY = "password";
            
        // NOTE: In 'Production' code this would be stored in a file 
        //       or some other location not in the code.
        private readonly string SECRET_KEY = "TOPSECRET";

        [HttpPost]
        public IActionResult Login()
        {
            var keys = Request.Form.Keys;

            string username = null;
            if (keys.Contains(USERNAME_KEY)) {
                username = Request.Form[USERNAME_KEY];
            }

            // NOTE: In 'Production' code this would get stashed in an encrypted 
            //       variable to prevent easy stealing of passwords and wiped as soon 
            //       as done using it.  Also wouldn't pass over the wire in the clear 
            //       nor without SSL.
            string password = null;
            if (keys.Contains(PASSWORD_KEY)) {
                password = Request.Form[PASSWORD_KEY];
            }
            
            if (!Authenticate(username, password)) {
                return this.HttpNotFound();                
            }

            var payload = new Dictionary<string, object>()
            {
                { "username", username }
            };

            string token = JWT.JsonWebToken.Encode(
                payload, 
                SECRET_KEY, 
                JWT.JwtHashAlgorithm.HS256);

            ViewData["Message"] = token;
            return Content(token, "text/plain", System.Text.Encoding.UTF8);
        }
        
        private bool Authenticate(string username, string password) {
            bool authenticated = false;
            if (username != null && password != null) {
                if (PASSWORD_MAP.ContainsKey(username) &&
                    PASSWORD_MAP[username] == password) 
                {
                    authenticated = true;
                }
            }
            
            return authenticated;
        }
        
        [HttpGet]
        public IActionResult Introspect(string jwt)
        {
            try {
                return Content(
                    JWT.JsonWebToken.Decode(jwt, SECRET_KEY), 
                    "text/json", 
                    System.Text.Encoding.UTF8);
            } catch (System.Exception) {
                return this.HttpUnauthorized();
            }
        }

    }
}
