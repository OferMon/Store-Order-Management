using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.DTO
{
    public class shippedOrderItemDTO
    {
        public string productName;
        public int quantity;
        public decimal listPrice;
        public decimal discount;
        public decimal totalPrice;
    }
}