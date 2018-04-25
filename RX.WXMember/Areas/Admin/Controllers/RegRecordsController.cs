using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RX.WXMember.Models;
using RX.WXMember.Filters;
using PagedList;
using RX.WXMember.Comm;

namespace RX.WXMember.Areas.Admin.Controllers
{
    [CheckLoginFilter]
    public class RegRecordsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Admin/RegRecords
        public ActionResult Index(string query,int? page)
        {
            var model = db.RegRecords.OrderByDescending(t => t.CreateTime).ToList();
            int pageNumber = page ?? 1;
            return View(model.ToPagedList(pageNumber,20));
        }

        // GET: Admin/RegRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegRecord regRecord = db.RegRecords.Find(id);
            if (regRecord == null)
            {
                return HttpNotFound();
            }
            return View(regRecord);
        }

        // GET: Admin/RegRecords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/RegRecords/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Record,GroundCode,RegCode,CreateTime,CreateUser")] RegRecord regRecord)
        {
            if (ModelState.IsValid)
            {
                db.RegRecords.Add(regRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(regRecord);
        }

        // GET: Admin/RegRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegRecord regRecord = db.RegRecords.Find(id);
            if (regRecord == null)
            {
                return HttpNotFound();
            }
            return View(regRecord);
        }

        // POST: Admin/RegRecords/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Record,GroundCode,RegCode,CreateTime,CreateUser")] RegRecord regRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(regRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(regRecord);
        }

        // GET: Admin/RegRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegRecord regRecord = db.RegRecords.Find(id);
            if (regRecord == null)
            {
                return HttpNotFound();
            }
            return View(regRecord);
        }

        // POST: Admin/RegRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RegRecord regRecord = db.RegRecords.Find(id);
            db.RegRecords.Remove(regRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
