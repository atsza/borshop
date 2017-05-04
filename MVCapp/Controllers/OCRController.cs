﻿using MVCapp.DAL;
using MVCapp.Models;
using Patagames.Ocr;
using Patagames.Ocr.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MVCapp.Controllers
{
    public class OCRController : Controller
    {
        private OnlineShopDataContext db = new OnlineShopDataContext();
        // GET: OCR
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            List<string> productNames = new List<string>();
            double max = 0, max2 = 0;
            string name1 = "", name2 = "";

            var products = from p in db.Products
                           select p.Name;
            foreach (var p in products)
            {
                productNames.Add(p.ToString());
            }
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Content/Images"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "Sikeres feltöltés";               

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "Nem lett fájl kiválasztva";
            }

            if (file != null && file.ContentLength > 0)
            {
                using (var api = OcrApi.Create())
                {
                    api.Init(Languages.English, "C:/Users/Ati/Documents/Visual Studio 2015/Projects/MVCapp/MVCapp");
                    using (var bmp = Bitmap.FromFile(Path.Combine(Server.MapPath("~/Content/Images"),
                                               Path.GetFileName(file.FileName))) as Bitmap)
                    {
                        string plainText = api.GetTextFromImage(bmp);
                        plainText = Regex.Replace(plainText, @"\W", "");
                        foreach (var name in productNames)
                        {
                            double temp = Levenshtein.CalculateSimilarity(plainText, name);
                            if (temp > max)
                            {
                                max2 = max;
                                name2 = name1;
                                max = temp;
                                name1 = name;
                            }
                            else if (temp > max2)
                            {
                                max2 = temp;
                                name2 = name;
                            }
                        }

                    }
                }
            }
            var results = from p in db.Products
                          where p.Name == name1 || p.Name == name2
                          select p;
            return View(results.ToList());
        }
    }

    static class Levenshtein
    {

        public static int LevenshteinDistance(string source, string target)
        {
            // degenerate cases
            if (source == target) return 0;
            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[target.Length + 1];
            int[] v1 = new int[target.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < source.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < target.Length; j++)
                {
                    var cost = (source[i] == target[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[target.Length];
        }

        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        public static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = LevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }
    }
}