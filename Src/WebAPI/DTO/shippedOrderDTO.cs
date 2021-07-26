using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.DTO
{
    public class shippedOrderDTO
    {
        public int orderId;
        public string customerName;
        public string customerPhone;
        public string storeName;
        public DateTime orderDate;
        public int productsNumber;
        public decimal totalPrice;
        public int supplyDelay;
    }
}