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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Base class for server rule samples that read samples from the embedded resource.
    /// </summary>
    public class SampleRuleBase : ISampleRule
    {
        #region Private members

        private readonly IList<ServerRuleApplyTimeEnum> _applyTime = new List<ServerRuleApplyTimeEnum>();
        private readonly string _embeddedXmlName;
        private string _description;
        private string _name;
        private ServerRuleTypeEnum _type;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SampleRuleBase"/>
        /// </summary>
        /// <param name="name">Name of the sample rule</param>
        /// <param name="description">Description of the sample rule</param>
        /// <param name="type">Type of the sample rule</param>
        /// <param name="embeddedXmlName">Name of the resource file containing the sample rule xml</param>
        public SampleRuleBase(string name, string description, ServerRuleTypeEnum type, string embeddedXmlName)
        {
            _name = name;
            _description = description;
            _type = type;
            _embeddedXmlName = embeddedXmlName;
        }

        #endregion

        #region ISampleRule Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ServerRuleTypeEnum Type
        {
            get { return _type; }
            set { _type = value; }
        }


        public XmlDocument Rule
        {
            get
            {
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), _embeddedXmlName);
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                stream.Close();
                return doc;
            }
        }

        public IList<ServerRuleApplyTimeEnum> ApplyTimeList
        {
            get { return _applyTime; }
        }

        #endregion
    }
}