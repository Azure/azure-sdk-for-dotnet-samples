//-----------------------------------------------------------------------
// <copyright file="AdminController.cs" company="Microsoft">
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
// Provides Controller for the Admin view
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Microsoft.Online.Demos.Aadexpense.ActionFilters;
using Microsoft.Online.Demos.Aadexpense.Models;
using Microsoft.Online.Demos.Aadexpense.ServiceReference1;

namespace Microsoft.Online.Demos.Aadexpense.Controllers
{
    [Authorize]
    [RequireAuthenticated]
    public class AdminController : Controller
    {
        public const string AllUsersViewName = "All Users";
        public const string AdminViewName = "Company Administrators";
        public const string BlockedUsersViewName = "Sign-in blocked User";
        public const string FilteredUsersViewName = "Query Users";
        
        string _domain = "";
        string _email = "";
        string _strClaimType;
        private readonly DemoDatabase _db = new DemoDatabase();
        private readonly RestApiInterface _restAPI = RestApiInterface.Instance;
        private IClaimsIdentity _claimsIdentity;
        private User _user;
        private Guid _employeepuid;
        // Track whether Dispose has been called.
        private bool _disposed;

        private void Initialize()
        {
            _claimsIdentity = ((IClaimsPrincipal)(Thread.CurrentPrincipal)).Identities[0];

            foreach (var c in _claimsIdentity.Claims)
            {
                _strClaimType = c.ClaimType;
                if (_strClaimType.EndsWith("domain"))
                    _domain = c.Value;

                if (_strClaimType.EndsWith("EmailAddress"))
                    _email = c.Value;
            }

            _user = _restAPI.GetUserByEmail(_email);
            _employeepuid = new Guid(_user.ObjectId.ToString());
            List<ReferencedObject> directReports = _restAPI.GetLinks(_employeepuid, "DirectReports");
            List<ReferencedObject> manager = _restAPI.GetLinks(_employeepuid, "Manager");
            if (manager != null && manager.Count != 0)
            {
                new Guid(manager[0].ObjectId.ToString());

            }

        }
        //
        // GET: /Admin/

        public PartialViewResult Search(string q)
        {
            var users = _restAPI.ExecuteQuery(q);
            return PartialView("_UserSearchResults", users);
        }

        public ActionResult QuickSearch(string term)
        {
            var users = _restAPI.ExecuteQuery(term);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Index()
        {
            Initialize();
            var authorizedUsers = from r in _db.AuthorizedUsers
                                  where r.isAuthorized == true
                                  where r.domain == _domain
                           select r;

            return View(authorizedUsers);
        }

        public ActionResult AddUser(Guid employeeId)
        {
            Initialize();
            
            var authorizedUsers = new AuthorizedUser();
            _user = _restAPI.GetUser(employeeId);
            authorizedUsers.isAuthorized = true;
            authorizedUsers.EmployeeGUID = employeeId;
            authorizedUsers.EmployeeName = _user.DisplayName;
            authorizedUsers.EmployeeDepartment = _user.Department;
            authorizedUsers.domain = _domain;
            _db.AuthorizedUsers.Add(authorizedUsers);
            _db.SaveChanges();
            return RedirectToAction("AddUsers");
        }

        public ActionResult RemoveUser(int id)
        {
            AuthorizedUser authorizedUser = _db.AuthorizedUsers.Find(id);
            _db.AuthorizedUsers.Remove(authorizedUser);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UserDetails(Guid id)
        {
            User user = _restAPI.GetUser(id);
            List<ReferencedObject> directReports = _restAPI.GetLinks(id, "DirectReports");
            List<ReferencedObject> manager = _restAPI.GetLinks(id, "Manager");
            ViewBag.DirectReports = directReports;
            if (manager != null && manager.Count != 0)
            {
                ViewBag.Manager = manager[0].DisplayName;
            }
            return View(user);
        }
        
        public ActionResult AddUsers(string type, string SearchString)
        {
            if (type == FilteredUsersViewName)
            {
                ViewBag.FilteredUserView = true;
                if (!string.IsNullOrEmpty(SearchString))
                {
                    List<User> users = _restAPI.ExecuteQuery(SearchString);
                    return View(users);
                }

                return View();
            }

            else if (type == "next")
            {
                var users = _restAPI.GetNextPage();
                if (_restAPI.ContinuationToken != null)
                {
                    ViewBag.HasNextPage = true;
                }
                if (_restAPI.CurrentPage != 1)
                {
                    ViewBag.HasPrevPage = true;
                }

                return View(users);
            }
            else
            {
                if (type == "prev")
                {
                    var users = _restAPI.GetPrevPage();
                    if (_restAPI.ContinuationToken != null)
                    {
                        ViewBag.HasNextPage = true;
                    }
                    if (_restAPI.CurrentPage != 1)
                    {
                        ViewBag.HasPrevPage = true;
                    }

                    return View(users);
                }
                if (type == "current")
                {
                    if (_restAPI.ContinuationToken != null)
                    {
                        ViewBag.HasNextPage = true;
                    }
                    if (_restAPI.CurrentPage != 1)
                    {
                        ViewBag.HasPrevPage = true;
                    }

                    return View(_restAPI.CurrentDisplayedList);
                }

                else
                {
                    var usersUnfiltered = _restAPI.GetAllUsers();
                    var users = (from user1 in usersUnfiltered let k = _db.AuthorizedUsers.Where(r => r.EmployeeGUID == user1.ObjectId) where !k.Any() select user1).ToList();

                    if (_restAPI.ContinuationToken != null)
                    {
                        ViewBag.HasNextPage = true;
                    }

                    return View(users);
                }
            }
        }

                protected virtual void Dispose()

        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }

                protected override void Dispose(bool disposing)
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
                            _restAPI.Dispose();
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
