using ApiBindingModels;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Domain;
using Services;
using Utilities;
using System.Drawing;
using Repositories.DBContext;
using System.Configuration;

namespace WebApp.Controllers.API {
    [RoutePrefix("api/utility")]
    public class UtilityController : ApiController {
        [HttpGet]
        [Route("getServerVars")]
        public async Task<IHttpActionResult> GetServerVars( ) {
            var result = new ServerVarsBindingModel();
            var getEnvironmentResult = ConfigurationSettings.AppSettings["Environment"].ToString();

            result.Environment = getEnvironmentResult;
            return Ok(result);
        }
    }
}    