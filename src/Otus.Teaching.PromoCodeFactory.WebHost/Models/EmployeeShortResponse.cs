﻿using AutoMapper;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using System;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Models
{
    [AutoMap(typeof(Employee), ReverseMap = true)]
    public class EmployeeShortResponse
    {
        public Guid Id { get; set; }
        
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}