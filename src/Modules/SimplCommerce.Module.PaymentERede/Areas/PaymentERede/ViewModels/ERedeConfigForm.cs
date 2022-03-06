using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels
{
    public class ERedeConfigForm
    {
        [Required]
        public string PublicKey { get; set; }

        [Required]
        public string PrivateKey { get; set; }

        [Required]
        public string MerchantId { get; set; }

        [Required]
        public bool IsProduction { get; set; } = false;

        [JsonIgnore]
        public string Environment { get { return IsProduction ? "production" : "sandbox"; } }
    }
}
