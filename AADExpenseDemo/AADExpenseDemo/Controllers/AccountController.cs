//-----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Microsoft">
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
// Provides Controller for Account View
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Online.Demos.Aadexpense.Models;

namespace Microsoft.Online.Demos.Aadexpense.Controllers
{
    public class AccountController : Controller
    {
        readonly DemoDatabase _db = new DemoDatabase();
                        // Track whether Dispose has been called.
        private bool _disposed;
        //
        // GET: /Account/

        public ActionResult Login()
        {

            return View(_db.Signups.ToList());
        }

        public ActionResult Logout()
        {
            return View();
        }

                protected new virtual void Dispose()

        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }

                protected new virtual void Dispose(bool disposing)
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
