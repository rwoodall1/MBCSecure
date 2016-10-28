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
using Exceptionless.Models;
using System.Net.Mail;
namespace WebApp.Controllers.API
{
    [RoutePrefix("api/order")]
    public class OrderController : BaseApiController
    {

        [HttpPost]
        [Route("submitAuthNet")]
        public async Task<IHttpActionResult> AuthNetSubmit(AuthNetBindingModel model)
        {

            var processingResult = new ServiceProcessingResult<List<OrderBindingModel>> { IsSuccessful = true };
            //Get the order first thing to make sure we have it.
            List<OrderBindingModel> Orders = new List<OrderBindingModel>();
           
            try
            {
                var sqlQuery = "SELECT Id,OrderId,PayType,Grade,BookType,Teacher,PersText1,Studentfname,Studentlname,Emailaddress,Schcode,ItemAmount,Itemqty,Schinvoicenumber,Orddate,ItemTotal,Schname,Yr,Icon1,Icon2,Icon3,Icon4,Josicon1,Josicon2,Josicon3,Josicon4 FROM temporders where orderid=@OrderId";
         
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@OrderId", model.InvoiceNumber) };
                var sqlQueryService = new SQLQuery();
                var orderResult = await sqlQueryService.ExecuteReaderAsync<OrderBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!orderResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error retrieving order.", "Error retrieving order.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                    return Ok(processingResult);
                }
                if (orderResult.Data == null)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error retrieving order.", "Error retrieving order.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                    return Ok(processingResult);
                }


                Orders = (List<OrderBindingModel>)orderResult.Data;

            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving order.", "Error retrieving order.", true, false);

                return Ok(processingResult);

            }

            //----------------------------------------------------------------------

