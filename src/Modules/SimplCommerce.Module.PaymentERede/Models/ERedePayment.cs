using System;
using System.ComponentModel.DataAnnotations;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.PaymentsERede.Models
{
    public class ERedePayment : EntityBase
    {
        public ERedePayment()
        {
            CreatedOn = DateTimeOffset.Now;
            LatestUpdatedOn = DateTimeOffset.Now;
        }

        public long OrderId { get; set; }

        public Order Order { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LatestUpdatedOn { get; set; }

        public decimal Amount { get; set; }

        public decimal PaymentFee { get; set; }

        [StringLength(450)]
        public string PaymentMethod { get; set; }

        [StringLength(450)]
        public string GatewayTransactionId { get; set; }

        public int Status { get; set; }

        public string FailureMessage { get; set; }
    }
}
