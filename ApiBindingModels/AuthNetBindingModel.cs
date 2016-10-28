using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBindingModels
{
    public class AuthNetBindingModel
    {
        //public string Type{ get; set; }//request.TransType AUTH_CAPTURE, AUTH_ONLY, PRIOR_AUTH_CAPTURE, CREDIT, VOID ect.
        public string Method { get; set; } //cc,echeck
        public string Cardnum{ get; set; }
        public string ExpirationDate { get; set; }     
        public string TransactionType { get; set; }///*' request.TransType AUTH_CAPTURE,AUTH_ONLY,PRIOR_AUTH_CAPTURE,CREDIT,VOID ect.*/
        public string CardCode { get; set; }//security code
        public string EcheckType { get; set; } = "WEB";
        public string BankAccName { get; set; }//customer fullname
        public string BankName { get; set; }//name of bank
        public string BankAccType { get; set; }//savings,checking,bussiness
        public string BankAbaCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string Amount { get; set; }//amount to charge
        public string Description { get; set; }//misc data
        public string CustId { get; set; }
        public string Schname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string InvoiceNumber { get; set; }//orderid
        public string EmailAddress  { get; set; }
     

    }
    public class AuthNetResponse
    {
        public string CardType { get; set; }
        public string Custid { get; set; }
        public string CardNum { get; set; }
        public string TransActionType { get; set; }
        public bool Approved { get; set; }
        public string Amount { get; set; }
        public string AuthCode { get; set; }
        public string TransId { get; set; }
        public string Message { get; set; }
        public string Method { get; set; }
        public string Email { get; set; }
        
        }

    }

