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
using System.IO;
using System.Reflection;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Provides convenience mean to load a javascript template from embedded resource.
    /// </summary>
    internal class ScriptTemplate
    {
        #region Private Members
        private String _script;

        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ScriptTemplate"/>
        /// </summary>
        /// <param name="assembly">The assembly which contains the embedded javascript template</param>
        /// <param name="name">Fully-qualified name of the javascript template (including the namespace)</param>
        /// <remarks>
        /// </remarks>
        public ScriptTemplate(Assembly assembly, string name)
        {
            Stream stream = assembly.GetManifestResourceStream(name);
            StreamReader reader = new StreamReader(stream);
            _script = reader.ReadToEnd();
            stream.Close();
            reader.Dispose();
        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        public string Script
        {
            get { return _script; }
        }

        #endregion Public properties

        #region Public Methods

        /// <summary>
        /// Replaces a token in the script with the specified value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Replace(string key, string value)
        {
            _script = _script.Replace(key, value);
        }

        #endregion Public Methods
    }
}
