using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RX.WXMember.Models;

namespace RX.WXMember.Services
{
    public class CustomerService
    {
        MyDbContext db = new MyDbContext();
        /// <summary>
        /// 添加会员，先查询该会员是否存在，不存在就添加， 存在就返回该会员信息。
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Customer AddCustomer(OAuthUserInfo userInfo)
        {
            var customer = db.Customers.SingleOrDefault(t => t.Openid == userInfo.openid);
            if(customer==null)
            {
                customer = new Customer();
                customer.Code = userInfo.nickname;
                customer.NickName = userInfo.nickname;
                customer.Sex = userInfo.sex;
                customer.Country = userInfo.country;
                customer.Province = userInfo.province;
                customer.City = userInfo.city;
                customer.Openid = userInfo.openid;
                customer.HeadImgUrl = userInfo.headimgurl.Substring(0,userInfo.headimgurl.Length-1);
                customer.Unionid = userInfo.unionid;
                customer.CreateTime = DateTime.Now;
                customer.LastTime = DateTime.Now;
                customer.IsStop = false;
                customer.IsManager = false;
                customer.Password = "888888";
                customer.Score = 0;
                db.Customers.Add(customer);
                db.SaveChanges();
            }
            else
            {
                customer.LastTime = DateTime.Now;
                db.SaveChanges();
            }
            return customer;
        }

        public Customer Login(string code,string password)
        {
            var c = db.Customers.SingleOrDefault(t => t.Openid == code);
            if (c != null)
            {
                return c;
            }
            c = db.Customers.SingleOrDefault(t => t.Code.Equals(code) && t.Password.Equals(password));
            return c;
        }
    }
}