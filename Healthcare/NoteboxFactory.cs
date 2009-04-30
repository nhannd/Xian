#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Manages instantiation of noteboxs.
    /// </summary>
    public class NoteboxFactory
    {
        #region Static members

        private static readonly NoteboxFactory _theInstance = new NoteboxFactory();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NoteboxFactory Instance
        {
            get { return _theInstance; }
        }

        #endregion

        private readonly List<Type> _noteboxClasses;

        private NoteboxFactory()
        {
            _noteboxClasses = CollectionUtils.Map<ExtensionInfo, Type>(new NoteboxExtensionPoint().ListExtensions(),
                delegate(ExtensionInfo info) { return info.ExtensionClass; });
        }

        /// <summary>
        /// Creates an instance of the notebox class as specified by the class name, which may be fully or
        /// only partially qualified.
        /// </summary>
        /// <param name="noteboxClassName"></param>
        /// <returns></returns>
        public Notebox CreateNotebox(string noteboxClassName)
        {
            return CreateNotebox(ResolvePartialClassName(noteboxClassName));
        }

        /// <summary>
        /// Creates an instance of the specified notebox class.
        /// </summary>
        /// <param name="noteboxClass"></param>
        /// <returns></returns>
        public Notebox CreateNotebox(Type noteboxClass)
        {
            return (Notebox)Activator.CreateInstance(noteboxClass);
        }


        private Type ResolvePartialClassName(string noteboxClassName)
        {
            Type noteboxClass = CollectionUtils.SelectFirst(_noteboxClasses,
                delegate(Type t) { return t.FullName.Contains(noteboxClassName); });

            if(noteboxClass == null)
                throw new ArgumentException(string.Format("{0} is not a valid notebox class name.", noteboxClassName));

            return noteboxClass;
        }
    }
}