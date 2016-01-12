using System.Net;
using System.Web.Mvc;

namespace FODT
{
    public class HttpBadRequestResult : HttpStatusCodeResult
    {
        public HttpBadRequestResult()
            : base(HttpStatusCode.BadRequest)
        { }

        public HttpBadRequestResult(string statusDescription)
            : base(HttpStatusCode.BadRequest, statusDescription)
        { }
    }
}