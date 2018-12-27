using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoveSelling.Models
{
    public class LoveAuthorizeFilter : FilterAttribute, IAuthorizationFilter
    {
        public int Rank { get; set; }

        //建構子 Default
        public LoveAuthorizeFilter()
        {
            Rank = 0;
        }

        //建構子
        public LoveAuthorizeFilter(int rank)
        {
            Rank = rank;
        }

        /// <summary>
        /// 授權
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.Session["Rank"] == null)
            {
                filterContext.HttpContext.Session["message"] = "請先登錄，登錄後會返回前頁";
                filterContext.Result = new RedirectResult($@"/Account/Logon/?returnUrl={filterContext.HttpContext?.Request?.RawUrl}");
            }
            else
            {
                if (Convert.ToInt32(filterContext.HttpContext.Session["Rank"]) < this.Rank)
                {
                    filterContext.HttpContext.Session["message"] = "權限不足，請洽協同平台發展科";
                    filterContext.Result = new RedirectResult($@"/Account/Index");
                }
            }

        }
    }
}