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

        public RequestWrapper(Requests requestType, string requestBody, User? user, bool isAuthorized = false)
        {
            RequestType = requestType;
            RequestBody = requestBody;
            User = user;
            IsAuthorized = isAuthorized;
        }

        public RequestWrapper(Requests requestType, string requestBody, bool isAuthorized = false)
        {
            RequestType = requestType;
            RequestBody = requestBody;
            IsAuthorized = isAuthorized;
        }

        public Requests RequestType { get; set; }
        public string RequestBody { get; set; }
        public User? User { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
