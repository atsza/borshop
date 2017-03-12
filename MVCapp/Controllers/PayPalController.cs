using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using PayPal.Api;
using MVCapp.Models;
using MVCapp.DAL;
using Newtonsoft.Json;

namespace MVCapp.Controllers
{
    public class PayPalController : Controller
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "use_vendor_currency_conversion")]
        public string use_vendor_currency_conversion { get; set; }
        private OnlineShopDataContext db = new OnlineShopDataContext();
       
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult PaymentWithCreditCard()
        {
            return View("CreditCardView");
        }

        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = Configuration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {             
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/PaymentWithPayPal?";

                    var guid = Convert.ToString((new Random()).Next(100000));

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }
            var cart = from c in db.ShoppingCart
                       select c;
            var productsincart = db.ShoppingCart.Include(s => s.Product);
            foreach (var item in productsincart)
            {
                item.Product.Quantity -= item.Count; 
            }
            foreach (var c in cart)
            {
                db.ShoppingCart.Remove(c);               
            }
            
            db.SaveChanges();
            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {

            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            int total = 0;
            var cartitems = from sc in db.ShoppingCart
                            join p in db.Products
                            on sc.ProductID equals p.ProductID
                            select new
                            {
                                p.Name,
                                sc.Count,
                                p.Price
                            };
            foreach (var item in cartitems)
            {
                itemList.items.Add(new Item()
                {
                    name = item.Name,
                    currency = "HUF",
                    price = item.Price.ToString(),
                    quantity = item.Count.ToString(),
                    sku = "sku"
                });
                int a = item.Count;
                int b = item.Price;
                total += a * b;
            }

            var payer = new Payer() { payment_method = "paypal"};

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = total.ToString()
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = "HUF",
                total = total.ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = "your invoice number",
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }
    }
}
