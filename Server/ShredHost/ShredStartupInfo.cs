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
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredStartupInfo : MarshalByRefObject
    {
        public ShredStartupInfo(Uri assemblyPath, string shredName, string shredTypeName)
        {
            Platform.CheckForNullReference(assemblyPath, "assemblyPath");
            Platform.CheckForNullReference(shredName, "shredName");
            Platform.CheckForEmptyString(shredName, "shredName");
            Platform.CheckForNullReference(shredTypeName, "shredTypeName");
            Platform.CheckForEmptyString(shredTypeName, "shredTypeName");

            _assemblyPath = assemblyPath;
            _shredName = shredName;
            _shredTypeName = shredTypeName;
        }

        #region Properties
        private Uri _assemblyPath;
        private string _shredName;
        private string _shredTypeName;

        public string ShredTypeName
        {
            get { return _shredTypeName; }
        }
	

        public string ShredName
        {
            get { return _shredName; }
        }
	
        public Uri AssemblyPath
        {
            get { return _assemblyPath; }
        }
        #endregion
    }
}
