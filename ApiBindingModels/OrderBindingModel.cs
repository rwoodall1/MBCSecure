using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBindingModels
{
    public class OrderBindingModel
    {
        public UInt32 Id { get; set; }
        public UInt32 OrderId { get; set; }
        public string PayType { get; set; }
        public string Grade { get; set; }
        public string BookType { get; set; }
        public string Teacher { get; set; }
        public string PersText1 { get; set; }
        public string Studentfname { get; set; }
        public string Studentlname { get; set; }
        public string Emailaddress { get; set; }
        public string Schcode { get; set; }
        public decimal ItemAmount { get; set; }
        public UInt32 Itemqty { get; set; }
        public UInt32 Schinvoicenumber { get; set; }
        public DateTime Orddate { get; set; }
        public decimal ItemTotal { get; set; }
        public string Schname { get; set; }
        public string Yr { get; set; }
        public DateTime Adcuto { get; set; }
        public UInt32 Icon1 { get; set; }
        public UInt32 Icon2 { get; set; }
        public UInt32 Icon3 { get; set; }
        public UInt32 Icon4 { get; set; }
        public string Josicon1 { get; set; }
        public string Josicon2 { get; set; }
        public string Josicon3 { get; set; }
        public string Josicon4 { get; set; }
        public string Caption1 { get; set; }
        public string Caption2 { get; set; }
        public string Caption3 { get; set; }
        public string Caption4 { get; set; }
        public decimal SalesTax { get; set; }

    }
    public class OrderBillingBindingModel
    {
        public string Studentfname { get; set; }
        public string PayType { get; set;}
        public string Studentlname { get; set; }
        public string Emailaddress { get; set; }
        public string Schcode { get; set; }
        public UInt32 Schinvoicenumber { get; set; }
        public string BankAccName { get; set; }
        public string Schname { get; set; }      
        public string CardNumber { get; set; }
        public string CardCode { get; set; }
        public string ExpirationDate { get; set; }
        public decimal SalesTax { get; set; }

        public decimal Total { get; set; }
    }
    public class ReceiptBindingModel
    {
        public string Schname { get; set; }
        public string Schcode { get; set; }
        public UInt32 OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerEmail{get;set;}
        
        public decimal Payment { get; set; }
        public string PayType { get; set; }
        public string TransId { get; set; }
        public string AuthCode { get; set; }
     public decimal TaxPaid { get; set; }
        public string PayerFname { get; set; }
        public string PayerLname { get; set; }
       
     
        public List<OrderBindingModel> Items { get; set; }
    }
    public class PaymentBindingModel
    {
        public string Schname { get; set; }
        public string Schcode { get; set; }
        public string PayerFname { get; set; }
        public string PayerLname { get; set; }
        public decimal Poamt { get; set; }
        public string PayType { get; set; }
        public string TransId { get; set; }
        public string AuthCode { get; set; }
        public string CustEmail { get; set; }
        public DateTime Ddate { get; set; }
        public UInt32 OrderId { get; set; }

    }
}
