//-----------------------------------------------------------------------
// <copyright file="ManagerController.cs" company="Microsoft">
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
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Microsoft.Online.Demos.Aadexpense.ActionFilters;
using Microsoft.Online.Demos.Aadexpense.Models;
using Microsoft.Online.DirectoryApi.ServiceReference1;

namespace Microsoft.Online.Demos.Aadexpense.Controllers
{ 
    [Authorize]
    [RequireAuthenticated]
    public class ManagerController : Controller
    {
        private readonly DemoDatabase _db = new DemoDatabase();
        private readonly RestApiInterface _restApi = RestApiInterface.Instance;
        string _domain = "", _first = "", _last = "", _email = "", _strClaimType;
        readonly IClaimsIdentity _claimsIdentity = ((IClaimsPrincipal)(Thread.CurrentPrincipal)).Identities[0];
        private User user;
        private string _managerName;
        private Guid _managerPuid;
        private string _directReports;
        private Guid _employeepuid;
        private bool _isAuthenticated;
        private bool _isAllowed;
        private bool _isAdmin;
                        // Track whether Dispose has been called.
        private bool _disposed;


        private void Initialize()
        {
            _isAuthenticated = (bool)(ViewData["IsAuthenticated"] ?? false);
            _isAllowed = (bool)(ViewData["IsAuthorized"] ?? false);
            _isAdmin = (bool)(ViewData["IsAdministrator"] ?? false);



            if (_isAuthenticated)
            {

                foreach (Claim c in _claimsIdentity.Claims)
                {
                    _strClaimType = c.ClaimType;
                    if (_strClaimType.EndsWith("domain"))
                        _domain = c.Value;
                    if (_strClaimType.EndsWith("FirstName"))
                        _first = c.Value;
                    if (_strClaimType.EndsWith("LastName"))
                        _last = c.Value;
                    if (_strClaimType.EndsWith("EmailAddress"))
                        _email = c.Value;
                }

                user = _restApi.GetUserByEmail(_email);
                _employeepuid = new Guid(user.ObjectId.ToString());
                List<ReferencedObject> directReports = _restApi.GetLinks(_employeepuid, "DirectReports");
                List<ReferencedObject> manager = _restApi.GetLinks(_employeepuid, "Manager");
                _directReports = directReports.ToString();
                if (manager != null && manager.Count != 0)
                {
                    _managerName = manager[0].DisplayName;
                    _managerPuid = new Guid(manager[0].ObjectId.ToString());

                }

            }
        }
        //
        // GET: /Manager/

        public ViewResult Index()
        {

            Initialize();

            var invoices = from r in _db.Invoices
                           from k in r.Managers
                           where r.InvoiceStatus == 1
                           where r.CompanyDomain == _domain
                           where k.ManagerPUID == user.ObjectId
                           orderby r.SubmitDate
                           select r;

            return View(invoices);

        }

        //
        // GET: /Manager/Details/5

        public ViewResult Details(int id)
        {

            Initialize();

            Invoice invoice = _db.Invoices.Find(id);
            return View(invoice);
        }

        public ActionResult Approve(int id)
        {
            
            Invoice invoice = _db.Invoices.Find(id);
            invoice.InvoiceStatus = 2;
            _db.Entry(invoice).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index"); 
        }


        public ActionResult Reject(int id)
        {
            Invoice invoice = _db.Invoices.Find(id);
            invoice.InvoiceStatus = 3;
            _db.Entry(invoice).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //

        //
        // GET: /Manager/Edit/5
 
        public ActionResult Edit(int id)
        {
            Invoice invoice = _db.Invoices.Find(id);
            return View(invoice);
        }

        //
        // POST: /Manager/Edit/5

        [HttpPost]
        public ActionResult Edit(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(invoice).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoice);
        }

        //
        // GET: /Manager/Delete/5
 
        public ActionResult Delete(int id)
        {
            Invoice invoice = _db.Invoices.Find(id);
            return View(invoice);
        }

        //
        // POST: /Manager/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Invoice invoice = _db.Invoices.Find(id);
            _db.Invoices.Remove(invoice);
            _db.SaveChanges();
            return RedirectToAction("Index");
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