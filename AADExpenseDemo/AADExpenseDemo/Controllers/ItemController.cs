//-----------------------------------------------------------------------
// <copyright file="ItemController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing 
// permissions and limitations under the License.
// </copyright>
//
// <summary>
//     
// Provides Controller for the Item view
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Online.Demos.Aadexpense.ActionFilters;
using Microsoft.Online.Demos.Aadexpense.Models;

namespace Microsoft.Online.Demos.Aadexpense.Controllers
{ 
    [Authorize]
    [RequireAuthenticated]
    public class ItemController : Controller
    {
        private readonly DemoDatabase _db = new DemoDatabase();
        // Track whether Dispose has been called.
        private bool _disposed;

        //
        // GET: /Item/

        public ViewResult Index()
        {
            return View(_db.Items.ToList());
        }

        //
        // GET: /Item/Details/5

        public ViewResult Details(int id)
        {
            Item item = _db.Items.Find(id);
            return View(item);
        }

        //
        // GET: /Item/Create

        public ActionResult Create(int invoiceID)
        {
            var invoices = _db.Invoices.Single(r => r.InvoiceID == invoiceID);
            ViewBag.InvoiceName = invoices.Reason;
            ViewBag.InvoiceEmployee = invoices.Employee;
            ViewBag.InvoiceID = invoices.InvoiceID;
            return View(new Item());
        } 

        //
        // POST: /Item/Create

        [HttpPost]
        public ActionResult Create(int invoiceID, Item item)
        {
            var invoices = _db.Invoices.Single(r => r.InvoiceID == invoiceID);
            ViewBag.InvoiceName = invoices.Reason;
            ViewBag.InvoiceEmployee = invoices.Employee;
            ViewBag.InvoiceID = invoices.InvoiceID;
            
            if (ModelState.IsValid)
            {
                
                invoices.Items.Add(item);
                _db.SaveChanges();
                return RedirectToAction("Details", "Invoice", new { id = invoices.InvoiceID });  
            }

            return View(item);
        }
        
        //
        // GET: /Item/Edit/5
 
        public ActionResult Edit(int id)
        {
            Item item = _db.Items.Find(id);

            var invoice = from k in _db.Invoices
                          from t in k.Items
                          where t.ItemID == id
                          select k;

            ViewBag.InvoiceID = invoice.First().InvoiceID;
            return View(item);
        }

        //
        // POST: /Item/Edit/5

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            var invoice = from k in _db.Invoices
                          from t in k.Items
                          where t.ItemID == item.ItemID
                          select k;


            if (ModelState.IsValid)
            {
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Details", "Invoice", new {id = invoice.First().InvoiceID});
            }
            return View(item);
        }

        //
        // GET: /Item/Delete/5
 
        public ActionResult Delete(int id)
        {
            Item item = _db.Items.Find(id);

            var invoice = from k in _db.Invoices
                          from t in k.Items
                          where t.ItemID == id
                          select k;

            ViewBag.InvoiceID = invoice.First().InvoiceID;

            return View(item);
        }

        //
        // POST: /Item/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Item item = _db.Items.Find(id);

            var invoice = from k in _db.Invoices
                          from t in k.Items
                          where t.ItemID == id
                          select k;
            var invId = invoice.First().InvoiceID;

            _db.Items.Remove(item);
            _db.SaveChanges();
            return RedirectToAction("Details", "Invoice", new { id = invId });
        }

        protected virtual void Dispose()

        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _db.Dispose();
                    base.Dispose(disposing);
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.


                // Note disposing has been done.
                _disposed = true;

            }

        }
    }
}