using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBindingModels
{
    public static class DomainModelsConverter {
        public static AuthNetBindingModel ToAuthNetBindingModel(this OrderBillingBindingModel payment)
        {
            return new AuthNetBindingModel
            {
                Method = payment.PayType,
                Cardnum = payment.CardNumber,
                ExpirationDate=payment.ExpirationDate,
                CardCode=payment.CardCode,
                BankAccName=payment.BankAccName,

                

            };


        }

    }
}
