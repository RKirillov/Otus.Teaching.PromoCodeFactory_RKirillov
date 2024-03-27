using AutoMapper;
using Otus.Teaching.PromoCodeFactory.Core.Domain;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Otus.Teaching.PromoCodeFactory.WebHost.ModelDTO
{
    [AutoMap(typeof(EmployeeDto), ReverseMap = true)]
    public class EmployeeSaveDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        //public List<string> Roles = new List<string>();

        public int AppliedPromocodesCount { get; set; }
    }
}
