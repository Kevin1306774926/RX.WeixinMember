using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RX.WXMember.Comm
{
    public class WeixinData
    {

        //public const string TOKEN = WeixinData.Token;
        //public const string AppId = "wx453d372e2d375660";
        //public const string Secret = "a68b1b0ebcf0f86afe627a55bb094877";
        //public const string EncodingAESkey = "xlONlFQwZfhLsshtmWEWnfyd2acs2hZ9QBIQVXJrtsu";
        /// <summary>
        /// 加密签名
        /// </summary>
        public const string SIGNATURE = "signature";
        /// <summary>
        /// 时间戳
        /// </summary>
        public const string TIMESTAMP = "timestamp";
        /// <summary>
        /// 随机数
        /// </summary>
        public const string NONCE = "nonce";
        /// <summary>
        /// 随机字符串
        /// </summary>
        public const string ECHOSTR = "echostr";

        /// <summary>
        /// 发送人
        /// </summary>
        public const string FROM_USERNAME = "FromUserName";
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public const string TO_USERNAME = "ToUserName";
        /// <summary>
        /// 消息内容
        /// </summary>
        public const string CONTENT = "Content";
        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public const string CREATE_TIME = "CreateTime";
        /// <summary>
        /// 消息类型
        /// </summary>
        public const string MSG_TYPE = "MsgType";
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public const string MSG_ID = "MsgId";        

        public static string CallbackUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["CallbackUrl"].ToString();
        public static string Token= System.Web.Configuration.WebConfigurationManager.AppSettings["Token"].ToString();
        public static string AppId= System.Web.Configuration.WebConfigurationManager.AppSettings["AppId"].ToString();
        public static string AppSecret= System.Web.Configuration.WebConfigurationManager.AppSettings["AppSecret"].ToString();
        public static string EncodingAESkey= System.Web.Configuration.WebConfigurationManager.AppSettings["EncodingAESkey"].ToString();
        public static string MchId = System.Web.Configuration.WebConfigurationManager.AppSettings["MchId"].ToString();
        /// <summary>
        /// 得到当前时间（整型）（考虑时区）
        /// </summary>
        /// <returns></returns>
        public static string GetNowTime()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;
            return a.ToString();
        }

    }
}