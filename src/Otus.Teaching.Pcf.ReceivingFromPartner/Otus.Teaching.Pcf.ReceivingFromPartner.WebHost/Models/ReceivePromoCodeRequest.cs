using System;

namespace Otus.Teaching.Pcf.ReceivingFromPartner.WebHost.Models
{
    public class ReceivingPromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public string PromoCode { get; set; }

        public string Preference { get; set; }
        
        public Guid? PartnerManagerId { get; set; }
    }
}