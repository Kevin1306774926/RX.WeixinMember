using Newtonsoft.Json;
using RX.WXMember.Comm;
using RX.WXMember.Models;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace RX.WXMember.Controllers
{
    public class PayController : Controller
    {
        private MyDbContext db = new MyDbContext();
        // GET: Pay
        public ActionResult Index(string id)
        {
            // 调用地址 http://w.roccode.cn/pay/RX20180100110073
            if (!string.IsNullOrEmpty(id))
            {
                Session["readerId"] = id;
            }
            string code = Request["code"];                      
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
                Session["AccessToken"] = result;
                //OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                //Session["UserInfo"] = userInfo;
                return View();
            }
        }

        [HttpPost]
        public async  Task<ActionResult> Index(FormCollection collection)
        {
            ModelForOrder order = null;
            int totalfee = 0;
            object objResult = "";
            string strTotal_fee = Request.Form["totalfee"];
            if (int.TryParse(strTotal_fee, out totalfee))
            {
                OAuthAccessTokenResult tokenResult = Session["AccessToken"] as OAuthAccessTokenResult;
                string body = "瑞雪管理系统充值";
                string timeStamp = TenPayV3Util.GetTimestamp();
                string nonceStr = TenPayV3Util.GetNoncestr();
                string openid = tokenResult.openid;
                string tenPayV3Notify = "http://w.roccode.cn/pay/ResultNotify";
                string key = "8f75e82b6f1b7d82f7952121a6801b4a";
                string billNo = string.Format("{0}{1}{2}", WeixinData.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), TenPayV3Util.BuildRandomStr(6));
                var xmlDataInfo = new TenPayV3UnifiedorderRequestData(WeixinData.AppId, WeixinData.MchId, body, billNo, totalfee, Request.UserHostAddress,
                    tenPayV3Notify, Senparc.Weixin.MP.TenPayV3Type.JSAPI, openid, key, nonceStr);

                UnifiedorderResult result = TenPayV3.Unifiedorder(xmlDataInfo);        //调用统一订单接口

                if (result.result_code == "SUCCESS")
                {
                    order = new ModelForOrder();
                    order.appId = result.appid;
                    order.nonceStr = result.nonce_str;
                    order.packageValue = "prepay_id=" + result.prepay_id;
                    order.paySign = TenPayV3.GetJsPaySign(result.appid, timeStamp, result.nonce_str, order.packageValue, key);
                    order.timeStamp = timeStamp;
                    order.msg = "预支付订单生成成功";

                    // 保存预支付订单信息  
                    string id = Session["readerId"] as string;
                    OAuthUserInfo userInfo = Session["UserInfo"] as OAuthUserInfo;
                    if (!string.IsNullOrEmpty(id))
                    {
                        string groundCode = id.Substring(0, 6);
                        string gameCode = id.Substring(6, 2);
                        string readerCode = id.Substring(8, 3);
                        //string sn = id.Substring(11, 5);
                        db.Orders.Add(new Order()
                        {
                            GroundCode = groundCode,
                            GameCode = gameCode,
                            ReaderCode = readerCode,
                            Amt = totalfee,
                            BillNo=billNo,
                            //WeiXinCode = userInfo.nickname,
                            //Openid = userInfo.openid,
                            //Unionid = userInfo.unionid
                        });
                        db.SaveChanges();                        
                    }

                }
            }
            else
            {
                order = new ModelForOrder();
                order.msg = "输入充值数量异常";
            }
            if (order == null)
            {
                order = new ModelForOrder();
                order.msg = "预支付订单生成失败，请重试！";
            }
            objResult = order;
            return Json(objResult);            
        }

        private async Task DooPost(string url,string json)
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            //创建HttpClient（注意传入HttpClientHandler）
            using (var http = new HttpClient(handler))
            {                
                //var json = JsonConvert.SerializeObject(record);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await http.PostAsync(url, content);

                //await异步等待回应
                //var response = await http.PostAsync(url, content);
                ////确保HTTP成功状态值
                //response.EnsureSuccessStatusCode();
                ////await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<ActionResult> ResultNotify()
        {
            //获取微信返回的数据
            string resultFromWx = getPostStr();
            //转换成xml文档       
            var ress = XDocument.Parse(resultFromWx);
            //返回给微信接口 提示微信处理成功
            string data = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
            //通信成功
            if (ress.Element("xml").Element("return_code").Value == "SUCCESS")
            {
                if (ress.Element("xml").Element("result_code").Value == "SUCCESS")
                {
                    //商户订单号<><!
                    string outNos = ress.Element("xml").Element("out_trade_no").Value;
                    //微信订单号
                    string tranId = ress.Element("xml").Element("transaction_id").Value;

                    if (!string.IsNullOrEmpty(outNos))
                    {
                        //判断统一订单是否支付成功
                        var order = db.Orders.Where(t => t.BillNo == outNos).FirstOrDefault();
                        if (order != null)
                        {
                            if (order.State != 1)      // 状态0：未支付，状态1：已支付
                            {
                                order.State = 1;
                                order.WxBillNo = tranId;
                                db.SaveChanges();

                                //调用场地接口,向扫描的二维码卡头充值
                                PayReader item = new PayReader();
                                item.GroundId = order.GroundCode;
                                item.ReaderCode = order.ReaderCode;
                                item.Amt = order.Amt/100;                                
                                string json = JsonConvert.SerializeObject(item);
                                string url = @"http://m.roccode.cn/api/payReader";

                                await DooPost(url, json);
                            }
                        }
                    }
                }
            }
            return Content(data,"text/xml");
        }

        public string getPostStr()
        {
            Int32 intLen = Convert.ToInt32(Request.InputStream.Length);
            byte[] b = new byte[intLen];
            Request.InputStream.Read(b, 0, intLen);
            return System.Text.Encoding.UTF8.GetString(b);
        }
    }
}