//-------------------------------------------------------------------------------------------------
// <copyright file="RestAPIInterfaces.cs" company="Microsoft">
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
// This provides the primary Connection implementation in to the Graph API. Notice that it implements the IConnection and IFactoryConnection 
// to give you the option of using it with the ConnectionFactory defined. This class demonstrates certain methods and changes to the REST payload 
// that must exist in all calls to the REST API. In particular, the AddHeaders() method is important for you to understand as this is 
// required for the REST API to accept the calls.
//     
// </summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Microsoft.Online.Demos.Aadexpense.Helpers;
using Microsoft.Online.Demos.Aadexpense.ServiceReference1;
using Microsoft.Online.DirectoryApi.TokenHelper;

namespace Microsoft.Online.Demos.Aadexpense.Models
{
    public class RestApiInterface : IDisposable, IFactoryConnection, IConnection
    {
        private readonly Microsoft.Online.Demos.Aadexpense.ServiceReference1.DirectoryDataService dataService;
        private Uri _connectionUri;
        private string _tenantContextId;
        private string _protectedResourcePrincipalId;
        private string _protectedResourceHostName;
        private string _graphHostName;
        private string _appPrincipalId;
        private string _stsUrl;
        private string _symmetricKey;
        private string _domain = "", _strClaimType;

        private string assertion;
        private DateTime tokenExpirationTime = DateTime.Now.ToUniversalTime();
        public  List<User> CurrentDisplayedList { get; set; } 
        public DataServiceQueryContinuation ContinuationToken { get; set; }
        public int CurrentPage { get; set; }
        public DemoDatabase _db;
        private IClaimsIdentity _claimsIdentity;

        public static Dictionary<int, Uri> pageMap = new Dictionary<int, Uri>();
        public int defaultPagesize = 20;
        // Track whether Dispose has been called.
        private bool disposed;
        private const string FactoryClassName = "RESTAPI";

        #region Singleton Pattern

        private static volatile RestApiInterface m_instance;
        private static readonly object m_syncRoot = new object();

