using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoveSelling.Service;
using LoveSelling.Models;

namespace LoveSelling.Controllers
{
    /// <summary>
    /// 登錄相關頁面
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// 帳號資訊首頁(DeBug用)
        /// </summary>
        /// <returns>回傳帳號登陸頁面</returns>
        public ActionResult Index()
        {
            ViewBag.LogonNumber = HttpContext.Application["online"];
            ViewBag.Message = Session["message"];
            ViewBag.Name = Session["name"];
            ViewBag.EmployeeID = Session["employeeID"];
            ViewBag.Rank = Session["Rank"];

            return View();
        }

        /// <summary>
        /// 登陸頁面
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>登錄輸入頁面</returns>
        public ActionResult Logon(string returnUrl = null)
        {

            ViewBag.LogonNumber = HttpContext.Application["online"];
            ViewBag.LogonMessage = Session["message"];
            ViewBag.ReturnUrl = returnUrl;

            if (Session["Rank"] != null)
                return RedirectToAction("Index", "Account");
            else
                return View();  //可以改成PartialView

        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns>登出後轉導回登錄頁</returns>
        public ActionResult Logout()
        {
            AccountHelper.Logout(Session["employeeID"].ToString());
            Session.RemoveAll();
            return RedirectToAction("Logon", "Account");
        }

        /// <summary>
        /// 進行登錄
        /// </summary>
        /// <param name="account"></param>
        /// <param name="returnUr">回去的網址</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logon(Account account, string returnUrl = null)
        {
            ViewBag.LogonNumber = HttpContext.Application["online"];
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View();

            //進行Logon
            account.ID = account.ID?.ToUpper(); //都轉成大寫
            var LogonInfo = AccountHelper.Logon(account);
            // 登入時清空所有 Session 資料
            Session.RemoveAll();

            switch (LogonInfo.SignInStatus)
            {
                case SignInStatus.Success:
                    Session["message"] = "登錄成功";
                    Session["name"] = LogonInfo.Name;
                    Session["employeeID"] = LogonInfo.EmployeeID;
                    Session["Rank"] = LogonInfo.Rank;
                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        var url = returnUrl.Split('/');
                        if (url.Length > 2)
                            return RedirectToAction(url?[2], url?[1]);
                        else
                            return RedirectToAction("Index", url?[1]);
                    }
                case SignInStatus.RequiresVerification:
                    Session["message"] = "登錄失敗:密碼錯誤";
                    break;
                case SignInStatus.Failure:
                    Session["message"] = "登錄失敗:無此帳號";
                    break;
                default:
                    Session["message"] = "登錄失敗:不明";
                    break;
            }
            return RedirectToAction("Logon", "Account", new { ReturnUrl = returnUrl });
        }
    }

}
