//-----------------------------------------------------------------------
// <copyright file="MoneyModelBinder.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.

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
// Custom binding for the Currency Type in order to parse money values correctly.
//
// </summary>
//----------------------------------------------------------------------------------------------

using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Online.Demos.Aadexpense.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


public class MoneyModelBinder : IModelBinder
{

    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        var modelState = new ModelState { Value = valueResult };

        decimal actualValue = 0;
        try
        {

            if (bindingContext.ModelMetadata.DataTypeName == DataType.Currency.ToString())
                decimal.TryParse(valueResult.AttemptedValue, NumberStyles.Currency, null, out actualValue);
            else
                actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.CurrentCulture);


        }
        catch (FormatException e)
        {
            modelState.Errors.Add(e);
        }

        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
        return actualValue;
    }
}
