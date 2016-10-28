using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LS.WebApp.Utilities
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private readonly string _message;

        public ExceptionHandlingAttribute(string message)
        {
            _message = message;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            //Log Critical errors
            Debug.WriteLine(context.Exception);
            ServiceProcessingResult<string> result = new ServiceProcessingResult<string>
            {
                Error = new ProcessingError(_message, _message, true),
                IsSuccessful = false,
                Data = ""
            };
            var settings = new JsonSerializerSettings {ContractResolver = new CamelcaseContractResolver()};
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(result, Formatting.Indented, settings)),
                ReasonPhrase = "Critical Exception"
            });
        }

        public class CamelcaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
            }
        }
    }
}