using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCapp.Models
{
    public class Category
    {
        
        
        public int CategoryID { get; set; }

        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}