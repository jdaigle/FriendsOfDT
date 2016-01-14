using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace FODT
{
    public class HttpApiResult
    {
        [JsonIgnore]
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;

        public string Message { get; set; }
        public string RedirectToURL { get; set; }
    }
}