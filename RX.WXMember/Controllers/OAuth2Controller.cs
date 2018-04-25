using RX.WXMember.Comm;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RX.WXMember.Controllers
{
    public class OAuth2Controller : Controller
    {
        // GET: OAuth2
        public ActionResult Index()
        {
            //var code = Session["code"] as string;
            var code = Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                string host = Request.Url.Host;
                string path = Request.Path;
                string state = "ROC" + DateTime.Now.Millisecond;
                string redirect_uri = OAuthApi.GetAuthorizeUrl(Comm.WeixinData.AppId, string.Format(@"http://{0}{1}", host, path), state, OAuthScope.snsapi_base);
                return Redirect(redirect_uri);
            }
            else
            {
                //通过，用code换取access_token
                OAuthAccessTokenResult result = null;
                try
                {
                    result = OAuthApi.GetAccessToken(WeixinData.AppId, WeixinData.AppSecret, code);
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
                if (result.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + result.errmsg);
                }
                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                return View(userInfo);
            }            
        }
    }
}