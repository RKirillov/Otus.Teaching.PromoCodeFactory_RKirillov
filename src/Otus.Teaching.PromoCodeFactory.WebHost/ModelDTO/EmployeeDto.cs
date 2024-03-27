using AutoMapper;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using Otus.Teaching.PromoCodeFactory.Core.Domain;
using Otus.Teaching.PromoCodeFactory.WebHost.ModelDTO;
using System;
using System.Collections.Generic;

namespace Otus.Teaching.PromoCodeFactory.Core.Domain.Administration
{
    [AutoMap(typeof(Employee), ReverseMap = true)]
    public class EmployeeDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public List<Role> Roles { get; set; } = new List<Role>();

        public int AppliedPromocodesCount { get; set; }
    }
}