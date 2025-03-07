﻿using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.Models
{
    public class DetailedPayment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int PaymentId { get; set; }
        [Required]
        public double? Price { get; set; }
        public int Status { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual Order? Order { get; set; }
    }
}
