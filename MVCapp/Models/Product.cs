using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCapp.Models
{

    public class Product
    {
        public int ProductID { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int CategoryId { get; set; }

        public int Quantity { get; set; }

        public string WeinRegion { get; set; }

        public bool IsDiscount { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<ShoppingCart> ShoppingCart { get; set; }
    }
}