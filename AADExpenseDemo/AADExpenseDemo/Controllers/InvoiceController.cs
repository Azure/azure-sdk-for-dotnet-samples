//-----------------------------------------------------------------------
// <copyright file="InvoiceController.cs" company="Microsoft">
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
// Provides Controller for the Invoice view
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Microsoft.Online.Demos.Aadexpense.ActionFilters;
using Microsoft.Online.Demos.Aadexpense.Models;
using Microsoft.Online.DirectoryApi.ServiceReference1;

namespace Microsoft.Online.Demos.Aadexpense.Controllers
{
    [Authorize]
    [RequireAuthenticated]
    public class InvoiceController : Controller
    {
        public const string AllUsersViewName = "All Users";
        public const string AdminViewName = "Company Administrators";
        public const string BlockedUsersViewName = "Sign-in blocked User";
        public const string FilteredUsersViewName = "Query Users";
        private string _domain = "", _email = "", _strClaimType;
        private IClaimsIdentity _claimsIdentity;
        private readonly DemoDatabase _db = new DemoDatabase();
        private readonly RestApiInterface _restApi = RestApiInterface.Instance;
        private User _user;
        private string _managerName;
        private Guid _managerPuid;
        private Guid _employeepuid;
        // Track whether Dispose has been called.
        private bool _disposed;

        private void Initialize()
        {
            _claimsIdentity = ((IClaimsPrincipal) (Thread.CurrentPrincipal)).Identities[0];

            foreach (var c in _claimsIdentity.Claims)
            {
                _strClaimType = c.ClaimType;
                if (_strClaimType.EndsWith("domain"))
                    _domain = c.Value;
                if (_strClaimType.EndsWith("EmailAddress"))
                    _email = c.Value;
            }

            _user = _restApi.GetUserByEmail(_email);
            _employeepuid = new Guid(_user.ObjectId.ToString());
            List<ReferencedObject> directReports = _restApi.GetLinks(_employeepuid, "DirectReports");
            List<ReferencedObject> manager = _restApi.GetLinks(_employeepuid, "Manager");
            if (manager == null || manager.Count == 0) return;
            _managerName = manager[0].DisplayName;
            _managerPuid = new Guid(manager[0].ObjectId.ToString());
        }

