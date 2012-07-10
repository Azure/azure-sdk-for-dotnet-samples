//-------------------------------------------------------------------------------------------------
// <copyright file="InvoiceModels.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
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
// Model for the Invoice Controller
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
    public class Invoice
    {
        public virtual int InvoiceID { get; set; }

        [RequiredAttribute]
        [DataType(DataType.Text)]
        [Display(Name = "Employee Name")]
        public virtual string Employee { get; set; }

        [RequiredAttribute]
        [DataType(DataType.Text)]
        [Display(Name = "Reason for Expense Report")]
        public virtual string Reason { get; set; }

        [RequiredAttribute]
        [DataType(DataType.Date)]
        [Display(Name = "Expense Start Date")]
        public virtual DateTime SubmitDate { get; set; }

        [RequiredAttribute]
        [DataType(DataType.Date)]
        [Display(Name = "Expense End Date")]
        public virtual DateTime EndDate { get; set; }

        public virtual int InvoiceStatus { get; set; }
        public virtual Guid CompanyPUID { get; set; }
        public virtual string CompanyDomain { get; set; }
        public virtual Guid EmployeePUID { get; set; }

      
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Manager> Managers { get; set; }

    }

    public class Item

    {

        public virtual int ItemID { get; set; }

        [RequiredAttribute]
        [DataType(DataType.Date)]
        [Display(Name = "Expense Date")]
        public virtual DateTime date { get; set; }
        
        [RequiredAttribute]
        [DataType(DataType.Text)]
        [Display(Name = "Expense Name")]
        public virtual string name { get; set; }

        [RequiredAttribute]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")]
        public virtual decimal amount { get; set; }
}

    public class Manager
    {
        public virtual int ManagerID { get; set; }
        public virtual Guid ManagerPUID { get; set; }
        public virtual string ManagerName { get; set; }
        public virtual string ManagerComment { get; set; }
    }


}
