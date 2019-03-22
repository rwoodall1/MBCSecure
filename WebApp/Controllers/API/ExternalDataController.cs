
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
        private static string _user = "salesforce";
        private static string _password = "4ca508a3-5d7b-49bf-bab2-a95a08530840";
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
            var basicAuthIsValid = username ==_user && password ==_password;
            return basicAuthIsValid;
        }

     
        [HttpGet]
        [Route("salesForceDataRetrievel")]
        public async Task<IHttpActionResult> SalesForceDataRetrievel()
        {
            var processingResult = new ServiceProcessingResult<List<SalesForceBindingModel>> { IsSuccessful = true };
            if (!RequestIsAuthorized())
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }
            string constr = ConfigurationManager.AppSettings["OleConnection"].ToString();
            using (OleDbConnection Conn = new OleDbConnection(constr))//10.37.32.50
                                                                      
            //using (OleDbConnection Conn = new OleDbConnection("Provider = VFPOLEDB.1; Data Source = E:\\MbcData\\MBC5"))
           // using (OleDbConnection Conn = new OleDbConnection("Provider = VFPOLEDB.1; Data Source = M:\\MBC5"))
            {
                using(OleDbCommand cmd=new OleDbCommand("SELECT CUST.Schcode as MemorybookCode, CUST.OracleCode AS OracleNumber,CUST.Contryear As ContractYear,Cust.contdate As ContractLoadDate,Cust.prevpublisher AS Publisher,CUST.stage As Staging,Quotes.nopages As Pages,Quotes.nocopies As Copies,Quotes.fbkprc As BookPrice, Quotes.sbtot As SubTotal,CsNames.Id as CSR,Quotes.invno AS InvoiceNumber,Quotes.agreedte AS AgreementRecDate,Quotes.onlinecuto As OnlinePayCloseOutDate ,Quotes.agreerec AS Agreementreceived ,Produtn.cstat As Status,Produtn.dedayin AS DeadLineDayIn,Produtn.shpdate AS Shipdate,Produtn.kitrecvd AS KitReceivedDate,Produtn.jobno AS JobNo,Produtn.screcv AS SpecCvrRecDate,Produtn.covertype AS CoverType from CUST LEFT JOIN Csnames ON Cust.csrep = CSNames.Source INNER JOIN QUOTES ON Cust.schcode = Quotes.schcode INNER JOIN (Select Quotes.Schcode, MAX(QUOTES.INVNO) AS maxinvno FROM quotes GROUP BY quotes.schcode ORDER BY schcode ) tmp ON Quotes.schcode = tmp.schcode AND Quotes.invno = tmp.maxinvno INNER JOIN produtn ON Quotes.invno = produtn.invno WHERE Cust.Contryear >= '16' Or Cust.Contryear = '17' Or Cust.contryear = '18' ORDER BY CUST.OracleCode", Conn))
                    
                {
                    try
                    {
                        var dt = new DataTable();
                        cmd.CommandType = CommandType.Text;
                        Conn.Open();
                        using (OleDbDataAdapter a = new OleDbDataAdapter(cmd))
                        {
                            a.Fill(dt);
                            List<SalesForceBindingModel> lCust = new List<SalesForceBindingModel>();
                            ExceptionlessClient.Default.CreateLog("SaleForce Data Log")
                                  .AddTags("SaleForce Data Log")
                                  .AddObject("Number 0f Rows=" + dt.Rows.Count.ToString())
                                  .AddObject(DateTime.Now)
                                  .Submit();

                            var rownum = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                rownum = rownum + 1;
                                var rec = new SalesForceBindingModel()
                                {
                                    MemorybookCode = row["MemorybookCode"].ToString().Trim(),
                                    OracleNumber = row["OracleNumber"].ToString().Trim(),
                                    ContractYear = row["ContractYear"].ToString().Trim(),
                                    ContractLoadDate = ((DateTime)row["ContractLoadDate"]).ToString("d"),
                                    Staging = row["Staging"].ToString().Trim(),
                                    Publisher = row["Publisher"].ToString().Trim(),
                                    Copies = row["Copies"].ToString().Trim(),
                                    Pages = row["Pages"].ToString().Trim(),
                                    BookPrice = row["BookPrice"].ToString().Trim(),
                                    SubTotal = row["SubTotal"].ToString().Trim(),
                                    Status = row["Status"].ToString().Trim(),
                                    CSR = row["CSR"].ToString().Trim(),
                                    Shipdate = ((DateTime) row["Shipdate"]).ToString("d"),
                                    RowNumber = rownum.ToString(),
                                    InvoiceNumber=((decimal) row["InvoiceNumber"]).ToString(),
                                    AgreementReceived= (bool)row["Agreementreceived"],
                                    AgreementRecDate= ((DateTime)row["AgreementRecDate"]).ToString("d"),
                                    KitReceivedDate = ((DateTime)row["KitReceivedDate"]).ToString("d"),
                                    JobNo= row["JobNo"].ToString().Trim(),
                                    SpecCvrRecDate= ((DateTime)row["SpecCvrRecDate"]).ToString("d"),
                                    CoverType= row["CoverType"].ToString() == "SPE" ? "SPE":"STAN"

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
                          .SetMessage("Error retrieving Sales Force Data")
                          .AddTags("Sales Force")
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