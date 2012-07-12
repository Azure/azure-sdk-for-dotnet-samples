//-----------------------------------------------------------------------
// <copyright file="Connection.cs" company="Microsoft">
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
// Provides a connection to the Azure Active Directory REST API    
// 
// This class implements the Factory Connection and Connection interfaces, allowing for the IFactoryConnection class to find
// this implementation when it builds it's list of available Connection objects which it can instantiate.
//
// </summary>
//----------------------------------------------------------------------------------------------

using Microsoft.Online.Demos.Aadexpense.Models;

namespace Microsoft.Online.Demos.Aadexpense.Helpers
{
    public class Connection : IFactoryConnection, IConnection
    {
        private const string FactoryClassName = "RESTAPI2";

        public object GetFactoryKey()
        {
            return FactoryClassName;
        }

        public virtual RestApiInterface aConnection()
        {
            using (RestApiInterface restAPI = RestApiInterface.Instance)
            {
                return restAPI;
            }
        }
    }
}

