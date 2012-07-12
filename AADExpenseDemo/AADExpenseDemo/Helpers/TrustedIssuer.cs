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

namespace Microsoft.Online.Demos.Aadexpense.Helpers
{
    using System;
    using System.Web;

    public class TrustedIssuer
    {
        public TrustedIssuer(string name, string displayName, string spn)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Spn = spn;
            var reply = Microsoft.IdentityModel.Configuration.MicrosoftIdentityModelSection.DefaultServiceElement.FederatedAuthentication.WSFederation.Reply;
            this.LoginUrl = new Uri(string.Format(Constants.LoginUrlPattern, HttpContext.Current.Server.UrlEncode(spn), HttpContext.Current.Server.UrlEncode(reply)));
        }

        public TrustedIssuer(string name, string displayName, string spn, Uri replyUrl)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Spn = spn;
            this.LoginUrl = new Uri(string.Format(Constants.LoginUrlPattern, HttpContext.Current.Server.UrlEncode(spn), HttpContext.Current.Server.UrlEncode(replyUrl.AbsoluteUri)));
        }

        public string Name { get; internal set; }

        public string DisplayName { get; internal set; }

        public string Spn { get; internal set; }

        public Uri LoginUrl { get; internal set; }
    }
}
