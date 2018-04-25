using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RX.WXMember.Models;

namespace RX.WXMember
{
    public class ShiftRecordsController : ApiController
    {
        MyDbContext db = new MyDbContext();
        
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(string id)
        {
            return db.GetMaxId(id).ToString();
        }

        // POST api/<controller>
        public void Post([FromBody]IEnumerable<ShiftRecord> value)
        {
            db.ShiftRecords.AddRange(value);
            db.SaveChanges();
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}