//-----------------------------------------------------------------------
// <copyright file="IFactoryConnection.cs" company="Microsoft">
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
// Provides an interface in to the IFactoryConnection. Any Connection object that implements both of these interfaces can be found by the ConnectionFactory
//
// </summary>
//----------------------------------------------------------------------------------------------

using Microsoft.Online.Demos.Aadexpense.Models;

namespace Microsoft.Online.Demos.Aadexpense.Helpers
{
    public interface IFactoryConnection
    {
        object GetFactoryKey();


    }

    public interface IConnection
    {
        RestApiInterface aConnection();

    }
}

