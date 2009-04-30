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
using System.Text;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines the class of data that an extension of <see cref="XmlDataImexExtensionPoint"/> is
    /// responsible for.
    /// </summary>
    /// <remarks>
    /// The data-class is a logical class name and need not refer to an actual entity class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class ImexDataClassAttribute : Attribute
    {
        private readonly string _dataClass;
        private int _itemsPerFile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataClass"></param>
        public ImexDataClassAttribute(string dataClass)
        {
            _dataClass = dataClass;
        }

        /// <summary>
        /// Gets the name of the data class.
        /// </summary>
        public string DataClass
        {
            get { return _dataClass; }
        }

        /// <summary>
        /// Gets or sets the default number of items to export per file.
        /// </summary>
        public int ItemsPerFile
        {
            get { return _itemsPerFile; }
            set { _itemsPerFile = value; }
        }
    }
}
