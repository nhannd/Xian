#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
    /// <summary>
    /// The default implementation of <see cref="IExtensionFactory"/> that creates extensions from
    /// the set of plugins discovered at runtime.
    /// </summary>
    /// <remarks>
    /// This class is safe for use by mutliple concurrent threads.
    /// </remarks>
    internal class DefaultExtensionFactory : IExtensionFactory
    {
        internal DefaultExtensionFactory()
        {
        }

        #region IExtensionFactory Members

        /// <summary>
        /// Creates one of each type of object that extends the input <paramref name="extensionPoint" />, 
        /// matching the input <paramref name="filter" />; creates a single extension if <paramref name="justOne"/> is true.
        /// </summary>
        /// <param name="extensionPoint">The <see cref="ExtensionPoint"/> to create extensions for.</param>
        /// <param name="filter">The filter used to match each extension that is discovered.</param>
        /// <param name="justOne">Indicates whether or not to return only the first matching extension that is found.</param>
        /// <returns></returns>
        public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            // get subset of applicable extensions
            ExtensionInfo[] extensions = ListExtensions(extensionPoint, filter);

            // attempt to instantiate the extension classes
            List<object> createdObjects = new List<object>();
            foreach (ExtensionInfo extension in extensions)
            {
                if (justOne && createdObjects.Count > 0)
                    break;

                // is the extension a concrete class?
                if (!IsConcreteClass(extension.ExtensionClass))
                {
                    Platform.Log(LogLevel.Warn, SR.ExceptionExtensionMustBeConcreteClass,
                        extension.ExtensionClass.FullName);
                    continue;
                }

                // does the extension implement the required interface?
                if (!extensionPoint.InterfaceType.IsAssignableFrom(extension.ExtensionClass))
                {
                    Platform.Log(LogLevel.Warn, SR.ExceptionExtensionDoesNotImplementRequiredInterface,
                        extension.ExtensionClass.FullName,
                        extensionPoint.InterfaceType);

                    continue;
                }

                try
                {
                    // instantiate
                    object o = Activator.CreateInstance(extension.ExtensionClass);
                    createdObjects.Add(o);
                }
                catch (Exception e)
                {
                    // instantiation failed
					// this should not be considered an exceptional circumstance
					// instantiation may fail by design in some cases (e.g extension is designed only to run on a particular platform)
					Platform.Log(LogLevel.Debug, e);
                }
            }

            return createdObjects.ToArray();
        }

        /// <summary>
        /// Gets metadata describing all extensions of the input <paramref name="extensionPoint"/>, 
        /// matching the given <paramref name="filter"/>.
        /// </summary>
        /// <param name="extensionPoint">The <see cref="ExtensionPoint"/> whose extension metadata is to be retrieved.</param>
        /// <param name="filter">An <see cref="ExtensionFilter"/> used to filter out extensions with particular characteristics.</param>
        /// <returns></returns>
        public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
            Type extensionPointClass = extensionPoint.GetType();

            // assume that Platform.PluginManager.Extensions is a thread-safe property, 
            // therefore no need to lock here
			return CollectionUtils.Select(Platform.PluginManager.Extensions,
				delegate(ExtensionInfo extension)
				{
					return extension.PointExtended == extensionPointClass
							&& extension.Enabled
							&& (filter == null || filter.Test(extension));
				}).ToArray();
        }

        private static bool IsConcreteClass(Type type)
        {
            return !type.IsAbstract && type.IsClass;
        }

        #endregion
    }
}
