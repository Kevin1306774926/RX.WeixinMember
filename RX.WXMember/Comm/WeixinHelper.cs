using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RX.WXMember.Comm
{
    public class WeixinHelper
    {
        public static string CheckServer(string token, string signature, string timestamp, string nonce, string echostr)
        {
            string[] tmp = { token, timestamp, nonce };
            Array.Sort(tmp);
            string str = string.Join("", tmp);
            string code = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");
            if (code.ToLower().Equals(signature))
            {
                return echostr;
            }
            return "";
        }

        public static int GetTime()
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            return (int)(DateTime.Now - dt).Ticks;
        }

        public static string TextMessage(string toUserName, string fromUserName, string content)
        {
            string message = string.Format(@"<xml>
                                                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                                                    <CreateTime>{2}</CreateTime>
                                                                    <MsgType><![CDATA[text]]></MsgType>
                                                                    <Content><![CDATA[{3}]]></Content>
                                                                    </xml>", fromUserName, toUserName, GetTime(), content);
            return message;
        }
    }
}