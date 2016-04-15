using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using JWT;

namespace AuthService.Controllers
{
    public class UserController : Controller
    {
        // NOTE: In 'Production' code this would be stored in a file 
        //       or some other location not in the code.
        private readonly string SECRET_KEY = "TOPSECRET";

        public class UserInfoXml {
            [JsonProperty(PropertyName = "username")]
            public string UserName { get; set; }
        }

        [HttpGet]
        public IActionResult Info(string jwt)
        {
            try {
                var payload = JWT.JsonWebToken.Decode(jwt, SECRET_KEY);
                UserInfoXml payloadObject = JsonConvert.DeserializeObject<UserInfoXml>(payload);
                
                User user = RetrieveUser(payloadObject.UserName);

                return Content(
                    user.toXml(),
                    "text/xml", 
                    System.Text.Encoding.UTF8);
            } catch (System.Exception) {
                return this.HttpUnauthorized();
            }
        }

        private User RetrieveUser(string username)
        {
            // NOTE: Would pull username's info from database or 
            //       perhaps another service.
            string[] firstNames = { "Rick", "Alexander", "Bob", "Sen", "Linda",
                                     "Keith", "Chris", "Brett", "Woody", 
                                     "Ralph", "Holick", "Grant", "Benjamin",
                                     "Anne", "Tina", "Eric", "Henry", 
                                     "Khan" };
                                     
            var rand = new System.Random();
            string firstName = firstNames[rand.Next(0, firstNames.Length - 1)];

            Address address = new Address(
                "343 Winslow Way E.", 
                "98110", 
                "Bainbridge Island",
                "Nunavet");
            
            User user = new User(
                System.Guid.NewGuid().ToString(),
                username,
                firstName,
                "Goodsun",
                (firstName + "@mailinator.com"),
                address);
                                
            return user;
        }

    }
    
    // TODO: Should move these into a 'model' namespace.
    public class Address {
        public string Line1 { get; private set; }
        public string PostalCode { get; private set; }
        public string City { get; private set; }
        public string Province { get; private set; }
        
        public Address(
            string line1,
            string postalCode,
            string city,
            string province
        ) 
        {
            this.Line1 = line1;
            this.PostalCode = postalCode;
            this.City = city;
            this.Province = province;
        }
        
        public string toXml()
        {
            string xml = "{ \"line1\": \"" + this.Line1 + 
                "\", \"postal_code\": \"" + this.PostalCode + "\", \"city\": \"" +
                this.City + "\", \"province\": \"" + 
                this.Province + "\" }";
                
            return xml;
        }
    }
    
    public class User {
        // NOTE: Would validate/constrain User attributes length and content 
        //       in 'Production' code.  Each attribute would have its own type 
        //       to prevent mishaps in calling constructor/using the attributes.
        public string Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public Address Address { get; private set; }
        
        public User(
            string id,
            string username,
            string firstName,
            string lastName,
            string email,
            Address address
        ) {
            this.Id = id;
            this.Username = username;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Address = address;
        }
        
        public string toXml() {
            // NOTE: Could have used JsonConvert but didn't discover it until 
            //       too late to change.     
            string xml = "{\"id\": \"" + this.Id + "\", \"username\": \"" +
                this.Username + "\", \"first_name\": \"" + this.FirstName + 
                "\", \"last_name\": \"" + this.LastName + "\", \"email:\": \"" + 
                this.Email + "\", \"address\": " + this.Address.toXml() + 
                ", \"is_deleted\": 0 }";
                
            return xml;
        }    

    }
}
