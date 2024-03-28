using AutoMapper;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using System;

namespace Otus.Teaching.PromoCodeFactory.WebHost.ModelDTO
{
    [AutoMap(typeof(Employee), ReverseMap = true)]
    public class EmployeeShortDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}