using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoveSelling.Service;
using LoveSelling.Models;
using LoveSelling.ViewModels;
using System.Data.SqlClient;
using System.Threading;

namespace LoveSelling.Controllers
{
    /// <summary>
    /// 購買頁面的controller
    /// </summary>
    public class BuyProductController : Controller
    {
       
        /// <summary>
        /// 銷售系統首頁
        /// </summary>
        /// <returns>回傳頁面</returns>
        [LoveAuthorizeFilter((int)AccountRank.User)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 展示賣出總金額
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowTotal(Place? place = null)
        {
            ViewBag.totalAmount = ProductHelper.GetTotalAmount(place);

            switch (place)
            {
                case Place.C004:
                    ViewBag.place = "總部大樓";
                    break;
                case Place.C105:
                    ViewBag.place = "登峰園區";
                    break;
                case Place.C810:
                    ViewBag.place = "希望園區";
                    break;
                default:
                    ViewBag.place = "本屆義賣活動";
                    break;
            }

            return View();
        }

        /// <summary>
        /// 交易進行
        /// </summary>
        /// <param name="id"></param>
        /// <returns>回傳頁面</returns>
        [LoveAuthorizeFilter((int)AccountRank.User)]
        public PartialViewResult Transaction(string id = null)
        {
            if (id == null || id?.Length != 6)
                return PartialView("_TransactionResult", new SellResult() { TotalAmount = ProductHelper.GetTotalAmount() });

            var product = ProductHelper.Load(id).FirstOrDefault();
            if (product != null && !product.isSell.GetValueOrDefault()) //??
                ProductHelper.Transaction(product, Session["employeeID"]?.ToString());

            var sellResult = new SellResult() { Product = product, TotalAmount = ProductHelper.GetTotalAmount() };
            return PartialView("_TransactionResult", sellResult);
        }

        /// <summary>
        /// 後台管理
        /// </summary>
        /// <returns></returns>
        [LoveAuthorizeFilter((int)AccountRank.Maintainer)]
        public ActionResult Maintain()
        {
            //搜尋特別處理
            var keyword = Session["keyword"]?.ToString();
            ViewBag.keyword = keyword;
            var products = ProductHelper.Find(keyword);

            return View(products);
        }

        /// <summary>
        /// 查詢產品
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <returns>回傳查詢商品的列表</returns>
        [LoveAuthorizeFilter((int)AccountRank.Maintainer)]
        public PartialViewResult FindProducts(string keyword = null)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;

            var products = ProductHelper.Find(keyword);
            Session["keyword"] = keyword;
            return PartialView("_FindProducts", products);
        }

        /// <summary>
        /// 查詢單筆資料資訊
        /// </summary>
        /// <param name="id"></param>
        /// <returns>回傳修改頁面</returns>
        [HttpPost]
        [LoveAuthorizeFilter((int)AccountRank.Maintainer)]
        public ActionResult LoadProduct(string id = null)
        {
            ViewBag.Action = (id == null) ? "AddProduct" : "UpdateProduct";
            ViewBag.UnitItems = GetUnitItems();

            if (id == null) return PartialView("_SaveProduct");

            var product = ProductHelper.Load(id).FirstOrDefault();
            return PartialView("_SaveProduct", product);
        }

        [HttpPost]
        [LoveAuthorizeFilter((int)AccountRank.Maintainer)]
        public ActionResult UpdateProduct(Product product)
        {
            ViewBag.Action = (product.isSell == null) ? "AddProduct" : "UpdateProduct";
            ViewBag.UnitItems = GetUnitItems();
            Thread.Sleep(500); //Delay 0.5秒 避免Ajax的指令未完成

            if (!ModelState.IsValid)
                return PartialView("_SaveProduct", product);

            var employeeId = Session["employeeID"]?.ToString();
            try
            {
                if (product.isSell == null)
                    ProductHelper.AddProduct(product, employeeId);
                else
                    ProductHelper.UpdateProduct(product, employeeId);

            }
            catch (SqlException e)
            {
                if (e.Number.ToString() == "2627") ViewBag.ProductMessage = "商品編號重複";
                else ViewBag.ProductMessage = "Exception: " + e.Number.ToString();
                return PartialView("_SaveProduct", product);
            }

            return null;
        }

        [LoveAuthorizeFilter((int)AccountRank.Maintainer)]
        public ActionResult DeleteProduct(string id)
        {
            if (id != null) ProductHelper.DeleteProduct(id, Session["employeeID"]?.ToString());
            return RedirectToAction("Maintain");
        }

        /// <summary>
        /// 取得單位DropDownList的資料
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetUnitItems()
        {
            var unitItems = HttpContext.Application["UnitItems"] as Dictionary<string, string>;
            return unitItems.Select(n => new SelectListItem()
            {
                Text = $@"{n.Key} {n.Value}",
                Value = n.Key
            }).ToList();

        }

    }
}