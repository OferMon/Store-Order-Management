using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI.DTO;
using System.Data.Entity;

namespace WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PartAController : ApiController
    {
        public List<shippedOrderDTO> GetOrdersList()
        {
            BikeStoresDBContext db = new BikeStoresDBContext();
            List<order> ordersFromDB = db.orders.Where(x => x.order_status == 4).Include(x => x.customer).Include(x => x.store).Include(x => x.order_items).ToList();
            List<shippedOrderDTO> orders = new List<shippedOrderDTO>();

            foreach (order tmp in ordersFromDB)
            {
                shippedOrderDTO o = new shippedOrderDTO();
                o.orderId = tmp.order_id;
                o.customerName = tmp.customer.last_name + " " + tmp.customer.first_name;
                o.customerPhone = tmp.customer.phone == null ? "" : tmp.customer.phone;
                o.storeName = tmp.store.store_name;
                o.orderDate = tmp.order_date;
                o.productsNumber = tmp.order_items.Sum(x => x.quantity);  //לבדוק
                o.totalPrice = tmp.order_items.Sum(x => x.list_price * x.quantity * (1 - x.discount));   //לבדוק

                if (tmp.shipped_date < tmp.required_date)
                    o.supplyDelay = 0;
                else
                    o.supplyDelay = ((DateTime)tmp.shipped_date - tmp.required_date).Days;

                orders.Add(o);
            }

            return orders;
        }


        [Route("api/PartA/{orderId}")]
        public List<shippedOrderItemDTO> GetOrderItemsList(int orderId)
        {
            BikeStoresDBContext db = new BikeStoresDBContext();
            List<order_items> orderItemsFromDB = db.order_items.Where(x => x.order_id == orderId).Include(x => x.product).ToList();
            List<shippedOrderItemDTO> orderItems = new List<shippedOrderItemDTO>();

            foreach (order_items tmp in orderItemsFromDB)
            {
                shippedOrderItemDTO item = new shippedOrderItemDTO();
                item.productName = tmp.product.product_name;
                item.quantity = tmp.quantity;
                item.listPrice = tmp.list_price;
                item.discount = tmp.discount;
                item.totalPrice = tmp.list_price * tmp.quantity * (1 - tmp.discount);

                orderItems.Add(item);
            }

            return orderItems;
        }

    }
}

