//-----------------------------------------------------------------------
// <copyright file="RequireAuthenticatedAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//
// 
//    Copyright 2012 Microsoft Corporation
//    All rights reserved.
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
//  Determines if a user is logged in to the service. If the user is logged in to the service, it provides the
//  ability to query the Azure Active Directory Graph API for values required for the demo application.
// 
// To provide help to the developer in the context of a View, this class inserts data in to the ViewData store
// for retreival latter in the View. 
//
//                ViewData["Name"] // Logged in user's name
//                ViewData["Street"] // Logged in user's street address
//                ViewData["City"] // Logged in user's city
//                ViewData["PostalCode"] // Logged in user's postal code
//                ViewData["Department"] // Logged in user's assigned department
//                ViewData["JobTitle"]  // Logged in user's job title
//                ViewData["UserPrincipalName"] // Logged in user's full username and domain name
//
// </summary>
//----------------------------------------------------------------------------------------------

using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using System;
using System.Web;
using System.Linq;
using Microsoft.Online.Demos.Aadexpense.Models;
using Microsoft.Online.Demos.Aadexpense.ServiceReference1;


namespace Microsoft.Online.Demos.Aadexpense.ActionFilters
{
    public class RequireAuthenticatedAttribute : ActionFilterAttribute, IDisposable {

        public bool IsAuthenticated;
        public bool IsAdministrator;
        public bool IsAllowed;
        public bool IsManager;
        private IClaimsIdentity _claimsIdentity;
        string _email="", _strClaimType;
        private readonly DemoDatabase _db = new DemoDatabase();
        private User _user;
        private Guid _employeepuid;
        // Track whether Dispose has been called.
        private bool _disposed;


        private void Initializer()
        {
            _claimsIdentity = ((IClaimsPrincipal)(Thread.CurrentPrincipal)).Identities[0];

            if (_claimsIdentity != null)
                foreach (var c in _claimsIdentity.Claims)
                {
                    _strClaimType = c.ClaimType;
                    if (_strClaimType.EndsWith("EmailAddress"))
                        _email = c.Value;
                }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (request != null &&
                response != null)
            {
                IsAuthenticated= request.IsAuthenticated;
            
            }

            if (IsAuthenticated)

            {
                Initializer();
                var restAPI = RestApiInterface.Instance;
                

//                var restAPI = new RestApiInterface();
                _user = restAPI.GetUserByEmail(_email);
                filterContext.Controller.ViewData["IsAuthenticated"] = true;

                var k = from r in _db.AuthorizedUsers
                        where r.EmployeeGUID == _user.ObjectId
                        select r;

                if (k.Any())
                {
                    filterContext.Controller.ViewData["IsAuthorized"] = true;
                }

                if (restAPI.IsAdministrator(new Guid(_user.ObjectId.ToString())))
                {
                    filterContext.Controller.ViewData["IsAdministrator"] = true;
                }
            
                if (restAPI.IsManager(new Guid(_user.ObjectId.ToString())))
                {
                    filterContext.Controller.ViewData["IsManager"] = true;
                }

                filterContext.Controller.ViewData["Name"] = _user.DisplayName;
                filterContext.Controller.ViewData["Street"] = _user.StreetAddress;
                filterContext.Controller.ViewData["City"] = _user.City;
                filterContext.Controller.ViewData["PostalCode"] = _user.PostalCode;
                filterContext.Controller.ViewData["Department"] = _user.Department;
                filterContext.Controller.ViewData["JobTitle"] = _user.JobTitle;
                filterContext.Controller.ViewData["UserPrincipalName"] = _user.UserPrincipalName;

            }

            base.OnActionExecuting(filterContext);


        }

        public void Dispose()
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
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.


                // Note disposing has been done.
                _disposed = true;

            }
        }


    }
}
