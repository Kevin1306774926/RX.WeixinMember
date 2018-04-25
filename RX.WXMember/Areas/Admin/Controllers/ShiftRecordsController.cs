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
    public class ShiftRecordsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Admin/ShiftRecords
        public ActionResult Index(string query,int? page)
        {
            int pageNumber = page ?? 1;
            var model = db.ShiftRecords.OrderByDescending(t => t.OpTime).ToList();
            return View(model.ToPagedList(pageNumber,20));
        }

        // GET: Admin/ShiftRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftRecord shiftRecord = db.ShiftRecords.Find(id);
            if (shiftRecord == null)
            {
                return HttpNotFound();
            }
            return View(shiftRecord);
        }

        // GET: Admin/ShiftRecords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ShiftRecords/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GroundCode,CheckIn,CheckOut,SaleQty,RefundQty,OfferQty,TotalQty,UserName,SaleAmt,RefundAmt,SumAmt,TotalRestQtyBefor,TotalRestQtyAfter,TotalRestQty,OpTime,MangerCode,Ts,ShiftId")] ShiftRecord shiftRecord)
        {
            if (ModelState.IsValid)
            {
                db.ShiftRecords.Add(shiftRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shiftRecord);
        }

        // GET: Admin/ShiftRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftRecord shiftRecord = db.ShiftRecords.Find(id);
            if (shiftRecord == null)
            {
                return HttpNotFound();
            }
            return View(shiftRecord);
        }

        // POST: Admin/ShiftRecords/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GroundCode,CheckIn,CheckOut,SaleQty,RefundQty,OfferQty,TotalQty,UserName,SaleAmt,RefundAmt,SumAmt,TotalRestQtyBefor,TotalRestQtyAfter,TotalRestQty,OpTime,MangerCode,Ts,ShiftId")] ShiftRecord shiftRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shiftRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shiftRecord);
        }

        // GET: Admin/ShiftRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftRecord shiftRecord = db.ShiftRecords.Find(id);
            if (shiftRecord == null)
            {
                return HttpNotFound();
            }
            return View(shiftRecord);
        }

        // POST: Admin/ShiftRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShiftRecord shiftRecord = db.ShiftRecords.Find(id);
            db.ShiftRecords.Remove(shiftRecord);
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