            var authNetService = new AuthNetService();
            var result = await authNetService.SubmittAsync(model);
            if (!result.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error submiting payment to Authorzie.net", "Error submiting payment to Authorzie.net", true, false);
                ExceptionlessClient.Default.SubmitLog(typeof(OrderController).FullName, result.Error.UserHelp, "Error");
                return Ok(processingResult);

            }
            AuthNetResponse AuthNetData = new AuthNetResponse();
            AuthNetData = result.Data;
            if (!AuthNetData.Approved)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Your payment to Authorized.net failed for the following reason:" + AuthNetData.Message, "Your payment to Authorized.net failed for the following reason:" + AuthNetData.Message, true, false);
                return Ok(processingResult);
            }//not approved
            else//Is Approved
            {
                foreach (var order in Orders)
                {
                    var sqlQuery1 = "INSERT INTO Orders (grade,booktype,teacher,perstext1,studentfname,studentlname,emailaddress,schcode,itemamount,itemqty,schinvoicenumber,orderid,orddate,paytype,itemtotal,schname,parentpayment,yr,icon1,icon2,icon3,icon4,josicon1,josicon2,josicon3,josicon4) VALUES(@grade,@booktype,@teacher,@perstext1,@studentfname,@studentlname,@emailaddress,@schcode,@itemamount,@itemqty,@schinvoicenumber,@orderid,@orddate,@paytype,@itemtotal,@schname,@parentpayment,@yr,@icon1,@icon2,@icon3,@icon4,@josicon1,@josicon2,@josicon3,@josicon4)";

                    MySqlParameter[] parameters = new MySqlParameter[] {

                    new MySqlParameter("@grade",order.Grade),
                    new MySqlParameter("@booktype",order.BookType),
                    new MySqlParameter("@teacher", order.Teacher),
                    new MySqlParameter("@perstext1",order.PersText1),
                    new MySqlParameter("@studentfname",order.Studentfname),
                    new MySqlParameter("@studentlname",order.Studentlname),
                    new MySqlParameter("@emailaddress",order.Emailaddress),
                    new MySqlParameter("@schcode",order.Schcode),
                    new MySqlParameter("@itemamount",order.ItemAmount),
                    new MySqlParameter("@itemqty",order.Itemqty),
                    new MySqlParameter("@schinvoicenumber",order.Schinvoicenumber),
                    new MySqlParameter("@orderid",order.OrderId),
                    new MySqlParameter("@orddate",order.Orddate),
                    new MySqlParameter("@paytype",order.PayType),
                    new MySqlParameter("@itemtotal",order.ItemTotal),
                    new MySqlParameter("@schname",order.Schname),
                    new MySqlParameter("@parentpayment",1),
                    new MySqlParameter("@yr",order.Yr),
                    new MySqlParameter("@icon1", order.Icon1),
                    new MySqlParameter("@icon2", order.Icon2),
                    new MySqlParameter("@icon3",order.Icon3),
                    new MySqlParameter("@icon4",order.Icon4),
                    new MySqlParameter("@josicon1",order.Josicon1),
                    new MySqlParameter("@josicon2",order.Josicon2),
                    new MySqlParameter("@josicon3",order.Josicon3),
                    new MySqlParameter("@josicon4",order.Josicon4),

                };
                    try
                    {
                        var sqlQueryService = new SQLQuery();
                        var orderResult = await sqlQueryService.ExecuteNonQueryAsync(CommandType.Text, sqlQuery1, parameters);
                        if (!orderResult.IsSuccessful)
                        {
                            ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                            //create the mail message

                            MailMessage mail = new MailMessage();
                            //set the addresses
                            mail.From = new MailAddress(ConfigurationManager.AppSettings["FromAddr"]);
                            mail.To.Add("randy@woodalldevelopment.com");
                            //set the content
                            mail.Subject = "Mysql Error:Inserting Order " + order.Schname.ToString() + "(" + order.Schcode.ToString() + ")";
                            mail.Body = "An error occured inserting a order record into the Mysql database server. The following data was not recorded in the order table.<br/>School Name:" + order.Schname +
                            "<br/>Student Name:" + order.Studentfname + " " + order.Studentlname +
                            "<br/>School Code:" + order.Schcode +
                            "<br/>Order Id:" + model.InvoiceNumber +
                            "<br/>Grade:" + order.Grade +
                            "<br/>Teacher:" + order.Teacher +
                            "<br/>Book Type:" + order.BookType +
                            "<br/>EmailAddress:" + order.Emailaddress +
                            "<br/>Perstext1:" + order.PersText1 +
                            "<br/>Item Amount:" + order.ItemAmount +
                            "<br/>Item Total:" + order.ItemTotal +
                            "<br/>Item Qty:" + order.Itemqty +
                            "<br/>icon1:" + order.Icon1 +
                            "<br/>icon2:" + order.Icon2 +
                            "<br/>icon3:" + order.Icon3 +
                            "<br/>icon4:" + order.Icon4 +
                            "<br/>Year:" + order.Yr +
                            "<br/><br/>Mysql Exception Code:" + orderResult.Error.UserMessage;
                            mail.IsBodyHtml = true;
                            //send the message
                            SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpuser"], ConfigurationManager.AppSettings["smtppassword"]);
                            //smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis 'only works on some serves
                            try
                            {
                                smtp.Send(mail);
                            }
                            catch (Exception ex)
                            {
                                //go on don't stop because email cant be sent.
                            }
                        }
                        if (orderResult.Data == 0)
                        {
                            ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, "Failed to insert an order", "Error");

                            MailMessage mail = new MailMessage();
                            //set the addresses
                            mail.From = new MailAddress(ConfigurationManager.AppSettings["FromAddr"]);
                            mail.To.Add("randy@woodalldevelopment.com");
                            //set the content
                            mail.Subject = "Mysql Error:Inserting Order " + order.Schname.ToString() + "(" + order.Schcode.ToString() + ")";
                            mail.Body = "An error occured inserting a order record into the Mysql database server. The following data was not recorded in the order table.<br/>School Name:" + order.Schname +
                            "<br/>Student Name:" + order.Studentfname + " " + order.Studentlname +
                            "<br/>Order Id:" + model.InvoiceNumber +
                            "<br/>School Code:" + order.Schcode +
                            "<br/>Grade:" + order.Grade +
                            "<br/>Teacher:" + order.Teacher +
                            "<br/>Book Type:" + order.BookType +
                            "<br/>EmailAddress:" + order.Emailaddress +
                            "<br/>Perstext1:" + order.PersText1 +
                            "<br/>Item Amount:" + order.ItemAmount +
                            "<br/>Item Total:" + order.ItemTotal +
                            "<br/>Item Qty:" + order.Itemqty +
                            "<br/>icon1:" + order.Icon1 +
                            "<br/>icon2:" + order.Icon2 +
                            "<br/>icon3:" + order.Icon3 +
                            "<br/>icon4:" + order.Icon4 +
                            "<br/>Year:" + order.Yr +
                            "<br/><br/>Mysql Exception Code:" + orderResult.Error.UserMessage;
                            mail.IsBodyHtml = true;
                            //send the message
                            SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpuser"], ConfigurationManager.AppSettings["smtppassword"]);
                            //smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis 'only works on some serves
                            try
                            {
                                smtp.Send(mail);
                            }
                            catch (Exception ex)
                            {
                                //go on don't stop because email cant be sent.
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        ex.ToExceptionless().Submit();

                    }

                }//endforeach
                 //,  
                 //insert payment even if order failed
                try {
                    var sqlQuery = "INSERT INTO Payment (orderid,schname,schcode,custemail,ddate,poamt,paytype,ccnum,invno,parentpay,payerfname,payerlname,addr,city,state,zip,transid,authcode) VALUES(@orderid,@schname,@schcode,@custemail,@ddate,@poamt,@paytype,@ccnum,@invno,@parentpay,@payerfname,@payerlname,@addr,@city,@state,@zip,@transid,@authcode)";
                    MySqlParameter[] parameters1 = new MySqlParameter[] {

                    new MySqlParameter("@orderid",model.InvoiceNumber),
                    new MySqlParameter("@custemail",model.EmailAddress),
                    new MySqlParameter("@ddate",DateTime.Now),
                    new MySqlParameter("@poamt",model.Amount),
                    new MySqlParameter("@paytype",model.Method),
                    new MySqlParameter("@transid",AuthNetData.TransId),
                    new MySqlParameter("@authcode",AuthNetData.AuthCode),
                    new MySqlParameter("@ccnum",model.Cardnum==null?"":model.Cardnum.Substring(model.Cardnum.Length-3)),
                    new MySqlParameter("@invno",Orders[0].Schinvoicenumber),
                    new MySqlParameter("@schname",Orders[0].Schname),
                    new MySqlParameter("@schcode",AuthNetData.Custid),
                    new MySqlParameter("@parentpay",1),
                    new MySqlParameter("@payerfname",model.FirstName),
                    new MySqlParameter("@payerlname",model.LastName),
                    new MySqlParameter("@addr",model.Address),
                    new MySqlParameter("@city",model.City),
                    new MySqlParameter("@state",model.State.TrimEnd()),
                    new MySqlParameter("@zip",model.Zip)

                };
                    var sqlQueryService1 = new SQLQuery();
                    var payResult = await sqlQueryService1.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, parameters1);
                
                if (!payResult.IsSuccessful)
                {
                    //fail it because we don't have the payment to create a receipt.
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Your payment was made but an error occurred creating your receipt. To obtain a receipt contact your school adviser with this tranasaction id:" + AuthNetData.TransId, "Your payment was made but an error occurred creating your receipt. To obtain a receipt contact your school adviser with this tranasaction id:" + AuthNetData.TransId, true, false);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Error inserting  parent payment.", "Error").AddObject(model).AddObject(AuthNetData);
                    return Ok(processingResult);

                }
                EmailReceipt(model.InvoiceNumber);
                }
                catch (Exception ex)
                {
                ex.ToExceptionless()
                    .SetMessage("Error inserting payment.")
                    .AddTags("Insert Payment Error")
                    .AddObject(model)
                    .AddObject(AuthNetData)
                    .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Your payment was made but an error occurred creating your receipt. To obtain a receipt contact your school adviser with this tranasaction id:" + AuthNetData.TransId, "Your payment was made but an error occurred creating your receipt. To obtain a receipt contact your school adviser with this tranasaction id:" + AuthNetData.TransId, true, false);

                return Ok(processingResult);

            }

            }// End Approved
            return Ok(processingResult);
        }
        [HttpPost]
        [Route("submitSchoolAuthNet")]
        public async Task<IHttpActionResult> SchoolAuthNetSubmit(AuthNetBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };
            var authNetService = new AuthNetService();
            var result = await authNetService.SubmittAsync(model);
            if (!result.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error submiting payment to Authorzie.net", "Error submiting payment to Authorzie.net", true, false);
                ExceptionlessClient.Default.SubmitLog(typeof(OrderController).FullName, result.Error.UserHelp, "Error");
                return Ok(processingResult);

            }
            AuthNetResponse AuthNetData = new AuthNetResponse();
            AuthNetData = result.Data;
            if (!AuthNetData.Approved)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Your payment to Authorized.net failed for the following reason:" + AuthNetData.Message, "Your payment to Authorized.net failed for the following reason:" + AuthNetData.Message, true, false);
                return Ok(processingResult);
            }
            var sqlQuery = "INSERT INTO Payment (schcode,schname,custemail,ddate,poamt,paytype,ccnum,invno,parentpay,payerfname,payerlname,transid,authcode) VALUES(@schcode,@schname,@custemail,@ddate,@poamt,@paytype,@ccnum,@invno,@parentpay,@payerfname,@payerlname,@transid,@authcode)";
            string fname = "";
            string lname = "";
            if (!String.IsNullOrEmpty(model.BankAccName))
            {
                fname = model.BankAccName.Substring(0, model.BankAccName.IndexOf(" ") - 1);
                lname = model.BankAccName.Substring(model.BankAccName.IndexOf(" ") + 1);
            }
            else {
                fname = model.FirstName;
                lname = model.LastName;
            }
            MySqlParameter[] parameters = new MySqlParameter[] {

                    new MySqlParameter("@custemail",model.EmailAddress),
                    new MySqlParameter("@ddate",DateTime.Now),
                    new MySqlParameter("@poamt",model.Amount),
                    new MySqlParameter("@payerfname",fname),
                    new MySqlParameter("@payerlname",lname),
                    new MySqlParameter("@paytype",model.Method),
                    new MySqlParameter("@transid",AuthNetData.TransId),
                    new MySqlParameter("@authcode",AuthNetData.AuthCode),
                    new MySqlParameter("@ccnum",model.Cardnum==null?"":model.Cardnum.Substring(model.Cardnum.Length-3)),
                    new MySqlParameter("@invno",model.InvoiceNumber),
                    new MySqlParameter("@schcode",AuthNetData.Custid),
                    new MySqlParameter("@schname",model.Schname),
                    new MySqlParameter("@parentpay","0"),


                };
            var sqlQueryService = new SQLQuery();
            var payResult = await sqlQueryService.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, parameters);
            if (!payResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Your payment was made but an error occurred creating your receipt. To obtain a receipt contact Memory Book with this tranasaction id:" + AuthNetData.TransId, "Your payment was made but an error occurred creating your receipt. To obtain a receipt contact Memory Book with this tranasaction id:" + AuthNetData.TransId, true, false);
                ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Error inserting school payment.", "Error").AddObject(model).AddObject(AuthNetData);
                return Ok(processingResult);

               

            }
            EmailSchoolReceipt(AuthNetData.TransId);


            processingResult.Data = AuthNetData.TransId;
            return Ok(processingResult);

        }
        protected void EmailSchoolReceipt(string transid)
        {

            //Get the order first thing to make sure we have it.
            ReceiptBindingModel Receipt = new ReceiptBindingModel();
            try
            {
                var sqlQuery = "SELECT Schname,Schcode,PayerFname,PayerLname,Poamt,PayType,TransId,AuthCode,CustEmail,Ddate,OrderId from Payment  where transid=@Transid";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@Transid", transid) };
                var sqlQueryService = new SQLQuery();
                var payResult = sqlQueryService.ExecuteReaderAsync<PaymentBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!payResult.Result.IsSuccessful)
                {

                    ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, payResult.Result.Error.UserMessage, "Error");

                }

                List<PaymentBindingModel> payments = (List<PaymentBindingModel>)payResult.Result.Data;
                PaymentBindingModel payment = payments[0];//should only be one payment
                Receipt.Schname = payment.Schname;
                Receipt.Schcode = payment.Schcode;
                Receipt.PayerFname = payment.PayerFname;
                Receipt.PayerLname = payment.PayerLname;
                Receipt.Payment = payment.Poamt;
                Receipt.PayType = payment.PayType;
                Receipt.TransId = payment.TransId;
                Receipt.AuthCode = payment.AuthCode;
                Receipt.OrderId = payment.OrderId;
                Receipt.CustomerEmail = payment.CustEmail;
                Receipt.OrderDate = payment.Ddate;
                Receipt.Payment = payment.Poamt;

                var body = "";

                var hbody = "<div class='form-group col-md-12'><label style = 'font-size:x-large' ><strong>Thank you for your payment</strong></label></div>"
               + "<div  class='form-group'><div class='col-sm-6'><strong>School Name: </strong>" + payment.Schname + "</div></div>"
               + "<div class='form-group'> <div class='col-sm-6'><strong>School Code: </strong>" + payment.Schcode + "</div></div>"
               + " <div class='form-group'> <div class='col-sm-6'><strong>Payer Name: </strong>" + payment.PayerFname + " " + payment.PayerLname + "</div></div>"
               + " <div class='form-group'> <div class='col-sm-6'><strong>Order Date: </strong>" + payment.Ddate.ToString("MM/dd/yyyy") + "</div></div>"
               + "<div class='form-group'>  <div class='col-sm-6'><strong>Pay Type: </strong>" + payment.PayType + "</div></div>"

               + "<div class='form-group'><div class='col-sm-6'><strong>Transaction Id: </strong>" + payment.TransId + "</div></div>"
               + "<div class='form-group'><div class='col-sm-6'><strong>Authorization Code: </strong>" + payment.AuthCode + "</div></div>"
               + "<div class='form-group'><div class='col-sm-6'><strong>Amount Paid: </strong>$" + payment.Poamt + "</div> </div><div></div>";
                body = hbody;

                //body = body + "<div style='color: red; margin - bottom:5px'><i>If you have questions about your order contact your schools yearbook advisor. </i></div><hr>";



                var emailhelper = new Utilities.EmailHelper();

                emailhelper.SendEmail("Receipt for a School Payment to Memory Book Company (Transaction Id " + Receipt.TransId + ")  Using " + Receipt.PayType + "  " + DateTime.Now.ToShortDateString(), Receipt.CustomerEmail, "", "authnet@memorybook.com", body, Utils.EmailType.Mbc);


            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                    .MarkAsCritical()
                    .SetMessage("Failed to send receipt email.")
                    .Submit();
            }



        }
        protected void EmailReceipt(string orderid)
        {

            //Get the order first thing to make sure we have it.
            ReceiptBindingModel Receipt = new ReceiptBindingModel();
            try
            {
                var sqlQuery = "SELECT Orders.Id,OrderId,PayType,Grade,BookType,Teacher,PersText1,Studentfname,Studentlname,Emailaddress,Schcode,ItemAmount,Itemqty,Schinvoicenumber,Orddate,ItemTotal,Schname,Yr,l1.caption as Caption1,l2.caption As Caption2,l3.caption As Caption3,l4.caption As Caption4 FROM Orders Left Join lookup l1 On l1.ivalue=Orders.icon1 Left Join lookup l2 on l2.ivalue=Orders.icon2 Left Join lookup l3 on l3.ivalue=Orders.icon3  Left Join lookup l4 on l4.ivalue=Orders.icon4  where orderid=@OrderId";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@OrderId", orderid) };
                var sqlQueryService = new SQLQuery();
                var orderResult = sqlQueryService.ExecuteReaderAsync<OrderBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!orderResult.Result.IsSuccessful)
                {

                    ExceptionlessClient.Default.CreateLog(typeof(TempOrderController).FullName, orderResult.Result.Error.UserMessage, "Error");

                }
                Receipt.Items = (List<OrderBindingModel>)orderResult.Result.Data;
                var hasAd = false;
                foreach (var order in Receipt.Items)
                {
                    switch (order.BookType)
                    {
                        case "Full Page Ad":
                            hasAd = true;
                            break;
                        case "1/2 Page Ad":
                            hasAd = true;
                            break;
                        case "1/4 Page Ad":
                            hasAd = true;
                            break;
                        case "1/8 Page Ad":
                            hasAd = true;
                            break;

                    }

                }

                MySqlParameter[] payParameters = new MySqlParameter[] { new MySqlParameter("@OrderId", orderid) };
                var sqlQueryService1 = new SQLQuery();
                sqlQuery = "Select Schcode,PayerFname,PayerLname,Poamt,PayType,TransId, AuthCode,CustEmail,Ddate,OrderId,Schname from Payment where orderid=@OrderId";
                var payResult = sqlQueryService.ExecuteReaderAsync<PaymentBindingModel>(CommandType.Text, sqlQuery, payParameters);
                if (!payResult.Result.IsSuccessful)
                {

                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Result.Error.UserMessage, "Error");


                }
                List<PaymentBindingModel> payments = (List<PaymentBindingModel>)payResult.Result.Data;
                PaymentBindingModel payment = payments[0];//should only be one payment
                Receipt.Schname = payment.Schname;
                Receipt.Schcode = payment.Schcode;
                Receipt.PayerFname = payment.PayerFname;
                Receipt.PayerLname = payment.PayerLname;
                Receipt.Payment = payment.Poamt;
                Receipt.PayType = payment.PayType;
                Receipt.TransId = payment.TransId;
                Receipt.AuthCode = payment.AuthCode;
                Receipt.OrderId = payment.OrderId;
                Receipt.CustomerEmail = payment.CustEmail;
                Receipt.OrderDate = payment.Ddate;
                Receipt.Payment = payment.Poamt;

                var body = "";

                var hbody = "<div class='form-group col-md-12'><label style = 'font-size:x-large' ><strong>Thank you for your payment</strong></label></div>"
               + "<div  class='form-group'><div class='col-sm-6'><strong>School Name: </strong>" + payment.Schname + "</div></div>"
               + "<div class='form-group'> <div class='col-sm-6'><strong>School Code: </strong>" + payment.Schcode + "</div></div>"
               + " <div class='form-group'> <div class='col-sm-6'><strong>Payer Name: </strong>" + payment.PayerFname + " " + payment.PayerLname + "</div></div>"
               + " <div class='form-group'> <div class='col-sm-6'><strong>Order Date: </strong>" + payment.Ddate.ToString("MM/dd/yyyy") + "</div></div>"
               + "<div class='form-group'>  <div class='col-sm-6'><strong>Order Id: </strong>" + payment.OrderId + "</div></div>"
               + "<div class='form-group'>  <div class='col-sm-6'><strong>Pay Type: </strong>" + payment.PayType + "</div></div>"

               + "<div class='form-group'><div class='col-sm-6'><strong>Transaction Id: </strong>" + payment.TransId + "</div></div>"
               + "<div class='form-group'><div class='col-sm-6'><strong>Authorization Code: </strong>" + payment.AuthCode + "</div></div>"
               + "<div class='form-group'><div class='col-sm-6'><strong>Amount Paid: </strong>$" + payment.Poamt + "</div> </div><div></div>";
                body = hbody;
                if (hasAd)
                {
                    body = body + "<div style = 'font-size:larger' ><strong ><a href = 'http://mbcadpages.v5.pressero.com/' target = '_blank'> You have ordered an ad. Click here to configure your ad. (http://mbcadpages.v5.pressero.com/)</a></strong></div>";
                }
                body = body + "<div style='color: red; margin - bottom:5px'><i>If you have questions about your order contact your school's yearbook advisor. </i></div><hr>";
                foreach (var order in Receipt.Items)
                {
                    var items = "<div style='margin:7px'><table>"
                         + "<tr><td style='text-align:right'><strong> Item Quantity: </strong ></td><td>" + order.Itemqty + "</td></tr>"
                    + "<tr><td style='text-align:right'><strong> Item: </strong></td><td> " + order.BookType + "</td></tr>"
                    + "<tr><td style='text-align:right'><strong> Student Name: </strong></td><td> " + order.Studentfname + " " + order.Studentlname + "</td></tr>"
                    + "<tr><td style='text-align:right'><strong> Teacher: </strong></td><td>" + order.Teacher + "</td></tr>"
                    + "<tr><td style='text-align:right'><strong> Grade: </strong></td><td>" + order.Grade + "</td></tr>";
                    var text = "";
                    if (order.PersText1 != "" && order.PersText1 != "Not Available")
                    {
                        if (order.BookType == "Love Line")
                        {
                            text = "<tr><td style='text-align:right'><strong>Love Line: </strong></td><td >" + order.PersText1 + "</td></tr>";
                            items = items + text;
                        }
                        else if (order.BookType != "Love Line" && order.PersText1 != "")
                        {
                            text = "<tr><td style='text-align:right'><strong>Personalized Text :</strong></td><td>" + order.PersText1 + "</td></tr>";
                            items = items + text;
                        }
                    }
                    var icontext = "";
                    if (order.BookType == "Personalized Ink Yearbook" || order.BookType == "Personalized Foil Yearbook")
                    {
                        icontext = "<tr><td style='text-align:right'><strong>Icons: </strong></td><td style='font-size:xx-small'><i>" + order.Caption1 + " " + order.Caption2 + " " + order.Caption3 + " " + order.Caption4 + "</i></td></tr>";
                        items = items + icontext;

                    }
                    items = items + "<tr><td style='text-align:right'>Item Amount: </td><td>$" + order.ItemTotal + "</td ></tr></table></div><div></div><hr>";
                    body = body + items;

                }


                var emailhelper = new Utilities.EmailHelper();
               
                emailhelper.SendEmail("Receipt for a Parent Payment to Memory Book Company (Transaction Id " + Receipt.TransId + ")  Using " + Receipt.PayType + "  " + DateTime.Now.ToShortDateString(), Receipt.CustomerEmail, "", "authnet@memorybook.com", body, Utils.EmailType.Mbc);


            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                    .MarkAsCritical()
                    .SetMessage("Failed to send receipt email.")
                    .Submit();
            }



        }


        [HttpPost]
        [Route("getReceipt")]
        public async Task<IHttpActionResult> GetReceipt(string orderid)
        {

            var processingResult = new ServiceProcessingResult<ReceiptBindingModel> { IsSuccessful = true };
            //Get the order first thing to make sure we have it.
            ReceiptBindingModel Receipt = new ReceiptBindingModel();
            try
            {
                var sqlQuery = "SELECT Orders.Id,OrderId,PayType,Grade,BookType,Teacher,PersText1,Studentfname,Studentlname,Emailaddress,Schcode,ItemAmount,Itemqty,Schinvoicenumber,Orddate,ItemTotal,Schname,Yr,l1.caption as Caption1,l2.caption As Caption2,l3.caption As Caption3,l4.caption As Caption4 FROM Orders Left Join lookup l1 On l1.ivalue=Orders.icon1 Left Join lookup l2 on l2.ivalue=Orders.icon2 Left Join lookup l3 on l3.ivalue=Orders.icon3  Left Join lookup l4 on l4.ivalue=Orders.icon4  where orderid=@OrderId";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@OrderId", orderid) };
                var sqlQueryService = new SQLQuery();
                var orderResult = await sqlQueryService.ExecuteReaderAsync<OrderBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!orderResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("There was an error retrieving your order receipt.", "There was an error retrieving your order receipt.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                    return Ok(processingResult);
                }
                Receipt.Items = (List<OrderBindingModel>)orderResult.Data;

                MySqlParameter[] payParameters = new MySqlParameter[] { new MySqlParameter("@OrderId", orderid) };
                var sqlQueryService1 = new SQLQuery();
                sqlQuery = "Select Schcode,PayerFname,PayerLname,Poamt,PayType,TransId, AuthCode,CustEmail,Ddate,OrderId,Schname from Payment where orderid=@OrderId";
                var payResult = await sqlQueryService.ExecuteReaderAsync<PaymentBindingModel>(CommandType.Text, sqlQuery, payParameters);
                if (!payResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("There was an error retrieving your order receipt.", "There was an error retrieving your order receipt.", true, false);
                    ExceptionlessClient.Default.SubmitLog(typeof(TempOrderController).FullName, orderResult.Error.UserMessage, "Error");
                    return Ok(processingResult);

                }
                List<PaymentBindingModel> payments = (List<PaymentBindingModel>)payResult.Data;
                PaymentBindingModel payment = payments[0];//should only be one payment
                Receipt.Schname = payment.Schname;
                Receipt.Schcode = payment.Schcode;
                Receipt.PayerFname = payment.PayerFname;
                Receipt.PayerLname = payment.PayerLname;
                Receipt.Payment = payment.Poamt;
                Receipt.PayType = payment.PayType;
                Receipt.TransId = payment.TransId;
                Receipt.AuthCode = payment.AuthCode;
                Receipt.OrderId = payment.OrderId;
                Receipt.CustomerEmail = payment.CustEmail;
                Receipt.OrderDate = payment.Ddate;
                Receipt.Payment = payment.Poamt;
                processingResult.Data = Receipt;


            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("There was an error retrieving your order receipt.", "There was an error retrieving your order receipt.", true, false);

                return Ok(processingResult);

            }

            return Ok(processingResult);
            //----------------------------------------------------------------------


        }


        [HttpPost]
        [Route("getSchoolReceipt")]
        public async Task<IHttpActionResult> GetSchoolReceipt(string transid)
        {
            var processingResult = new ServiceProcessingResult<PaymentBindingModel> { IsSuccessful = true };
            //Get the order first thing to make sure we have it.

            try
            {
                var sqlQuery = "SELECT Schname,Schcode,PayerFname,PayerLname,Poamt,PayType,TransId,AuthCode,CustEmail,Ddate,OrderId from Payment  where transid=@Transid";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@Transid", transid) };
                var sqlQueryService = new SQLQuery();
                var payResult =await sqlQueryService.ExecuteReaderAsync<PaymentBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!payResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError(payResult.Error.UserMessage, payResult.Error.UserMessage, true, false);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, payResult.Error.UserMessage, "Error");
                    return Ok(processingResult);
                }

                List<PaymentBindingModel> payments = (List<PaymentBindingModel>)payResult.Data;
                PaymentBindingModel payment = payments[0];//should only be one payment
                processingResult.Data = payment;
                return Ok(processingResult);

            }catch(Exception ex)
            {
                ex.ToExceptionless()
                       .SetMessage("Error retrieving payment for school receipt.");
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ex.Message,ex.Message, true, false);
                return Ok(processingResult);
            }

    }
    }
}