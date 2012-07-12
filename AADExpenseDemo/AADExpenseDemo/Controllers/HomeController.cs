//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
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
// Provides Controller for the Home View
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Online.Demos.Aadexpense.ActionFilters;
using Microsoft.Online.Demos.Aadexpense.Models;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Microsoft.Online.Demos.Aadexpense.ServiceReference1;


namespace Microsoft.Online.Demos.Aadexpense.Controllers
{
    [RequireAuthenticated]
    public class HomeController : Controller
    {

        string _email = "", _strClaimType;
        readonly IClaimsIdentity _claimsIdentity = ((IClaimsPrincipal)(Thread.CurrentPrincipal)).Identities[0];

        private readonly DemoDatabase _db = new DemoDatabase();

        private User _user;
        private Guid _employeepuid;
        private bool _isAuthenticated;
        private bool _isAllowed;
        private bool _isAdmin;
                        // Track whether Dispose has been called.
        private bool disposed;
     
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
                        ;
                    if (_strClaimType.EndsWith("FirstName"))
                        ;
                    if (_strClaimType.EndsWith("LastName"))
                        ;
                    if (_strClaimType.EndsWith("EmailAddress"))
                        _email = c.Value;
                }

                    RestApiInterface restAPI = RestApiInterface.Instance;
                _user = restAPI.GetUserByEmail(_email);
                _employeepuid = new Guid(_user.ObjectId.ToString());
                List<ReferencedObject> directReports = restAPI.GetLinks(_employeepuid, "DirectReports");
                List<ReferencedObject> manager = restAPI.GetLinks(_employeepuid, "Manager");
                    if (manager != null && manager.Count != 0)
                {
                    new Guid(manager[0].ObjectId.ToString());

                }

            } 
        }

        public ActionResult Index()
             {
            Initialize();

            if (_isAuthenticated)
            {
                ViewBag.Name = _user.DisplayName;
                ViewBag.JobTitle = _user.JobTitle;
                ViewBag.Department = _user.Department;

                if (!_isAllowed && !_isAdmin) return RedirectToAction("NotAuthorized");
            }

            return View();
        }

        public ActionResult About()
        {
            Initialize();
           
            ViewBag.Message = "Welcome to the Azure Active Directory Demonstration";

            return View();
        }

        public ActionResult NotAuthorized()
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
                    if (!this.disposed)
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
                        disposed = true;

                    }
                }
    }
}
