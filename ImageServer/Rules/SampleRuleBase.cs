#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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