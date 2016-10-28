using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.ModelBinding;
using Common.Logging;
//using Utilities;


namespace WebApp.Controllers.API
{
    public class BaseApiController : ApiController
    {
        protected ILog Logger { get; set; }

        protected BaseApiController()
        {
            //Logger = LoggerFactory.GetLogger(GetType());
        }

        protected static string GetModelStateErrorsAsString(ModelStateDictionary modelState,
            string errorSeparator = "\n")
        {
            var errorList = modelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList();
            return String.Join(errorSeparator, errorList);
        }
    }
}
