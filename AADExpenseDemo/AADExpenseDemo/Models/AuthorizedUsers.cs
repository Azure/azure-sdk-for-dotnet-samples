//-------------------------------------------------------------------------------------------------
// <copyright file="AuthorizedUsers.cs" company="Microsoft">
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
//     Model to maintain authorized users of the application. We determine if we should display the "Submit Expense Reports" tab based on matching
//     the EmployeeGUID and the domain to the information retreived for the user in the Graph API
// </summary>
//-------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.Online.Demos.Aadexpense.Models
{
    public class AuthorizedUser
    {
        public virtual int AuthorizedUserID { get; set; }
        public virtual Guid EmployeeGUID { get; set; }
        public virtual string EmployeeName { get; set; }
        public virtual string EmployeeDepartment { get; set; }
        public virtual bool isAuthorized { get; set; }
        public virtual string domain { get; set; }
    }
        

}