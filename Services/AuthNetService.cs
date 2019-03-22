using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.Configuration;
using ApiBindingModels;
using System.Web;
using System.Net;
using System.IO;
using Exceptionless;
using Exceptionless.Models;
namespace Services
{
    public class AuthNetService
    {
        public async Task<ServiceProcessingResult<AuthNetResponse>> SubmittAsync(AuthNetBindingModel model)


        {
            var result = new ServiceProcessingResult<AuthNetResponse> { IsSuccessful = true };
            //  ' By default, this sample code is designed to post to our test server for
            //  ' developer accounts: https://test.authorize.net/gateway/transact.dll
            //  ' for real accounts (even in test mode), please make sure that you are
            //  'posting to: https://secure.authorize.net/gateway/transact.dll
            //' post_url = "https://secure.authorize.net/gateway/transact.dll"
            string post_url = ConfigurationManager.AppSettings["AuthUrl"].ToString();
            Dictionary<string, string> post_values = new Dictionary<string, string>();
            string login = ConfigurationManager.AppSettings["mbc" + "ApiLogin"].ToString();//need variable to tell what site to use. should come in post
            string Key= ConfigurationManager.AppSettings["mbc" + "TransactionKey"].ToString();
            string test= ConfigurationManager.AppSettings["GatewayTest"].ToString();


            //post_values.Add("x_test_request", ConfigurationManager.AppSettings("GatewayTest")) /*use this for submissions to live site only-----------------------------------------------------------------------------------------------------------------*/
            post_values.Add("x_test_request", test);
            post_values.Add("x_version", "3.1");
            post_values.Add("x_login", login);
            post_values.Add("x_tran_key", Key);
            post_values.Add("x_delim_data", "TRUE");
            post_values.Add("x_delim_char", "|");
            post_values.Add("x_relay_response_array", "FALSE");
            post_values.Add("x_type", "AUTH_CAPTURE"); /*' request.TransType AUTH_CAPTURE,AUTH_ONLY,PRIOR_AUTH_CAPTURE,CREDIT,VOID ect.*/
            post_values.Add("x_method", model.Method);/* 'CC,ECHECK*/
            post_values.Add("x_echeck_type", "WEB");/* 'web*/
            post_values.Add("x_card_num", model.Cardnum );
            post_values.Add("x_exp_date", model.ExpirationDate);
            post_values.Add("x_card_code", model.CardCode);
            post_values.Add("x_recurring_billing", "FALSE"); /*' we don't use this so is always false*/
            post_values.Add("x_bank_acct_name", model.BankAccName);//customer name
            post_values.Add("x_bank_name", model.BankName);
            post_values.Add("x_bank_acct_type", model.BankAccType); /*'savings,checking,businesschecking*/
            post_values.Add("x_bank_aba_code", model.BankAbaCode);
            post_values.Add("x_bank_acct_num", model.BankAccountNumber);
            post_values.Add("x_amount", model.Amount);
            post_values.Add("x_description", "");
            post_values.Add("x_cust_id", model.CustId);
            post_values.Add("x_first_name", model.FirstName);
            post_values.Add("x_last_name", model.LastName);
            post_values.Add("x_address", model.Address);
            post_values.Add("x_state", model.State);
            post_values.Add("x_ city", model.City);
            post_values.Add("x_zip", model.Zip);
            post_values.Add("x_invoice_num", model.InvoiceNumber);
            post_values.Add("x_email", model.EmailAddress);
            post_values.Add("x_duplicate_window", "420");/* '7 minutes*/

            //    ' Additional fields can be added here as outlined in the AIM integration
            //' guide at: http://developer.authorize.net

            //' This section takes the input fields and converts them to the proper format
            //' for an http post.  For example: "x_login=username&x_tran_key=a1B2c3D4"
            string post_string = "";
            foreach (KeyValuePair<string, string> field in post_values)
            {
                post_string += field.Key + "=" + HttpUtility.UrlEncode(field.Value) + "&";
            }
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            post_string = post_string.Substring(0, post_string.Length - 1);
            //' create an HttpWebRequest object to communicate with Authorize.net
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
            objRequest.Method = "POST";
            objRequest.ContentLength = post_string.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            //' post data is sent as a stream
            StreamWriter myWriter =new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(post_string);
            myWriter.Close();
            // returned values are returned as a stream, then read into a string

            
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                StreamReader responseStream = new StreamReader(objResponse.GetResponseStream());
                string post_response = responseStream.ReadToEnd();
                responseStream.Close();
            

            // the response_array string is broken into an array


            String[] response_array = post_response.Split('|');
         
           
                AuthNetResponse Returnresponse_array = new AuthNetResponse();
                try
                {
                    //Returnresponse_array.Approved = Returnresponse_array.GetText(response_array(1)) '1,2,3,4 approved,declined,error,held for review
                    bool approvedret = false;
                    switch (response_array[0])
                    {
                        case "1":
                            approvedret = true;
                            break;
                        default:
                            approvedret = false;
                            break;
                    }
                    Returnresponse_array.Approved = approvedret;
                    Returnresponse_array.Message = response_array[3];
                    Returnresponse_array.AuthCode = response_array[4];
                    Returnresponse_array.TransId = response_array[6];
                    string r = response_array[7];
                    string rr = response_array[5];
                    Returnresponse_array.Amount = response_array[9];
                    Returnresponse_array.Method = response_array[10];
                    Returnresponse_array.TransActionType = response_array[11];
                    Returnresponse_array.Custid = response_array[12];
                    Returnresponse_array.Email = response_array[23];
                    Returnresponse_array.CardNum = response_array[50];
                    Returnresponse_array.CardType = response_array[51];
                    //will fail if submission fails and there are not enough elements
                }
                catch (Exception ex)
                {
                ex.ToExceptionless().Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError(ex.Message, ex.Message, true, false);
                return result;
            }

            result.IsSuccessful = true;
            result.Data = Returnresponse_array;
            return result;
        }
    }
}
