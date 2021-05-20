﻿using System;

namespace TradeMap.Service.DTO.Response
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? Role { get; set; }
        public bool? Active { get; set; }
        public string FcmToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? BrandId { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }
    }
}