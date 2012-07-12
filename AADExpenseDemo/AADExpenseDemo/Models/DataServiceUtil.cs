//-------------------------------------------------------------------------------------------------
// <copyright file="DataServiceUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
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
//     Helper methods in the model that allow for accessing the Graph API in a RESTful manner
// </summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.Online.Demos.Aadexpense.Models
{
    /// <summary>
    /// Exception type which represents the DataServiceException thrown by the ADO.NET Data Service
    /// </summary>
    [Serializable]
    public class ParsedException : Exception
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public bool Retryable { get; set; }
        public bool RetryAfter { get; set; }
        public bool InnerErrorExists { get; set; }
        public string InnerErrorCode { get; set; }
        public string InnerErrorMessage { get; set; }
    }

    /// <summary>
    /// Helper class to de-serialize DataServiceExceptions thrown by an ADO.NET Data Service
    /// </summary>
    public static class DataServiceExceptionUtil
    {
        #region Variables

        private const string DataServicesMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        static readonly XName xnCode = XName.Get("code", DataServicesMetadataNamespace);
        static readonly XName xnMessage = XName.Get("message", DataServicesMetadataNamespace);
        static readonly XName xnInnerError = XName.Get("innererror", DataServicesMetadataNamespace);
        #endregion

        public static ParsedException ParseException(Exception ex)
        {
            Exception innerException = ex.InnerException;
            ParsedException parsedException = null;

            if (innerException != null)
            {
                try
                {
                    XDocument xDoc = XDocument.Parse(innerException.Message);
                    parsedException = new ParsedException();
                    
                    var xElement = xDoc.Root.Element(xnMessage);
                    if (xElement != null)
                        parsedException.Message = xDoc.Root != null && xElement != null ?
                                                                                                                xElement.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;

                    var element = xDoc.Root.Element(xnCode);
                    if (element != null)
                        parsedException.Code = element != null ?
                                                                                     element.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;

                    if (xDoc.Root.Element(xnInnerError) != null)
                    {
                        XElement innerError = xDoc.Root.Element(xnInnerError);
                        parsedException.InnerErrorExists = true;
                        var xElement1 = innerError.Element(xnMessage);
                        if (xElement1 != null)
                            parsedException.InnerErrorMessage = innerError != null && xElement1 != null ?
                                                                                                                                xElement1.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;

                        var element1 = innerError.Element(xnCode);
                        if (element1 != null)
                            parsedException.InnerErrorCode = element1 != null ?
                                                                                                    element1.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
                    }
                }
                catch
                {
                    throw new Exception("Generic DataService Exception", ex);

                }
            }

            return parsedException;
        }
    }
}