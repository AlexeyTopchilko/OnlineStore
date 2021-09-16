﻿using System;

namespace OrderMicroservice.Service.Services.Models
{
    public class UserOrdersViewModel
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public decimal TotalPrice { get; set; }

        public string State { get; set; }
    }
}