        public static RestApiInterface Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_syncRoot)
                    {
                        if (m_instance == null)
                        {
                            m_instance = new RestApiInterface();
                        }
                    }
                }

                return m_instance;
            }
        }

        #endregion
        
        private RestApiInterface()
        {
            _db = new DemoDatabase();

            ReadConfigurations();
            dataService = new Microsoft.Online.Demos.Aadexpense.ServiceReference1.DirectoryDataService(_connectionUri)
                              {IgnoreResourceNotFoundException = true};
            AddHeaders();
        }

        private void Initializer()
        {
            _claimsIdentity = ((IClaimsPrincipal)(Thread.CurrentPrincipal)).Identities[0];
            
            // We get the domain from the Claim. This is what we use to match against the domain returned in queries to the Graph API
            // to ensure multipe tenants can use the application.
            //
            foreach (var c in _claimsIdentity.Claims)
            {
                _strClaimType = c.ClaimType;
                if (_strClaimType.EndsWith("domain"))
                    _domain = c.Value;
            }
        }

        /// <summary>
        /// Add headers required for REST API calls
        /// </summary>   
        private void AddHeaders()
        {
            dataService.SendingRequest += delegate(object sender1, SendingRequestEventArgs args)
            {
                GetToken();
                ((HttpWebRequest)args.Request).Headers.Add("Authorization", assertion);
                ((HttpWebRequest)args.Request).Headers.Add("x-ms-dirapi-data-contract-version", "0.5");
            };
        }

        private void GetToken()
        {
            try
            {
                if ((tokenExpirationTime - DateTime.Now.ToUniversalTime() < new TimeSpan(0, 2, 0)) ||
                    assertion == null)
                {
                    tokenExpirationTime = DateTime.Now.ToUniversalTime().AddHours(1);
                    
                    var webToken = new JsonWebToken(
                        _appPrincipalId,
                        _tenantContextId.ToString(CultureInfo.InvariantCulture),
                        (new Uri(_stsUrl)).DnsSafeHost,
                        JWTTokenHelper.AcsPrincipalId,
                        DateTime.Now.ToUniversalTime(),
                        60 * 60);

                    // webToken.NameIdentifier = string.Format("{0}@{1}", appPrincipalId, tenantContextId);


                   // You can get ACS token using Asymmetric Key as well. Here would be the implementation.
                   // X509Certificate2 clientCertificate = new X509Certificate2(clientCertificateFilePath, clientCertificatePassword, X509KeyStorageFlags.Exportable);
                   // assertion = JWTTokenHelper.GenerateAccessToken(webToken, clientCertificate);

                    // Get ACS token using symmetricKey
                    assertion = JWTTokenHelper.GenerateAssertion(webToken, _symmetricKey);

                    string resource = String.Format("{0}/{1}@{2}", _protectedResourcePrincipalId, _protectedResourceHostName, _tenantContextId);
                    assertion = JWTTokenHelper.GetOAuthAccessTokenFromACS(_stsUrl, assertion, resource);
                }
            }
            catch (WebException webExc)
            {
                if (webExc.Response != null)
                {
                    using (Stream responseStream = webExc.Response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(responseStream);
                        string responseMessage = sr.ReadToEnd();
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Populate the constants from the database or web.config
        /// </summary>
        private void ReadConfigurations()
        {

            Initializer();
            var companyID = from r in _db.Signups
                         where r.DomainName == _domain
                         select r.CompanyId;


            var appPrincipalID = from r in _db.Signups
                                 where r.DomainName == _domain
                                 select r.AppPrincipalId;

            _tenantContextId = companyID.First().ToString(CultureInfo.InvariantCulture);
            _protectedResourcePrincipalId = ConfigurationManager.AppSettings["ProtectedResourcePrincipalId"];
            _protectedResourceHostName = ConfigurationManager.AppSettings["ProtectedResourceHostName"];
            _graphHostName = ConfigurationManager.AppSettings["GraphHostName"];
            _connectionUri = new Uri(string.Format(@"https://{0}/{1}", _graphHostName, _tenantContextId));
            _appPrincipalId = appPrincipalID.First().ToString(CultureInfo.InvariantCulture);
            _stsUrl = ConfigurationManager.AppSettings["StsUrl"];
            _symmetricKey = ConfigurationManager.AppSettings["SymmetricKey"];
        }

        /// <summary>
        /// Get all the users
        /// </summary>
        /// <returns>List of users</returns>
        public  List<User> GetAllUsers()
        {
            InvokeOperationWithRetry(() =>
            {
                var users = dataService.Users.AddQueryOption("$top", defaultPagesize);
                var userQuery = users.Execute();
                CurrentDisplayedList = userQuery.ToList();
                ContinuationToken = ((QueryOperationResponse)userQuery).GetContinuation();
                CurrentPage = 1;
                pageMap[1] = ((QueryOperationResponse)userQuery).Query.RequestUri;

                if (ContinuationToken != null)
                {
                    pageMap[2] = new Uri(string.Format("{0}&$top={1}", ContinuationToken.NextLinkUri, defaultPagesize));
                }
            });

            return CurrentDisplayedList;
        }

        /// <summary>
        /// Get the next page of results.
        /// </summary>
        /// <returns>List of users</returns>
        public  List<User> GetNextPage()
        {
            if (pageMap[CurrentPage + 1] != null)
            {
                var userQuery = dataService.Execute<User>(pageMap[++CurrentPage]);
                CurrentDisplayedList = userQuery.ToList<User>();
                ContinuationToken = ((QueryOperationResponse)userQuery).GetContinuation();

                if (ContinuationToken != null)
                {
                    pageMap[CurrentPage + 1] = new Uri(string.Format("{0}&$top={1}", ContinuationToken.NextLinkUri, defaultPagesize));
                }
            }

            return CurrentDisplayedList;
        }

        public  List<User> GetPrevPage()
        {
            if (pageMap[CurrentPage - 1] != null)
            {   var userQuery = dataService.Execute<User>(pageMap[--CurrentPage]);
                CurrentDisplayedList = userQuery.ToList<User>();
            }

            return CurrentDisplayedList;
        }

        public  User GetUser(Guid objectId)
        {
            string detailsUserQuery = string.Format("{0}('{1}')", "Users", objectId);
            var user = dataService.CreateQuery<User>(detailsUserQuery).ToList<User>()[0];
            return user;
        }

        public User GetUserByEmail(string emailAddress)
        {
            string detailsUserQuery = string.Format("{0}('{1}')", "Users", emailAddress);
            var user = dataService.CreateQuery<User>(detailsUserQuery).ToList<User>()[0];
            return user;
        }



        public List<ReferencedObject> GetLinks(Guid objectId, string linkName)
        {
            string query = string.Format("{0}/{1}('{2}')/{3}", _connectionUri, "Users", objectId, linkName);
            var linkQuery = dataService.Execute<ReferencedObject>(new Uri(query));
            return linkQuery.ToList();
        }

        /// <summary>
        /// Execute a dataservice query
        /// </summary>
        /// <param name="query">Request query</param>
        /// <returns>List of users.</returns>
        public  List<User> ExecuteQuery(string query)
        {
            InvokeOperationWithRetry(() =>
            {
                var userQueryUri = new Uri(string.Format("{0}/Users?$filter=DisplayName eq '{1}'", _connectionUri.ToString(), query));
                var userQuery = dataService.Execute<User>(userQueryUri);
                CurrentDisplayedList = userQuery.ToList();
                ContinuationToken = ((QueryOperationResponse)userQuery).GetContinuation();
            });

            return CurrentDisplayedList;
        }


        public bool IsManager(Guid objectID)
        {
            User user = this.GetUser(objectID);
            List<ReferencedObject> directReports = this.GetLinks(objectID, "DirectReports");

            if (directReports.Any()) return true;

            return false;

        }

        public bool IsAdministrator(Guid objectId)
        {
            var roleQuery = dataService.Roles.Execute();
                List<Role> restReturned = roleQuery.ToList();
            Role adminRole =
                    restReturned.First(role => role.DisplayName == "Company Administrator");
            string detailsUserQuery = string.Format("{0}('{1}')/Members", "Roles", adminRole.ObjectId);
             List<ReferencedObject> restRoleMembers = dataService.CreateQuery<ReferencedObject>(detailsUserQuery).ToList<ReferencedObject>();
                List<User> adminUsers = new List<User>();
            return restRoleMembers.Any(restRolemember => restRolemember != null && restRolemember.ObjectId == objectId);
        }
        
        /// <summary>
        /// Get all the unlicensed users.
        /// </summary>
        /// <returns></returns>
        
        
        public  List<User> GetAdministrators()
        {
            InvokeOperationWithRetry(() =>
            {
                var roleQuery = dataService.Roles.Execute();
                List<Role> restReturned = roleQuery.ToList();
                Role adminRole =
                    restReturned.First(role => role.DisplayName == "Company Administrator");
                string detailsUserQuery = string.Format("{0}('{1}')/Members", "Roles", adminRole.ObjectId);
                List<ReferencedObject> restRoleMembers = dataService.CreateQuery<ReferencedObject>(detailsUserQuery).ToList<ReferencedObject>();
                List<User> adminUsers = restRoleMembers.Select(restRolemember => GetUser(restRolemember.ObjectId.GetValueOrDefault())).ToList();

                CurrentDisplayedList = adminUsers;
            });

            return CurrentDisplayedList;
        }


        public List<User> GetBlockedUsers()
        {
            const string query = "Users?$filter=AccountEnabled eq false";
            CurrentDisplayedList = ExecuteQuery(query);
            return CurrentDisplayedList;
        }

        /// <summary>
        /// Delegate for invoking networking operations with retry.
        /// </summary>
        /// <param name="operation"></param>
        private void InvokeOperationWithRetry(Action operation)
        {
            try
            {
                operation();
            }
            catch (Exception ex)
            {
                ParsedException parsedException = DataServiceExceptionUtil.ParseException(ex);
                if (parsedException == null)
                {
                    throw;
                }
                else
                {
                    switch (parsedException.Code)
                    {
                        case "Authentication_ExpiredToken":
                        case "Authentication_Unauthorized":
                        case "Authentication_Unknown":
                        case "Authentication_UnsupportedTokenType":
                        case "Headers_DataContractVersionMissing":
                        case "Headers_HeaderNotSupported":
                        case "Service_InternalServerError":
                        case "Directory_CompanyNotFound":
                        case "Request_ThrorttledPermanently":
                        case "Unsupported_Query":
                        case "":
                            {
                                throw parsedException;
                            }

                        case "Directory_BindingRedirection": break;
                        case "Directory_ReplicaUnavailable": break;
                        case "Request_InvalidReplicaSessionKey": break;
                        case "Request_ThrorttledTemporarily": break;
                    }
                }
            }
        }


        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);

        }


        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _db.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.


                // Note disposing has been done.
                disposed = true;

            }
        }

        // These methods are used to hook in to the ConnectionFactory if desired
        // Returns the instance of the object vs. creating a new object per the Singleton pattern.

        public RestApiInterface aConnection()
        {
            return RestApiInterface.Instance;
        }

        // This method will register the name of the class to the ConnectionFactory
        public object GetFactoryKey()
        {
            return FactoryClassName;
        }
    }
}
