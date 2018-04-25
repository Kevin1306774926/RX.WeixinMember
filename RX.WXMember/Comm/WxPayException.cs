using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RX.WXMember.Comm
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg)
            : base(msg)
        {

        }
    }
}