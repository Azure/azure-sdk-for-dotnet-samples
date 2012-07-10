//-------------------------------------------------------------------------------------------------
// <copyright file="TokenHeader.cs" company="Microsoft">
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
// <summary>ACS token header data contract</summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Online.DirectoryApi.TokenHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Web.Script.Serialization;
    

    /// <summary>
    /// ACS token header contract
    /// </summary>
    public class TokenHeader
    {
        /// <summary>
        /// Initializes a new instance of the TokenHeader class.
        /// </summary>
        public TokenHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenHeader class.
        /// </summary>
        /// <param name="algo">Signing algorithm.</param>
        /// <param name="hash">Certificate hash.</param>
        public TokenHeader(string algo, string hash)
        {
            this.TokenType = "JWT";
            this.Algorithm = algo;
            this.CertificateHash = hash;
        }

        /// <summary>
        /// Gets or sets the token type (JWT).
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the ExpiresIn.
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// Gets or sets the certificate hash.
        /// </summary>
        public string CertificateHash { get; set; }

        /// <summary>
        /// Encodes the tokens into JSON format.
        /// </summary>
        /// <returns>OtherClaims encoded in JSON</returns>
        public string EncodeToJson()
        {
            Dictionary<string, string> allClaims = new Dictionary<string, string>
                                                       {{"typ", this.TokenType}, {"alg", this.Algorithm}};

            if (!String.IsNullOrEmpty(this.CertificateHash))
            {
                allClaims.Add("x5t", this.CertificateHash);
            }

            JavaScriptSerializer jserializer = new JavaScriptSerializer();
            return jserializer.Serialize(allClaims);
        }
    }
}
