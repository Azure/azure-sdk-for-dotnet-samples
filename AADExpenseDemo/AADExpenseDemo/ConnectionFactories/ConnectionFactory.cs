//-----------------------------------------------------------------------
// <copyright file="ConnectionFactory.cs" company="Microsoft">
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
// This class provides several features:
// 
// 1) Implements the singleton pattern to ensure only one type of Connection is open at any one time
// 2) Using reflection, determines all possible connection classes in the code that could provide a connection through implementing IConnection
// 3) Allows for the creation of a Connection through instantiation or reuse of existing Connection.     
//
//
// </summary>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Microsoft.Online.Demos.Aadexpense.Helpers
{

    public class ConnectionFactory
    {
        #region Singleton Pattern

        private static volatile ConnectionFactory m_instance;
        private static readonly object m_syncRoot = new object();

        public static ConnectionFactory Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_syncRoot)
                    {
                        if (m_instance == null)
                        {
                            m_instance = new ConnectionFactory();
                        }
                    }
                }

                return m_instance;
            }
        }

        #endregion

        #region Constructor

        private ConnectionFactory()
        {
            // Perform variable initializations
            m_Connections = new ListDictionary();


            // Initialize the factory
            Initialize();
        }

        #endregion

        #region Variables

        // Key to type mappings for product creation
        private readonly ListDictionary m_Connections;


        // Cached reference to the containing assembly
        private Assembly m_asm;

        #endregion

        #region Methods

        public IConnection CreateConnection(object key)
        {
            if (key == null)
            {
                throw new NullReferenceException("Invalid key supplied, must be " +
                                                 "non-null.");
            }

            var type = (Type) m_Connections[key];

            if (type == null)
            {
                return null;
            }
            var inst = m_asm.CreateInstance(type.FullName, true,
                                            BindingFlags.CreateInstance,
                                            null, null, null, null);

            if (inst == null)
            {
                throw new NullReferenceException("Null product instance.  " +
                                                 "Unable to create necessary connection class.");
            }

            var connection = (IConnection) inst;

            return connection;
        }

        #endregion

        #region Helpers

        // Find and map available product classes
        private void Initialize()
        {
            // Get the assembly that contains this code
            Assembly asm = Assembly.GetCallingAssembly();

            // Get a list of all the types in the assembly
            Type[] allTypes = asm.GetTypes();
            foreach (Type type in allTypes)
            {
                // Only scan classes that aren't abstract
                if (type.IsClass && !type.IsAbstract)
                {
                    // If a class implements the IFactoryProduct interface,
                    // which allows retrieval of the product class key...
                    var iFactoryConnection = type.GetInterface("IConnection");
                    if (iFactoryConnection != null)
                    {
                        // Create a temporary instance of that class...
                        object inst = asm.CreateInstance(type.FullName, true,
                                                         BindingFlags.CreateInstance, null, null, null, null);

                        if (inst != null)
                        {
                            // And generate the product classes key
                            var keyDesc = (IFactoryConnection) inst;
                            var key = keyDesc.GetFactoryKey();
                            inst = null;

                            // Determine whether the product class implements
                            // one or more of the necessary product interfaces
                            // and add mappings for each one that's implemented
                            var prodInterface = type.GetInterface("IConnection");
                            if (prodInterface != null)
                            {
                                m_Connections.Add(key, type);
                            }

                        }
                    }
                }
            }

            m_asm = asm;
        }

        #endregion
    }
}
