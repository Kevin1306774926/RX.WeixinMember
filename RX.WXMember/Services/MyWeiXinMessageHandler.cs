using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.Context;
using RX.WXMember.Models;
using System.IO;

namespace RX.WXMember.Services
{
    public class MyWeiXinMessageHandler : MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        public MyWeiXinMessageHandler(Stream inputStream):base(inputStream)
        {

        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎光临";
            return responseMessage;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            string content = requestMessage.Content;
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

                    switch (arrayContent[0])
                    {
                        case "1":
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
                        case "2":
                            string DecryptStr = Comm.EncryptUtils.SimpleDecrypt(arrayContent[1]);
                            string[] list = DecryptStr.Split(new char[] { '-' });
                            string groundId = list[0];
                            string date = list[2];
                            long count = int.Parse(date) + int.Parse(arrayContent[2]) + int.Parse(arrayContent[3]);
                            count = count % 9;
                            string data = string.Format("{0}-{1}-{2}-{3}{4}", groundId, date, arrayContent[2], arrayContent[3], count);
                            retContent = Comm.EncryptUtils.SimpleEncrypt(data);
                            retContent = string.Format("{0}\r\n{1}", DecryptStr, retContent);
                            break;
                        default:
                            break;
                    }
                    record.RegCode = retContent;
                    RegRecordsService service = new RegRecordsService();
                    service.Add(record);
                }
            }
            catch (Exception ex)
            {
                retContent = ex.Message;
            }            
            var responseMessage=this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content=retContent;
            return responseMessage;
        }

    }
}