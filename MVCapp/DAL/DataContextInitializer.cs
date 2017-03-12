using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MVCapp.Models;

namespace MVCapp.DAL
{
    public class DataContextInitializer : DropCreateDatabaseIfModelChanges<OnlineShopDataContext>
    {
        protected override void Seed(OnlineShopDataContext context)
        {
            var categories = new List<Category>
            {
                new Category() {Name="fehér" },
                new Category() {Name="vörös" },
                new Category() {Name="rosé" }
            };

            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var products = new List<Product> {
                new Product() {Name="Thummerer Bikavér" , Price = 1000 , CategoryId=2, Quantity=10, WeinRegion="Egri", IsDiscount=false}
            };
            products.ForEach(p => context.Products.Add(p));

            var regions = new List<RegionCoordinates>
            {
                new RegionCoordinates() {RegionName= "Balatonboglári", Latitude=46.773067 , Longitude=17.659941 },
                new RegionCoordinates() {RegionName= "Kunsági", Latitude=46.630721 , Longitude=19.402703 },
                new RegionCoordinates() {RegionName= "Egri", Latitude=47.903159 , Longitude=20.376645 },
                new RegionCoordinates() {RegionName= "Ászár-neszmélyi", Latitude=47.728365 , Longitude=18.369583 },
                new RegionCoordinates() {RegionName= "Etyek-budai", Latitude=47.438996 , Longitude=18.739382 },
                new RegionCoordinates() {RegionName= "Villány-siklósi", Latitude=45.869859 , Longitude=18.452277 },
                new RegionCoordinates() {RegionName= "Tokaj-hegyaljai", Latitude=48.114954 , Longitude=21.406371 },
                new RegionCoordinates() {RegionName= "Badacsonyi", Latitude=46.789696 , Longitude=17.495202 },
                new RegionCoordinates() {RegionName= "Pécsi", Latitude=46.072948 , Longitude=18.229637 },
                new RegionCoordinates() {RegionName= "Szekszárdi", Latitude=46.347041 , Longitude=18.705483 }
            };
            regions.ForEach(r => context.RegionCoordinates.Add(r));
            context.SaveChanges();
        }

    }
}