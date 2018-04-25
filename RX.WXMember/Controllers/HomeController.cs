using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RX.WXMember.Comm;
using System.Xml;
using RX.WXMember.Services;
using RX.WXMember.Models;
using System.Text;

namespace RX.WXMember.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            string EncodingAESKey = "xlONlFQwZfhLsshtmWEWnfyd2acs2hZ9QBIQVXJrtsu";
            string token = WeixinData.Token;
            string signature = Request["signature"];
            string timestamp = Request["timestamp"];
            string nonce = Request["nonce"];
            string echostr = Request["echostr"];

            string echo = string.Empty;
            echo = echostr;
            //echo = Comm.WeixinHelper.CheckServer(token, signature, timestamp, nonce, echostr);            
            Response.Write(echo);
        }
        [HttpPost]
        public void Index(FormCollection collection)
        {
            string host = Request.Url.Host;
            System.IO.Stream stream = Request.InputStream;
            XmlDocument dom = new XmlDocument();
            dom.Load(stream);
            XmlElement root = dom.DocumentElement;
            string toUserName = root.SelectSingleNode("ToUserName").InnerText;
            string fromUserName = root.SelectSingleNode("FromUserName").InnerText;
            string msgType = root.SelectSingleNode("MsgType").InnerText;

            CustomerService cs = new CustomerService();
            //Customer customer = cs.Login(fromUserName, null);
            //if (customer == null)
            //{
            //    return;
            //}

            string retMsg = string.Empty;
            switch (msgType.ToLower())
            {
                case "text":
                    string content = root.SelectSingleNode("Content").InnerText;
                    string retContent = "无法识别的指令";
                    try
                    {
                        string[] arrayContent = content.Split(new char[] { ',', '，', '.' });
                        if (arrayContent.Length > 1)
                        {
                            RegRecord record = new RegRecord();
                            record.Record = content;
                            record.GroundCode = arrayContent[1];
                            record.CreateTime = DateTime.Now;
                            //record.CreateUser = customer.NickName;

                            switch (arrayContent[0])
                            {
                                case "1":       //网络版一代 打码方式
                                    retContent = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(arrayContent[1], "MD5");
                                    string s;
                                    int i;
                                    if (int.TryParse(arrayContent[2], out i))
                                    {
                                        i += 48;
                                        char c = (char)i;
                                        s = c.ToString();
                                    }
                                    else
                                    {
                                        s = arrayContent[2].Substring(0, 1);
                                    }
                                    retContent = retContent.Insert(15, s.ToUpper());
                                    break;
                                case "2":       //网络版二代 打码方式
                                    string DecryptStr = Comm.EncryptUtils.SimpleDecrypt(arrayContent[1]);
                                    string[] list = DecryptStr.Split(new char[] { '-' });
                                    string groundId = list[0];
                                    record.GroundCode = groundId;

                                    string date = list[2];
                                    long count = int.Parse(date) + int.Parse(arrayContent[2]) + int.Parse(arrayContent[3]);
                                    count = count % 9;
                                    string data = string.Format("{0}-{1}-{2}-{3}{4}", groundId, date, arrayContent[2], arrayContent[3], count);
                                    retContent = Comm.EncryptUtils.SimpleEncrypt(data);
                                    retContent = string.Format("{0}\r\n{1}", DecryptStr, retContent);
                                    break;
                                case "3":       //GH 单机版打码方式
                                    #region GH 单机版打码方式

                                    StringBuilder regcode = new StringBuilder();
                                    string Code = arrayContent[1];
                                    if (Code.Length == 10)
                                    {
                                        string groundCode = arrayContent[1].Substring(0, 6);
                                        record.GroundCode = groundCode;
                                    }
                                    byte[] tmpBytes = System.Text.Encoding.ASCII.GetBytes(Code);

                                    int crc = (int)Comm.CRC8.CRC(tmpBytes);
                                    if (crc > 99)
                                    {
                                        regcode.Append(crc.ToString().Substring(1, 2));
                                    }
                                    else
                                    {
                                        regcode.Append(crc.ToString().PadLeft(2, '0'));
                                    }

                                    string type = arrayContent[2];

                                    Random ran = new Random();
                                    switch (type)
                                    {
                                        case "1":       //设置时间(4、5、6、7 位置为四字节的时间)
                                            break;
                                        case "2":       //打码运行时间 (4、5 为运行天数)
                                            if (arrayContent.Length >= 3)
                                            {
                                                int day;
                                                if (int.TryParse(arrayContent[3], out day))
                                                {
                                                    regcode.Append("2");
                                                    regcode.Append(day.ToString().PadLeft(2, '0'));
                                                    regcode.Append(ran.Next(10, 99));
                                                }
                                            }
                                            break;
                                        case "3":     //永久码
                                            regcode.Append("3");
                                            regcode.Append(ran.Next(1000, 9999));
                                            break;
                                        case "4":    //读卡选择 1==CUP卡，0==M1卡
                                            break;
                                        case "5":    //pos机恢复出厂设置
                                            regcode.Append("5");
                                            regcode.Append(ran.Next(1000, 9999));
                                            break;
                                        case "6":    //pos场地密码恢复设置
                                            regcode.Append("6");
                                            regcode.Append(ran.Next(1000, 9999));
                                            break;
                                        case "7":    //pos收银密码恢复设置
                                            regcode.Append("7");
                                            regcode.Append(ran.Next(1000, 9999));
                                            break;
                                        case "8":    //pos管理密码恢复设置
                                            regcode.Append("8");
                                            regcode.Append(ran.Next(1000, 9999));
                                            break;
                                    }
                                    //计算最后两位
                                    if (regcode.Length == 7)
                                    {
                                        List<byte> arr = new List<byte>();
                                        string strReg = regcode.ToString();
                                        if (strReg.StartsWith("0"))                 //起始为如果为0 的话，就改成1
                                        {
                                            strReg = "1" + strReg.Substring(1);
                                        }

                                        for (int j = 0; j < regcode.Length; j++)
                                        {
                                            arr.Add(Convert.ToByte(int.Parse(strReg[j].ToString())));
                                        }

                                        byte[] tmp = arr.ToArray();


                                        int c = (int)Comm.CRC8.CRC(tmp);
                                        if (c > 99)
                                        {
                                            regcode.Append(c.ToString().Substring(1, 2));
                                        }
                                        else
                                        {
                                            regcode.Append(c.ToString().PadLeft(2, '0'));
                                        }


                                        retContent = regcode.ToString();
                                        if (retContent.StartsWith("0"))             //将注册码的首末字符如果为0，就改成1
                                        {
                                            retContent = "1" + retContent.Substring(1);
                                        }
                                        if (retContent.EndsWith("0"))
                                        {
                                            retContent = retContent.Substring(0, retContent.Length - 1) + "1";
                                        }
                                    }
                                    break;

                                #endregion
                                case "4":       //GH 网络版二代 打码方式
                                    string dayNum = string.Empty;
                                    string amtNum = string.Empty;
                                    DecryptStr = Comm.EncryptUtils.SimpleDecrypt(arrayContent[1]);
                                    if (arrayContent.Length > 3)       //指令,机器码,天数，总金额
                                    {
                                        dayNum = arrayContent[2];
                                        amtNum = arrayContent[3];
                                    }
                                    else
                                    {
                                        dayNum = arrayContent[2];
                                        amtNum = "20000";
                                    }
                                    list = DecryptStr.Split(new char[] { '-' });
                                    groundId = list[0];
                                    record.GroundCode = groundId;
                                    date = list[1];
                                    count = int.Parse(date) + int.Parse(dayNum) + int.Parse(amtNum);
                                    count = count % 9;
                                    data = string.Format("{0}-{1}-{2}{3}-{4}", groundId, date, dayNum, count, amtNum);
                                    retContent = Comm.EncryptUtils.SimpleEncrypt(data);
                                    break;
                                default:
                                    break;
                            }
                            record.RegCode = retContent;
                            //CompactEFContext db = new CompactEFContext();
                            //db.RegRecords.Add(record);
                            //db.SaveChanges();
                            RegRecordsService service = new RegRecordsService();
                            service.Add(record);
                        }
                        else
                        {
                            switch (content.ToLower())
                            {
                                case "test":
                                    retContent = string.Format("From:{0} To:{1} msg:{2}", fromUserName, toUserName, content);
                                    break;
                                case "login":
                                    retContent = string.Format(@"http://{0}/login", host);
                                    break;
                                case "pay":
                                    retContent = string.Format(@"http://{0}/pay", host);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retContent = ex.Message;
                    }
                    retMsg = Comm.WeixinHelper.TextMessage(toUserName, fromUserName, retContent);
                    break;
                case "location":
                    break;
                case "music":
                    break;
                case "news":
                    break;
                case "event":
                    retContent = "异常查询";
                    string eventType = root.SelectSingleNode("Event").InnerText;
                    switch (eventType.ToLower())
                    {
                        case "scancode_push":
                        case "scancode_waitmsg":
                            string ScanResult = root.SelectSingleNode("ScanCodeInfo").SelectSingleNode("ScanResult").InnerText;
                            if (!string.IsNullOrEmpty(ScanResult))
                            {
                                ScanResult = ScanResult.TrimEnd(new char[] { '/' });
                                string barcode = ScanResult.Substring(ScanResult.LastIndexOf("/") + 1);
                                if (barcode.Length == 12)
                                {
                                    retContent = string.Format("条码:{0}\r\n名称:{1}\r\n规格:{2}\r\n客服电话:{3}\r\n访问官网:{4}", barcode, "艾艾贴", "盒", "4008-591-227", "www.aiaitie.com");
                                }
                                else
                                {
                                    retContent = ScanResult;
                                }
                            }
                            break;
                    }
                    retMsg = Comm.WeixinHelper.TextMessage(toUserName, fromUserName, retContent);
                    break;
                default:
                    break;
            }

            Response.Write(retMsg);
            //return View();
        }


        public void SN(string code)
        {
            string ret = string.Empty;
            ret = RegService.GetRegCode(code);
            this.Response.Write(ret);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}