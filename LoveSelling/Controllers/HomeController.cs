using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoveSelling.Service;
using LoveSelling.Models;


namespace LoveSelling.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 活動首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

    }
}