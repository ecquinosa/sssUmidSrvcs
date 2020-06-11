using Business_Layer.services;
using sssUmidSrvcs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace sssUmidSrvcs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            ComputerInfo computerInfo = new ComputerInfo();
            Cryptor cryptor = new Cryptor();
            string CryptedText = cryptor.Encrypt(ComputerInfo.ProcIDMachName);
            computerInfo.WriteKeys(CryptedText);


            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
