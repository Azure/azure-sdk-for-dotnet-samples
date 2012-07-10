//-----------------------------------------------------------------------
// <copyright file="IdentityClaimsAttribute.cs" company="Microsoft">
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
// Provides a dump of the entire Claim to the website through an MVC3 Action Filter. Used for demonstrate purposes and for debugging
//
// </summary>
//----------------------------------------------------------------------------------------------

using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;

namespace Microsoft.Online.Demos.Aadexpense.ActionFilters
{
    public class IdentityClaimsAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            IClaimsPrincipal icp = Thread.CurrentPrincipal as IClaimsPrincipal;
            if (icp != null)
            {
                IClaimsIdentity ici = icp.Identity as IClaimsIdentity;

                filterContext.HttpContext.Response.Write(
                    "Invoice Manager Debug (if you see this, you are not running in prod )<br/><br/>:Claims:<br/>");
                if (ici != null)
                    foreach (Claim c in ici.Claims)
                        filterContext.HttpContext.Response.Write(c.ClaimType + "-" + c.Value + "<br/>");
            }
            base.OnResultExecuted(filterContext);
        }

    }
}