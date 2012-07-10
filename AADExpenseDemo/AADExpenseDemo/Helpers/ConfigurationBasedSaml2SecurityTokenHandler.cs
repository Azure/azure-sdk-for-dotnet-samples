//-----------------------------------------------------------------------
// <copyright file="ConfigurationBasedSaml2SecurityTokenHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//
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
//
// </summary>
//----------------------------------------------------------------------------------------------

using Microsoft.Online.Demos.Aadexpense.Models;

namespace Microsoft.Online.Demos.Aadexpense.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens.Saml2;


    public class ConfigurationBasedSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        readonly DemoDatabase db = new DemoDatabase();
        protected override void ValidateConditions(Saml2Conditions conditions, bool enforceAudienceRestriction)
        {
            base.ValidateConditions(conditions, false);

            if (enforceAudienceRestriction)
            {
                var allowedAudienceUris = this.GetAllowedAudienceUris();

                if (allowedAudienceUris.Count == 0)
                {
                    throw new InvalidOperationException("the audience uri repository is empty");
                }

                if ((conditions == null) || (conditions.AudienceRestrictions.Count == 0))
                {
                    throw new AudienceUriValidationFailedException("the conditions audience uri collection is empty");
                }

                foreach (Saml2AudienceRestriction restriction in conditions.AudienceRestrictions)
                {
                    this.SamlSecurityTokenRequirement.ValidateAudienceRestriction(allowedAudienceUris, restriction.Audiences);
                }
            }
        }

        private IList<Uri> GetAllowedAudienceUris()
        {
            var result = new List<Uri>(); 
            foreach (var k in db.Signups) 
            
            { result.Add(
                    new Uri("spn:" + k.ServicePrincipalURL)); 
            } 
            
            return result;
        }
    }
}