        public ActionResult QueryUsers(string searchString)
        {
            if (searchString == null) throw new ArgumentNullException("searchString");
            List<User> users = new List<User>();
            if (!string.IsNullOrEmpty(searchString))
            {
                users = _restApi.ExecuteQuery(searchString);
            }

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users(string type, string searchString)
        {
            if (searchString == null) throw new ArgumentNullException("searchString");

            if (type == AdminViewName)
            {
                var adminUsers = _restApi.GetAdministrators();
                return View(adminUsers);
            }
            if (type == BlockedUsersViewName)
            {
                var adminUsers = _restApi.GetBlockedUsers();
                return View(adminUsers);
            }
            if (type == FilteredUsersViewName)
            {
                ViewBag.FilteredUserView = true;
                if (!string.IsNullOrEmpty(searchString))
                {
                    List<User> users = _restApi.ExecuteQuery(searchString);
                    return View(users);
                }

                return View();
            }

            else if (type == "next")
            {
                var users = _restApi.GetNextPage();
                if (_restApi.ContinuationToken != null)
                {
                    ViewBag.HasNextPage = true;
                }
                if (_restApi.CurrentPage != 1)
                {
                    ViewBag.HasPrevPage = true;
                }

                return View(users);
            }
            else if (type == "prev")
            {
                var users = _restApi.GetPrevPage();
                if (_restApi.ContinuationToken != null)
                {
                    ViewBag.HasNextPage = true;
                }
                if (_restApi.CurrentPage != 1)
                {
                    ViewBag.HasPrevPage = true;
                }

                return View(users);
            }
            else if (type == "current")
            {
                if (_restApi.ContinuationToken != null)
                {
                    ViewBag.HasNextPage = true;
                }
                if (_restApi.CurrentPage != 1)
                {
                    ViewBag.HasPrevPage = true;
                }

                return View(_restApi.CurrentDisplayedList);
            }

            else
            {
                var users = _restApi.GetAllUsers();
                if (_restApi.ContinuationToken != null)
                {
                    ViewBag.HasNextPage = true;
                }

                return View(users);
            }
        }

        public ActionResult UserDetails(Guid id)
        {
            User user = _restApi.GetUser(id);
            List<ReferencedObject> directReports = _restApi.GetLinks(id, "DirectReports");
            List<ReferencedObject> manager = _restApi.GetLinks(id, "Manager");
            ViewBag.DirectReports = directReports;
            if (manager != null && manager.Count != 0)
            {
                ViewBag.Manager = manager[0].DisplayName;
            }
            return View(user);
        }


        //
        // GET: /Invoice/

        public ViewResult Index()
        {
            Initialize();
            var invoices = from r in _db.Invoices
                           where r.EmployeePUID == _employeepuid
                           where r.CompanyDomain == _domain
                           orderby r.SubmitDate
                           select r;

            return View(invoices);
        }

        //
        // GET: /Invoice/Details/5

        public ViewResult Details(int id)
        {
            Initialize();
            Invoice invoice = _db.Invoices.Find(id);
            ViewBag.Reason = invoice.Reason;
            return View(invoice);
        }

        //
        // GET: /Invoice/Create

        public ActionResult Create()
        {
            Initialize();
            var invoice = new Invoice
                              {Employee = _user.DisplayName, EmployeePUID = _employeepuid, CompanyDomain = _domain};


            return View(invoice);
        }

        //
        // POST: /Invoice/Create

        [HttpPost]
        public ActionResult Create(Invoice invoice)
        {
            Initialize();

            if (ModelState.IsValid)
            {
                invoice.EmployeePUID = _employeepuid;
                invoice.CompanyDomain = _domain;
                invoice.InvoiceStatus = 0;

                _db.Invoices.Add(invoice);
                _db.SaveChanges();

                return RedirectToAction("Details", new {id = invoice.InvoiceID});

            }
            return View(invoice);

        }

        //
        // GET: /Invoice/Edit/5

        public ActionResult Edit(int id)
        {
            Invoice invoice = _db.Invoices.Find(id);
            return View(invoice);
        }

        //
        // POST: /Invoice/Edit/5

        [HttpPost]
        public ActionResult Edit(Invoice invoice)
        {
            Initialize();

            if (ModelState.IsValid)
            {
                invoice.EmployeePUID = _employeepuid;
                invoice.CompanyDomain = _domain;
                _db.Entry(invoice).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invoice);
        }

        //
        // GET: /Invoice/Submit/5

        public ActionResult Submit()
        {
            Initialize();
            var item = new Manager();

            if (_managerName == null) return RedirectToAction("NoManager");

            item.ManagerPUID = _managerPuid;
            item.ManagerName = _managerName;



            return View(item);
        }

        //
        // POST: /Invoice/Submit/5

        [HttpPost]
        public ActionResult Submit(int invoiceId, Manager item)
        {
            Initialize();
            var invoices = _db.Invoices.Single(r => r.InvoiceID == invoiceId);
            item.ManagerPUID = _managerPuid;
            item.ManagerName = _managerName;

            if (ModelState.IsValid)
            {
                invoices.InvoiceStatus = 1;
                invoices.CompanyDomain = _domain;
                invoices.Managers.Add(item);
                _db.SaveChanges();
                return RedirectToAction("Index", "Invoice");
            }

            return View(item);
        }

        //
        // GET: /Invoice/Delete/5

        public ActionResult Delete(int id)
        {

            Invoice invoice = _db.Invoices.Find(id);


            return View(invoice);
        }

        //
        // POST: /Invoice/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = _db.Invoices.Find(id);

            var itemFetch = from k in _db.Items
                            from t in _db.Invoices
                            where t.InvoiceID == id
                            select k;

            foreach (var itemDetail in itemFetch)
            {
                _db.Items.Remove(itemDetail);
            }


            _db.Invoices.Remove(invoice);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult NoManager()
        {
            return View();
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
                    _restApi.Dispose();
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