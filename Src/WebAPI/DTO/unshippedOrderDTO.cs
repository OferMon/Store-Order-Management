using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.DTO
{
    public class unshippedOrderDTO
    {
        public int orderStatus;
        public string customerEmail;
        public DateTime requiredDate;
        public byte[] ordersRowVersion;
        public byte[] customersRowVersion;
    }
}