//-----------------------------------------------------------------------
// <copyright file="TrustedIssuersRepository.cs" company="Microsoft">
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
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;

    public class TrustedIssuersRepository
    {
        private string repositoryFileName;

        public TrustedIssuersRepository()
        {
            this.repositoryFileName = Constants.RepositoryFilename;
        }

        public TrustedIssuersRepository(string repositoryFileName)
        {
            this.repositoryFileName = repositoryFileName;
        }

        public IEnumerable<TrustedIssuer> GetTrustedIdentityProviderUrls()
        {
            var doc = XDocument.Load(this.GetCompleteRepositoryFilename());
            
            return doc.Root.Descendants(Constants.RepositoryIssuerElementName)
                           .Select(e => new TrustedIssuer(
                                    e.Attribute(Constants.RepositoryNameAttribute).Value,
                                    e.Attribute(Constants.RepositoryDisplayNameAttribute).Value,
                                    e.Attribute(Constants.RepositoryRealmAttribute).Value));
        }

        public TrustedIssuer GetTrustedIdentityProviderUrl(string name, Uri replyUrl)
        {
            var doc = XDocument.Load(this.GetCompleteRepositoryFilename());

            return doc.Root.Descendants(Constants.RepositoryIssuerElementName)
                           .Select(e => new TrustedIssuer(
                                    e.Attribute(Constants.RepositoryNameAttribute).Value,
                                    e.Attribute(Constants.RepositoryDisplayNameAttribute).Value,
                                    e.Attribute(Constants.RepositoryRealmAttribute).Value,
                                    replyUrl))
                           .SingleOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        private string GetCompleteRepositoryFilename()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath("~/" + this.repositoryFileName);
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.repositoryFileName);
            }
        }
    }
}