using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RX.WXMember.Services
{
    public class RegService
    {

        public static string  GetRegCode(string code)
        {
            string ret = string.Empty;
            string[] arrayContent = code.Split(new char[] { ',', '，', '.' });
            string DecryptStr = Comm.EncryptUtils.SimpleDecrypt(arrayContent[1]);
            string[] list = DecryptStr.Split(new char[] { '-' });
            string groundId = list[0];            

            string date = list[2];
            long count = int.Parse(date) + int.Parse(arrayContent[2]) + int.Parse(arrayContent[3]);
            count = count % 9;
            string data = string.Format("{0}-{1}-{2}-{3}{4}", groundId, date, arrayContent[2], arrayContent[3], count);
            ret = Comm.EncryptUtils.SimpleEncrypt(data);
            ret = string.Format("{0}\r\n{1}", DecryptStr, ret);
            return ret;
        }
    }
}