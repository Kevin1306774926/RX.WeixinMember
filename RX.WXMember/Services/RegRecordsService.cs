using RX.WXMember.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RX.WXMember.Services
{
    public class RegRecordsService
    {
        MyDbContext db = new MyDbContext();

        public RegRecordsService()
        {

        }

        public RegRecord Add(RegRecord m)
        {
            var model= db.RegRecords.Add(m);
            db.SaveChanges();
            return model;
        }
    }
}