using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCapp.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartID { get; set; }
        public int Count { get; set; }
        public int ProductID { get; set; }
        public string UserID { get; set; }
        public System.DateTime DateCreated { get; set; }

        public virtual Product Product { get; set; }
    }
}