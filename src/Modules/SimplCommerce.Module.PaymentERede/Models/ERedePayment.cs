using System.ComponentModel.DataAnnotations;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.PaymentsERede.Models
{
    public class ERedePayment : EntityBase
    {
        public ERedePayment()
        {
        }

        [StringLength(60)]
        public string reference { get; set; }

        [StringLength(40)]
        public string tid { get; set; }

        [StringLength(20)]
        public string nsu { get; set; }

        [StringLength(20)]
        public string authorizationCode { get; set; }

        [StringLength(20)]
        public string expiresAt { get; set; }

        [StringLength(10)]
        public string date { get; set; }

        [StringLength(10)]
        public string time { get; set; }

        [StringLength(10)]
        public string returnCode { get; set; }

        [StringLength(250)]
        public string returnMessage { get; set; }

        [StringLength(10)]
        public string avsReturnCode { get; set; }

        [StringLength(250)]
        public string avsReturnMessage { get; set; }

        [StringLength(10)]
        public string threeDSecureReturnCode { get; set; }

        [StringLength(250)]
        public string threeDSecureReturnMessage { get; set; }

        [StringLength(10)]
        public string brandReturnCode { get; set; }

        [StringLength(250)]
        public string brandReturnMessage { get; set; }

        [StringLength(40)]
        public string brandName { get; set; }
    }
}
