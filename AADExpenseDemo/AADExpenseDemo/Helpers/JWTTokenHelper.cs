//-------------------------------------------------------------------------------------------------
// <copyright file="JWTTokenHelper.cs" company="Microsoft">
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
//  Facilitates minting a test token.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Online.DirectoryApi.TokenHelper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;


    /// <summary>
    /// Helper class for minting and requesting JWT tokens
    /// </summary>
    public class JWTTokenHelper
    {
        /// <summary>
        /// Gets the well known principal id of ACS.
        /// Each ACS instance can have a unique id.
        /// </summary>
        public const string AcsPrincipalId = "00000001-0001-0000-c000-000000000000";

        /// <summary>
        /// Grant type claim
        /// </summary>
        private const string ClaimTypeGrantType = "grant_type";

        /// <summary>
        /// Assertion claim
        /// </summary>
        private const string ClaimTypeAssertion = "assertion";

        /// <summary>
        /// Resource claim
        /// </summary>
        private const string ClaimTypeResource = "resource";

        /// <summary>
        /// Prefix for bearer tokens.
        /// </summary>
        private const string BearerTokenPrefix = "Bearer ";

        /// <summary>
        /// Generates a JWT token for the given tenant, scope
        /// </summary>
        /// <param name="issuerPrincipalId">Issuer App principal id.</param>
        /// <param name="tenantRealm">Tenant context id.</param>
        /// <param name="audienceHostName">Service host name</param>
        /// <param name="audiencePrincipalId">Principal id of the protected resource or audience</param>
        /// <param name="nbfTime">Not valid before time.</param>
        /// <param name="validityInSeconds">Token validity duration.</param>
        /// <returns>A JWT token initialized with the basic claims.</returns>
        public static JsonWebToken GenerateSelfSignedToken(
            string issuerPrincipalId,
            string tenantRealm,
            string audienceHostName,
            string audiencePrincipalId,
            DateTime nbfTime,
            long validityInSeconds)
        {
            string issuer = JWTTokenHelper.GetFormattedPrincipal(issuerPrincipalId, string.Empty, tenantRealm);
            string audience = JWTTokenHelper.GetFormattedPrincipal(audiencePrincipalId, audienceHostName, tenantRealm);
            DateTime expirationTime = DateTime.Now.ToUniversalTime().AddSeconds(validityInSeconds);
            return new JsonWebToken(issuer, audience, nbfTime, expirationTime);
        }







        /// <summary>
        /// Generates a self-signed assertion.
        /// </summary>
        /// <param name="webToken">Json web token.</param>
        /// <param name="signingCert">Signing certificate.</param>
        /// <returns>Self signed assertion.</returns>
        public static string GenerateAssertion(
            JsonWebToken webToken, X509Certificate2 signingCert)
        {
            string encodedHash = Base64Utils.Encode(signingCert.GetCertHash());

            TokenHeader tokenHeaderContract = new TokenHeader("RS256", encodedHash);

            string tokenHeader = Base64Utils.Encode(tokenHeaderContract.EncodeToJson());
            string tokenBody = Base64Utils.Encode(webToken.EncodeToJson());
            string rawToken = string.Format("{0}.{1}", tokenHeader, tokenBody);
            string hash = Base64Utils.Encode(JWTTokenHelper.SignData(signingCert, rawToken));

            string accessToken = string.Format(
                "{0}.{1}",
                rawToken,
                hash);

            return accessToken;
        }

        /// <summary>
        /// Generate access token with a symmetric signing key.
        /// </summary>
        /// <param name="webToken">JSON web token.</param>
        /// <param name="signingKey">Symmetric signing key.</param>
        /// <returns>Self signed assertion.</returns>
        public static string GenerateAssertion(JsonWebToken webToken, string signingKey)
        {
            TokenHeader tokenHeaderContract = new TokenHeader("HS256", String.Empty);

            string tokenHeader = Base64Utils.Encode(tokenHeaderContract.EncodeToJson());
            string tokenBody = Base64Utils.Encode(webToken.EncodeToJson());
            string rawToken = string.Format("{0}.{1}", tokenHeader, tokenBody);

            string signature = Base64Utils.Encode(JWTTokenHelper.SignData(signingKey, rawToken));

            string accessToken = string.Format(
                "{0}.{1}",
                rawToken,
                signature);

            return accessToken;
        }

        /// <summary>
        /// Sign the text with the symmetric key.
        /// </summary>
        /// <param name="signingKey">Signing key.</param>
        /// <param name="data">Text to be signed.</param>
        /// <returns>Signed byte array.</returns>
        public static byte[] SignData(string signingKey, string data)
        {
            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(signingKey));
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sign the data with the X509Certificate
        /// </summary>
        /// <param name="signingCertificate">Signing certificate.</param>
        /// <param name="data">Data to be signed.</param>
        /// <returns>RSA SHA 256 Signature</returns>
        public static byte[] SignData(X509Certificate2 signingCertificate, string data)
        {
            X509AsymmetricSecurityKey securityKey = new X509AsymmetricSecurityKey(signingCertificate);

            RSACryptoServiceProvider rsa =
                securityKey.GetAsymmetricAlgorithm("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", true)
                as RSACryptoServiceProvider;

            if (!signingCertificate.HasPrivateKey)
            {
                throw new ArgumentException(string.Format(
                    "Private key is not found in the certificate: {0}",
                    signingCertificate.Subject));
            }

            if (rsa != null)
            {
                rsa.FromXmlString(signingCertificate.PrivateKey.ToXmlString(true));

                if (rsa.CspKeyContainerInfo.ProviderType != 24)
                {
                    System.Security.Cryptography.CspParameters cspParameters =
                        new System.Security.Cryptography.CspParameters
                            {
                                ProviderType = 24,
                                KeyContainerName = rsa.CspKeyContainerInfo.KeyContainerName,
                                KeyNumber = (int) rsa.CspKeyContainerInfo.KeyNumber
                            };

                    if (rsa.CspKeyContainerInfo.MachineKeyStore)
                    {
                        cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                    }

                    rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspParameters);
                }
            }

            HashAlgorithm hashAlgo = System.Security.Cryptography.SHA256.Create();
            byte[] signatureInBytes = rsa.SignData(Encoding.UTF8.GetBytes(data), hashAlgo);

            return signatureInBytes;
        }

        /// <summary>
        /// Get an access token from ACS (STS)
        /// </summary>
        /// <param name="stsUrl">ACS STS Url.</param>
        /// <param name="assertion">Assertion token.</param>
        /// <param name="resource">ExpiresIn name.</param>
        /// <returns>The OAuth access token.</returns>
        public static string GetOAuthAccessTokenFromACS(string stsUrl, string assertion, string resource)
        {
            string accessToken = string.Empty;

            WebClient client = new WebClient();
            NameValueCollection values = new NameValueCollection
                                             {
                                                 {
                                                     JWTTokenHelper.ClaimTypeGrantType,
                                                     "http://oauth.net/grant_type/jwt/1.0/bearer"
                                                     },
                                                 {JWTTokenHelper.ClaimTypeAssertion, assertion},
                                                 {JWTTokenHelper.ClaimTypeResource, resource}
                                             };

            byte[] responseBytes = client.UploadValues(stsUrl, "POST", values);

            MemoryStream ms = new MemoryStream(responseBytes);
            ms.Seek(0, SeekOrigin.Begin);

            DataContractJsonSerializer jsonSerializer = 
                new DataContractJsonSerializer(typeof(OAuthAccessTokenResponseContract));
            OAuthAccessTokenResponseContract response = 
                (OAuthAccessTokenResponseContract) jsonSerializer.ReadObject(ms);

            accessToken = response.AccessToken;

            return String.Format("{0}{1}", JWTTokenHelper.BearerTokenPrefix, accessToken);
        }

        /// <summary>
        /// Get the formatted SPN.
        /// </summary>
        /// <param name="principalName">Principal Id</param>
        /// <param name="hostName">Service host name</param>
        /// <param name="realm">Tenant realm.</param>
        /// <returns>Formatted SPN.</returns>
        public static string GetFormattedPrincipal(string principalName, string hostName, string realm)
        {
            if (!String.IsNullOrEmpty(hostName) && !String.IsNullOrEmpty(realm))
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}/{1}@{2}", principalName, hostName, realm);
            }
            else if (String.IsNullOrEmpty(realm))
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}/{1}", principalName, hostName);
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}@{1}", principalName, realm);
            }
        }

        #region Not for sample app

        /// <summary>
        /// Generates a serialized access token using the web token that is passed.
        /// Bearer {Header}.{Body}.Thumbprint
        /// All the 3 components are Base64 encoded individually.
        /// Since a lot of cryptography is performed here, this function will be CPU intensive
        /// </summary>
        /// <param name="webToken">Json Web Security Token</param>
        /// <param name="signingCert">Signing certificate.</param>
        /// <returns>OAuth bearer token (self-signed).</returns>
        public static string GenerateAccessToken(
            JsonWebToken webToken, X509Certificate2 signingCert)
        {
            return String.Format(
                "{0}{1}",
                JWTTokenHelper.BearerTokenPrefix,
                JWTTokenHelper.GenerateAssertion(webToken, signingCert));
        }

        /// <summary>
        /// Generate access token with a symmetric signing key.
        /// </summary>
        /// <param name="webToken">JSON web token.</param>
        /// <param name="signingKey">Symmetric signing key.</param>
        /// <returns>OAuth bearer token (self signed)</returns>
        public static string GenerateAccessToken(JsonWebToken webToken, string signingKey)
        {
            return String.Format(
                "{0}{1}",
                JWTTokenHelper.BearerTokenPrefix,
                JWTTokenHelper.GenerateAssertion(webToken, signingKey));
        }

        #endregion
    }
}
