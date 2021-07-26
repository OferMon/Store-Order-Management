using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.DTO
{
    public class updateOrder
    {
        public int orderId;
        public string customerName;
        public string customerEmail;
        public string storeName;
        public DateTime requiredDate;
        public int orderStatus;
        public decimal totalPrice;
        public byte[] ordersRowVersion;
        public byte[] customersRowVersion;
    }
}