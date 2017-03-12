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
        private OnlineShopDataContext db = new OnlineShopDataContext();

        // GET: ShoppingCarts
        public ActionResult List()
        {            
            int total = 0;
            var q = from sc in db.ShoppingCart
                    join p in db.Products
                    on sc.ProductID equals p.ProductID
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
            return View(db.ShoppingCart.ToList());
        }

        public ActionResult AddToCart(int id)
        {
            Product product = db.Products.Find(id);
      
            var cart = db.ShoppingCart;
            var target = cart.Where(c => c.Product.Name == product.Name && c.Count < product.Quantity);
            var overflow = cart.Where(c => c.Count >= product.Quantity);
            if (ModelState.IsValid)
            {
                if(!target.Any())
                {
                    var cartItem = new ShoppingCart()
                    {
                        Product = product,
                        Count = 1
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
}
