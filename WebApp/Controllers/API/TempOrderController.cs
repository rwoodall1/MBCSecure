using Core;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Net;
using ApiBindingModels;
using Newtonsoft.Json;
using Utilities;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Net.Http.Headers;
using Domain;
using Services;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Exceptionless;

namespace WebApp.Controllers.API {
    [RoutePrefix("api/tempOrder")]
    public class TempOrderController : BaseApiController {

        [HttpGet]
        [Route("getOrder")]
        public async Task<IHttpActionResult> GetOrder(int orderid) {
           
                var processingResult = new ServiceProcessingResult<List<OrderBindingModel>> { IsSuccessful = true };
            try {
                var sqlQuery = "SELECT * FROM temporders where orderid=@OrderId";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@OrderId",orderid) };
                var sqlQueryService = new SQLQuery();
                var orderResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                if (!orderResult.IsSuccessful){
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error retrieving order.", "Error retrieving order.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                    return Ok(processingResult);
                }

                if (orderResult.Data.Rows.Count > 0) {
                    List<OrderBindingModel> Orders= new List<OrderBindingModel>();
                    foreach (DataRow row in orderResult.Data.Rows)
                    {
                        var aa = row["icon1"];
                        var b = aa.GetType();
                        OrderBindingModel order = new OrderBindingModel()
                        {
                            Id = (UInt32)row["id"],// will not be null primary key of temporders
                            OrderId = (UInt32)row["orderid"],//if fail because null good, don't want a order with out an id.
                            BookType = row["booktype"].ToString(),
                            Teacher = row["teacher"].ToString(),
                            PersText1 = row["perstext1"].ToString(),
                            Studentfname = row["studentfname"].ToString(),
                            Studentlname = row["studentlname"].ToString(),
                            Emailaddress = row["emailaddress"].ToString(),
                            Schcode = row["schcode"].ToString(),
                            ItemAmount = row["itemamount"] == DBNull.Value ? 0 : (decimal)row["itemamount"],
                            Itemqty = row["itemqty"]==DBNull.Value ? 0 :(UInt32)row["itemqty"],
                            Schinvoicenumber = row["schinvoicenumber"]==DBNull.Value? 0:(UInt32)row["schinvoicenumber"],
                            Orddate =row["orddate"] == DBNull.Value ? DateTime.Now : (DateTime)row["orddate"],
                            ItemTotal = row["itemtotal"] == DBNull.Value ? 0 : (decimal)row["itemtotal"],
                            Schname = row["schname"].ToString(),
                            Yr = row["yr"].ToString(),
                            Icon1 = row["icon1"]==DBNull.Value? 0:(UInt32)row["icon1"],
                            Icon2 = row["icon1"]==DBNull.Value? 0:(UInt32)row["icon2"],
                            Icon3 = row["icon1"]==DBNull.Value? 0:(UInt32)row["icon3"],
                            Icon4 = row["icon1"]==DBNull.Value? 0:(UInt32)row["icon4"],
                            Josicon1 = row["josicon1"].ToString(),
                            Josicon2 = row["josicon2"].ToString(),
                            Josicon3 = row["josicon3"].ToString(),
                            Josicon4 = row["josicon4"].ToString()
                        };
                        Orders.Add(order);
                    }        
                    processingResult.Data =Orders;
                    processingResult.IsSuccessful = true;
                }
                else
                {
                    processingResult.IsSuccessful =false;
                    processingResult.Error = new ProcessingError("Failed to retrieve order", "Failed to retrieve order", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, "Order was not found.", "Error");
                   
                }

            } catch (Exception ex) {

                ex.ToExceptionless().Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to retrieve order", "Failed to retrieve order.", true, false);
            }
            return Ok(processingResult);
        }
        [HttpGet]
        [Route("getBillingInfo")]
        public async Task<IHttpActionResult> GetBillingInfo(int orderid)
        
            {

            var processingResult = new ServiceProcessingResult<List<OrderBillingBindingModel>> { IsSuccessful = true };
          
                var sqlQuery = "SELECT distinct Schcode,Schname,Schinvoicenumber,Emailaddress,Studentfname,Studentlname,SalesTax,Sum(itemtotal) as Total FROM `temporders` o where Orderid =@OrderId";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@OrderId", orderid) };
                var sqlQueryService = new SQLQuery();
                var orderResult = await sqlQueryService.ExecuteReaderAsync<OrderBillingBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!orderResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error retrieving billing information.", "Error retrieving billing information.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                    return Ok(processingResult);
                }

                if (orderResult.Data!=null)
                {
                var vResult = (List<OrderBillingBindingModel>)orderResult.Data;
                var vTaxRate = vResult[0].SalesTax;
                var vTotal = vResult[0].Total + (vResult[0].Total * vTaxRate);
                 vResult[0].Total = vTotal;
                 processingResult.Data = vResult; ;
                    processingResult.IsSuccessful = true;
                }
                else
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Failed to retrieve billing information.", "Failed to retrieve billing information.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, "Failed to retrieve billing information.", "Error");

                }

          
           
            return Ok(processingResult);
        }




    }
}