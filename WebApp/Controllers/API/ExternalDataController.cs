
using System.Data;
using System.Data.OleDb;
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
using System.Web;
using System.Text;
using Exceptionless;
using Exceptionless.Models;
namespace WebApp.Controllers.API
{
    [RoutePrefix("api/external")]
    public class ExternalDataController : ApiController
    {
        private static readonly string BasicAuthType = "Basic";
        private static readonly string AuthHeader = "Authorization";
        private bool RequestIsAuthorized()
        {
            var authHeader = HttpContext.Current.Request.Headers[AuthHeader];
            if (authHeader == null || !authHeader.StartsWith(BasicAuthType))
            {
                return false;
            }
            var encodedUsernamePassword = authHeader.Substring((BasicAuthType + " ").Length).Trim();
            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var credsArray = usernamePassword.Split(':');
            var username = credsArray[0];
            var password = credsArray[1];

            //var basicAuthIsValid = username == ApplicationConfig.WebAPIUsername && password == ApplicationConfig.WebAPIPassword;
            var basicAuthIsValid = username == "test" && password == "test";
            return basicAuthIsValid;
        }

     
        [HttpGet]
        [Route("testData")]
        public async Task<IHttpActionResult> testData()
        {
            var processingResult = new ServiceProcessingResult<List<TestBindingModel>> { IsSuccessful = true };
            if (!RequestIsAuthorized())
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            //using (OleDbConnection Conn = new OleDbConnection("Provider = VFPOLEDB.1; Data Source = M:\\MBC5"))
            using (OleDbConnection Conn = new OleDbConnection("Provider = VFPOLEDB.1; Data Source = E:\\MbcData\\MBC5"))
            {
                using(OleDbCommand cmd=new OleDbCommand("select Top(10) CUST.SCHNAME,CUST.SCHCODE from CUST where upper(CUST.SCHNAME) like 'A%' ORDER BY Schname", Conn))
                {
                    try
                    {
                        var dt = new DataTable();
                        cmd.CommandType = CommandType.Text;
                        Conn.Open();
                        using (OleDbDataAdapter a = new OleDbDataAdapter(cmd))
                        {
                            a.Fill(dt);
                            List<TestBindingModel> lCust = new List<TestBindingModel>();
                            foreach (DataRow row in dt.Rows)
                            {
                                var rec = new TestBindingModel()
                                {
                                    schcode = row["schcode"].ToString().Trim(),
                                    schname = row["schname"].ToString().Trim()
                                };
                                lCust.Add(rec);
                               

                            }
                            processingResult.Data = lCust;
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        processingResult.IsSuccessful = false;
                        ex.ToExceptionless()
                          .SetMessage("Error running TestData")
                          .AddTags("Test Function")
                          .AddObject(cmd)
                          .MarkAsCritical()
                          .Submit();
                        processingResult.Error = new ProcessingError(ex.Message, ex.Message,true, false);
                        return Ok(processingResult);
                    }
                    finally
                    {
                        Conn.Close();
                    }





                }


            }
            

           
            return Ok(processingResult);

        }
    

    }
}