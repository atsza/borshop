using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCapp.DAL;
using MVCapp.Models;

namespace MVCapp.Controllers
{
    public class ShoppingCartsController : Controller
    {
        public string ActualUserId { get; set; }

        private CurrentSession current = new CurrentSession();

        private OnlineShopDataContext db = new OnlineShopDataContext();

        // GET: ShoppingCarts
        public ActionResult List()
        {
            ActualUserId = current.GetUserId();
            int total = 0;
            var q = from sc in db.ShoppingCart
                    join p in db.Products
                    on sc.ProductID equals p.ProductID
                    where sc.UserID == ActualUserId
                    select new
                    {
                        p.Name,
                        sc.Count,
                        p.Price
                    };
            foreach (var item in q)
            {
                int a = item.Count;
                int b = item.Price;
                total += a * b;
            }
            ViewBag.Total = total;
            return View(db.ShoppingCart.Where(c => c.UserID == ActualUserId).ToList());
        }

        public ActionResult AddToCart(int id)
        {
            Product product = db.Products.Find(id);

            ActualUserId = current.GetUserId();
            var cart = db.ShoppingCart;
            var target = cart.Where(c => c.UserID == ActualUserId && c.Product.Name == product.Name && c.Count < product.Quantity);
            var overflow = cart.Where(c => c.Count >= product.Quantity);
            if (ModelState.IsValid)
            {
                if(!target.Any())
                {
                    var cartItem = new ShoppingCart()
                    {
                        Product = product,
                        Count = 1,
                        UserID = ActualUserId,
                        DateCreated = DateTime.Now
                    };
                    db.ShoppingCart.Add(cartItem);
                    TempData["Message"] = "Hozzaadva";
                }
                else
                {
                    if (overflow.Any())
                    {
                        TempData["Message"] = "Nincs tobb raktaron";
                    }
                    else
                    {
                        foreach (var item in target)
                        {
                            item.Count += 1;
                        }
                        TempData["Message"] = "Hozzaadva";
                    }                   
                }
                
                db.SaveChanges();
                
            }
            //int sum = cart.Select(t => t.Count).Sum();
            //ViewBag.AmountinCart = sum;

            return RedirectToAction("Index","Products");
        }




        public ActionResult Remove(int id)
        {
            ShoppingCart shoppingCart = db.ShoppingCart.Find(id);
            db.ShoppingCart.Remove(shoppingCart);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    class CurrentSession
    {
        public const string CartSessionKey = "UserID";

        public string GetUserId()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.     
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }
    }


}
