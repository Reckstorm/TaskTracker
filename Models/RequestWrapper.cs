using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RequestWrapper
    {
        public RequestWrapper(Requests requestType, string requestBody)
        {
            RequestType = requestType;
            RequestBody = requestBody;
        }

        public RequestWrapper() {}

        public RequestWrapper(Requests requestType, string requestBody, string username, string password)
        {
            RequestType = requestType;
            RequestBody = requestBody;
            Username = username;
            Password = password;
        }

        public Requests RequestType { get; set; }
        public string RequestBody { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
