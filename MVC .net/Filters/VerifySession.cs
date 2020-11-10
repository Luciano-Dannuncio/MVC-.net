using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore;
using MVCnetcore.Models;
using MVCnetcore.Controllers;

namespace MVCnetcore.Filters
{
    public class VerifySession : ActionFilterAttribute 
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);

                if (filterContext.HttpContext.Session.GetInt32("userId").HasValue == false)
                {
                    if (filterContext.Controller is LogInController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("./LogIn");
                    }
                }
                else
                {
                    
                }

            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("./LogIn"); 
            }
        }
    }
}
