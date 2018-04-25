using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RX.WXMember.Comm;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using RX.WXMember.Services;
using RX.WXMember.Models;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin;

namespace RX.WXMember.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Index()
        {
            string host = Request.Url.Host;
            ViewBag.UrlUserInfo = OAuthApi.GetAuthorizeUrl(Comm.WeixinData.AppId, string.Format(@"http://{0}/login/callback",host),WeixinData.Token, OAuthScope.snsapi_userinfo);
            ViewBag.UrlUserBase = OAuthApi.GetAuthorizeUrl(Comm.WeixinData.AppId, string.Format(@"http://{0}/login/callback", host),WeixinData.Token, OAuthScope.snsapi_base);
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            string userCode = Request["userCode"];
            string pwd = Request["pwd"];
            CustomerService cs = new CustomerService();
            Customer u= cs.Login(userCode, pwd);
            if(u!=null)
            {
                Session["user"] = u;
                if(u.IsManager)
                {
                    return RedirectToAction("Index", "admin");
                }
                else
                {
                    ViewBag.msg = "需要管理员权限才能登陆";
                }
            }
            ViewBag.msg = "用户名或密码错误";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Callback(string code, string state)
        {
            if (string.IsNullOrEmpty(code) || state != WeixinData.Token)
            {
                return Content("验证授权失败!");
            }
            //通过，用code换取access_token
            OAuthAccessTokenResult result = null;
            try
            {
                result = OAuthApi.GetAccessToken(Comm.WeixinData.AppId, Comm.WeixinData.AppSecret, code);
                if (result.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + result.errmsg);
                }
                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                CustomerService cs = new CustomerService();
                var customer = cs.AddCustomer(userInfo);
                if (customer.IsManager)
                {
                    Session["user"] = customer;
                    return RedirectToAction("index", "admin");
                }
                ViewBag.msg = "登录失败";

                return View("index");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}