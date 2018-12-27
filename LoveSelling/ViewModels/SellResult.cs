using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoveSelling.Models;

namespace LoveSelling.ViewModels
{
    public class SellResult
    {
        public Product Product { get; set; }

        public decimal TotalAmount { get; set; }
    }
}