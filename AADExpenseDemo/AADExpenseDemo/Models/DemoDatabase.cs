//-------------------------------------------------------------------------------------------------
// <copyright file="DemoDatabase.cs" company="Microsoft">
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
//     This is the Model class that creates the database using Entity Framework's code first capability. All new Models you may create 
//     will need to have an entry in this class.
// </summary>
//-------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Microsoft.Online.Demos.Aadexpense.Models
{
    public class DemoDatabase : DbContext
    {

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Signup> Signups { get; set; }
        public DbSet<Startup> Startups { get; set; }
        public DbSet<AuthorizedUser> AuthorizedUsers { get; set;  }
    }
}