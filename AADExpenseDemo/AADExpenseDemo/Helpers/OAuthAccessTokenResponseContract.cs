//-------------------------------------------------------------------------------------------------
// <copyright file="OAuthAccessTokenResponseContract.cs" company="Microsoft">
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
//     
// <summary>OAuth access token response data contract.</summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Online.DirectoryApi.TokenHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// OAuth access token response
    /// </summary>
    [DataContract]
    public class OAuthAccessTokenResponseContract
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the ExpiresIn.
        /// </summary>
        [DataMember(Name = "expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the expires on.
        /// </summary>
        [DataMember(Name = "expires_on")]
        public long ExpiresOn { get; set; }

        /// <summary>
        /// Gets or sets the not before.
        /// </summary>
        [DataMember(Name = "not_before")]
        public long NotBefore { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
    }
}
