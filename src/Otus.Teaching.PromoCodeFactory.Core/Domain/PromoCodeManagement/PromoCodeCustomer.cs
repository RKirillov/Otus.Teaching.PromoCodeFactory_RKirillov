﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCodeCustomer : BaseEntity
    {
        public Guid PromoCodeId { get; set; }
        public PromoCode PromoCode { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
