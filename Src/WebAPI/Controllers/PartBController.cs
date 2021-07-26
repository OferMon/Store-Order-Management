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
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PartBController : ApiController
    {
        public List<updateOrder> GetOrdersList()
        {
            BikeStoresDBContext db = new BikeStoresDBContext();
            List<order> ordersFromDB = db.orders.Where(x => x.order_status==1 || x.order_status == 2).Include(x => x.customer).Include(x => x.store).Include(x => x.order_items).ToList();
            List<updateOrder> orders = new List<updateOrder>();

            foreach (order tmp in ordersFromDB)
            {
                updateOrder o = new updateOrder();
                o.orderId = tmp.order_id;
                o.customerName = tmp.customer.last_name + " " + tmp.customer.first_name;
                o.customerEmail = tmp.customer.email;
                o.storeName = tmp.store.store_name;
                o.requiredDate = tmp.required_date;
                o.orderStatus = tmp.order_status;
                o.totalPrice = tmp.order_items.Sum(x => x.list_price * x.quantity * (1 - x.discount)); //לבדוק
                o.ordersRowVersion = tmp.RowVersion;
                o.customersRowVersion = tmp.customer.RowVersion;
                orders.Add(o);
            }

            return orders;
        }

        [Route("api/PartB/{orderId}")]
        public HttpResponseMessage PutUpdateOrder(int orderId, [FromBody]unshippedOrderDTO data)
        {
            BikeStoresDBContext db = new BikeStoresDBContext();

            order o = db.orders.Where(x => x.order_id == orderId).Include(x => x.customer).Include(x => x.store).Include(x => x.order_items).FirstOrDefault();

            if (data.orderStatus == 4 && o.order_status!=4)
            {
                foreach(order_items i in o.order_items)
                {
                    stock s = o.store.stocks.Single(x => x.product_id == i.product_id && x.store_id == o.store_id);
                    if (s.quantity < i.quantity || s.quantity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Gone, "לא ניתן לספק את ההזמנה עקב חוסר במלאי");
                    }
                    s.quantity -= i.quantity;
                } 
            }

            o.order_status = data.orderStatus;
            o.customer.email = data.customerEmail;
            o.required_date = data.requiredDate;
            o.shipped_date = DateTime.Now.Date;

            db.Entry(o).OriginalValues["RowVersion"] = data.ordersRowVersion;
            db.Entry(o.customer).OriginalValues["RowVersion"] = data.customersRowVersion;

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "ערכים לא תקינים");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var ctx = ((IObjectContextAdapter)db).ObjectContext;
                foreach (var et in ex.Entries)
                {
                    //store win
                    ctx.Refresh(System.Data.Entity.Core.Objects.RefreshMode.StoreWins, et.Entity);
                }
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "שגיאה - הרשומה נעולה על ידי משתמש אחר");
            }

            RowVersionDTO rowVersion = new RowVersionDTO();
            rowVersion.ordersRowVersion = o.RowVersion;
            rowVersion.customersRowVersion = o.customer.RowVersion;

            return Request.CreateResponse(HttpStatusCode.OK, rowVersion);
        }
    }
}
