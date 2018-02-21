using Shield.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Shield.Examples.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public Boolean Active { get; set; }
        public DateTime AddedOn { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }
        public string ContactName { get; set; }
        public Boolean HasDiscount { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public double UnitPrice { get; set; }

    }
}

namespace Shield.Examples.Controllers
{
    public class _ShielduiController : System.Web.Mvc.Controller
    {
        public System.Web.Mvc.ActionResult Index()
        {            
            return View();
        }

        //public System.Web.Mvc.ActionResult _List()
        //{
        //    List<Product> products = new List<Product>
        //            {
        //                new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1, Active=false, AddedOn = new DateTime(2012,3,3) },
        //                new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M,  Active=true, AddedOn = new DateTime(2012,5,3) },
        //                new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M, Active=false, AddedOn = new DateTime(2012,10,3) }
        //            };

        //    return Json(products, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        //}

        public System.Web.Mvc.ActionResult _List()
        {           
            List<Order> result  = new List<Order>
                    {
                        new Order { ID = 1, ContactName = "Alfreds Futterkiste", HasDiscount=true, Quantity=21, OrderDate=new DateTime(2013, 3,2), UnitPrice= 45.60},
                        new Order { ID = 2, ContactName = "Antonio Moreno Taquería", HasDiscount=true, Quantity=22, OrderDate=new DateTime(2013, 2,3), UnitPrice= 41.60},
                        new Order { ID = 3, ContactName = "Around the Horn", HasDiscount=false, Quantity=23, OrderDate=new DateTime(2013, 1,5), UnitPrice= 44.30},
                        new Order { ID = 4, ContactName = "Berglunds snabbköp", HasDiscount=true, Quantity=21, OrderDate=new DateTime(2013, 5,8), UnitPrice= 25.12},
                        new Order { ID = 5, ContactName = "Blauer See Delikatessen", HasDiscount=false, Quantity=26, OrderDate=new DateTime(2013, 6,3), UnitPrice= 15.34},
                        new Order { ID = 6, ContactName = "Blondesddsl père et fils", HasDiscount=true, Quantity=27, OrderDate=new DateTime(2013, 3,4), UnitPrice= 55.78},
                        new Order { ID = 7, ContactName = "Bólido Comidas preparadas", HasDiscount=false, Quantity=29, OrderDate=new DateTime(2013, 3,5), UnitPrice= 65.67},
                        new Order { ID = 8, ContactName = "Bon app'", HasDiscount=true, Quantity=21, OrderDate=new DateTime(2013, 7,1), UnitPrice= 45.10},
                        new Order { ID = 9, ContactName = "Bottom-Dollar Markets", HasDiscount=false, Quantity=31, OrderDate=new DateTime(2013, 1,4), UnitPrice= 43.61},
                        new Order { ID = 10, ContactName = "Ana Trujillo Emparedados", HasDiscount=false, Quantity=41, OrderDate=new DateTime(2013, 2,6), UnitPrice= 35.33},
                    };
            return Json(result, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ActionName("OrderUpdate")]
        public bool OrderUpdate(List<Order> orders)
        {
            if (orders == null)
            {
                throw new ArgumentNullException("orders");
            }

            foreach (Order item in orders)
            {
                //Order o = Orders.Where(pr => pr.ID == item.ID).First();
                //o.ContactName = item.ContactName;
                //o.HasDiscount = item.HasDiscount;
                //o.OrderDate = item.OrderDate;
                //o.Quantity = item.Quantity;
                //o.UnitPrice = item.UnitPrice;
            }

            return true;
        }
    }

    public class ProductsController : ApiController
    {
        public List<Product> Products
        {
            get
            {
                return getSession()["_pcProducts"] as List<Product>;
            }
        }

        private System.Web.SessionState.HttpSessionState getSession()
        {
            var session = System.Web.HttpContext.Current.Session;

            if (session == null)
            {
                var res = new HttpResponseMessage(HttpStatusCode.BadGateway);
                throw new HttpResponseException(res);
            }

            if (session["_pcProducts"] == null)
            {
                List<Product> products = new List<Product>
                    {
                        new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1, Active=false, AddedOn = new DateTime(2012,3,3) },
                        new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M,  Active=true, AddedOn = new DateTime(2012,5,3) },
                        new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M, Active=false, AddedOn = new DateTime(2012,10,3) }
                    };

                session["_pcProducts"] = products;
            }

            return session;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return Products;
        }

        [ActionName("productCreate")]
        public Product PostProduct(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            item.Id = Products.Max(i => i.Id) + 1;
            Products.Add(item);
            return item;
        }

        [ActionName("productUpdate")]
        public bool Update(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Product p = Products.Where(pr => pr.Id == item.Id).First();
            p.Active = item.Active;
            p.AddedOn = item.AddedOn;
            p.Category = item.Category;
            p.Name = item.Name;
            p.Price = item.Price;

            return true;
        }

        [ActionName("productRemove")]
        public void RemoveProduct(Product item)
        {
            Products.RemoveAll(p => p.Id == item.Id);
        }
    }
}