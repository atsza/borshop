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

        public virtual Product Product { get; set; }
    }
}