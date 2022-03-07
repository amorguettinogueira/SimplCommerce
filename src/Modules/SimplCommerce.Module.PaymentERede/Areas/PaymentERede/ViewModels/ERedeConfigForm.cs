using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels
{
    public class ERedeConfigForm
    {
        [Required]
        public bool Sandbox { get; set; } = false;

        [Required]
        public string RedePV { get; set; }

        [Required]
        public int QtdeParcelas { get; set; }

        [Required]
        public double ValorMinimoParcelamento { get; set; }

        [Required]
        public string RedeToken { get; set; }

        [Required]
        public string SoftDescriptor { get; set; }
    }
}
