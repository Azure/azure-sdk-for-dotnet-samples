//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
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
//
// </summary>
//----------------------------------------------------------------------------------------------
namespace Microsoft.Samples.Waad.Federation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class Constants
    {
        internal const string RepositoryFilename = "trustedIssuers.xml";
        internal const string LoginUrlPattern = "https://accounts.accesscontrol.windows.net/v2/wsfederation?wa=wsignin1.0&wtrealm={0}&wreply={1}";
        internal const string RepositoryIssuerElementName = "issuer";
        internal const string RepositoryNameAttribute = "name";
        internal const string RepositoryDisplayNameAttribute = "displayName";
        internal const string RepositoryRealmAttribute = "realm";
    }
}
