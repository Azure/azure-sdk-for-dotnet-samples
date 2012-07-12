//-------------------------------------------------------------------------------------------------
// <copyright file="SignupModel.cs" company="Microsoft">
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
// Provides the SignUp model for the Signup Controller
//     
// </summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Microsoft.Online.Demos.Aadexpense.Models
{
    public class Signup
    {
        public virtual int SignupID { get; set; }


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Company ID (from PowerShell script)")]
        public virtual string CompanyId { get; set;  }


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Application Principal ID (from PowerShell script)")] 
        public virtual String AppPrincipalId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Company Name (to identify this tenant during sign-on")]
        public virtual String Description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Domain name of tenant (example: contoso.onmicrosoft.com")]
        public virtual String DomainName { get; set; }

        public virtual String ServicePrincipalURL { get; set; }

    }

}