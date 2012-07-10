//-------------------------------------------------------------------------------------------------
// <copyright file="Base64Utils.cs" company="Microsoft">
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
//     Helper methods for encoding and decoding strings to base64.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Online.DirectoryApi.TokenHelper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helper method for encoding and decoding string to and from Base64
    /// </summary>
    public static class Base64Utils
    {
        /// <summary>
        /// Padding character
        /// </summary>
        private const char Base64PadCharacter = '=';

        /// <summary>
        /// Url encoding: + becomes -
        /// </summary>
        private const char Base64Character62 = '+';
        
        /// <summary>
        /// Url encoding: / becomes _
        /// </summary>
        private const char Base64Character63 = '/';

        /// <summary>
        /// Url encoding: + becomes -
        /// </summary>
        private const char Base64UrlCharacter62 = '-';

        /// <summary>
        /// Url encoding: / becomes _
        /// </summary>
        private const char Base64UrlCharacter63 = '_';

        /// <summary>
        /// Encode the string to base64, with url encoding
        /// </summary>
        /// <param name="arg">Input string.</param>
        /// <returns>Encoded string.</returns>
        public static string Encode(string arg)
        {
            return Base64Utils.Encode(Encoding.UTF8.GetBytes(arg));
        }

        /// <summary>
        /// Encodes the byte array
        /// </summary>
        /// <param name="arg">Input byte array.</param>
        /// <returns>Encoded string.</returns>
        public static string Encode(byte[] arg)
        {
            string text = System.Convert.ToBase64String(arg);
            text = text.Split(
                new char[] { Base64Utils.Base64PadCharacter })[0];

            text = text.Replace(Base64Utils.Base64Character62, Base64Utils.Base64UrlCharacter62);
            return text.Replace(Base64Utils.Base64Character63, Base64Utils.Base64UrlCharacter63);
        }

        /// <summary>
        /// Decode the string array
        /// </summary>
        /// <param name="arg">Base 64 encoded string, without padding.</param>
        /// <returns>Decoded byte array.</returns>
        public static byte[] DecodeBytes(string arg)
        {
            string text = arg;
            text = text.Replace(Base64Utils.Base64UrlCharacter62, Base64Utils.Base64Character62);
            text = text.Replace(Base64Utils.Base64UrlCharacter63, Base64Utils.Base64Character63);

            // In Base64 encoding, 3 octets are converted into 4 encoded characters
            // 6 bits are used to generate 1 ASCII character
            // If the number of octets is divisible by 3, then no padding is required
            // If not, enough padding is added
            // Before decoding, we need to event out the input to be divisible by 4
            // If the last group contained only 0 bytes, no padding is required
            // If the last group contains 1 or 2 bytes, a padding of == or = is added
            // The length of the last group can be inferred from the length % 4 of the input string.
            int numPadCharacters = 4 - (text.Length % 4);
            if (numPadCharacters == 3)
            {
                throw new ArgumentException("Illegal base64url string!", arg);
            }

            text += new string(Base64Utils.Base64PadCharacter, numPadCharacters);
            return Convert.FromBase64String(text);            
        }

        /// <summary>
        /// Decode the string array.
        /// </summary>
        /// <param name="arg">Input string.</param>
        /// <returns>Decoded string.</returns>
        public static string Decode(string arg)
        {
            return Encoding.UTF8.GetString(Base64Utils.DecodeBytes(arg));
        }
    }
}
