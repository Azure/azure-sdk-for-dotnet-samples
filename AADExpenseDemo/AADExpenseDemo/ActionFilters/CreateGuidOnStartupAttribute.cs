//-----------------------------------------------------------------------
// <copyright file="CreateGuidOnStartupAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
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
// Creates a GUID on startup of the application on first use. This is used to register the application
// with the Office365 tenant.  
// 
// </summary>
//----------------------------------------------------------------------------------------------

using System.Web.Mvc;
using System.Linq;
using Microsoft.Online.Demos.Aadexpense.Models;

namespace Microsoft.Online.Demos.Aadexpense.ActionFilters
{
    public class CreateGuidOnStartupAttribute : ActionFilterAttribute
    {
        private readonly DemoDatabase _db = new DemoDatabase();
        string _guids; 

        public void CheckforGuid()

        {
            var query = from r in _db.Startups select r;

            if (query.Any()) return;
            var start = new Startup()
                            {

                                AppPrincipalId = System.Guid.NewGuid()
                            };
            _db.Startups.Add(start);
            _db.SaveChanges();
        }

        public string GetGuid()
        {
            var query = from r in _db.Startups select r;

            if (!query.Any())
            {
                return null;
            }
            var t = from p in _db.Startups select p.AppPrincipalId;
            foreach (var guid in t)
            {
                _guids = guid + "";
            }

            return _guids;
        }
    }
}