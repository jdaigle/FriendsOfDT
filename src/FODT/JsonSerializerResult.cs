using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FODT
{
    public class JsonSerializerResult : ActionResult
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        public JsonSerializerResult()
        {
        }

        public JsonSerializerResult(object data)
        {
            Data = data;
        }

        public HttpStatusCode? StatusCode { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (StatusCode != null)
            {
                response.StatusCode = (int)StatusCode.Value;
            }

            if (Data != null)
            {
                var serialized = JsonConvert.SerializeObject(Data, settings);
                response.Write(serialized);
            }
        }
    }
}