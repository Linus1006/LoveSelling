using LoveSelling.Models;
using LoveSelling.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LoveSelling
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application開始做的事情
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //GlobalFilters.Filters.Add(new LoveAuthorizeFilter()); //全站權限要求 
            Application["online"] = 0;
            Application["UnitItems"] = ProductHelper.GetUnits();    //預先載入單位

        }

        /// <summary>
        /// 每個Session開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(Object sender, EventArgs e)
        {
            Session.Timeout = 60 * 8;
            //全域值須進行Lock控管
            Application.Lock();
            Application["online"] = (int)Application["online"] + 1;
            Application.UnLock();
        }

        /// <summary>
        /// 每個Session結束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(Object sender, EventArgs e)
        {
            Application.Lock();
            Application["online"] = (int)Application["online"] - 1;
            Application.UnLock();
        }

    }
}
