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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class XmlReplaceValue : Task
	{
		private string _xPath;
		private string _value;
		private string _file;

		private bool _nodeMustExist;

		public bool NodeMustExist
		{
			get { return _nodeMustExist; }
			set { _nodeMustExist = value; }
		}
	
		[Required]
		public string XPath
		{
			get { return _xPath; }
			set { _xPath = value; }
		}

		[Required]
		public string File
		{
			get { return _file; }
			set { _file = value; }
		}

		[Required]
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public override bool Execute()
		{
			_value = _value ?? "";

			if (String.IsNullOrEmpty(_file))
			{
				base.Log.LogMessage("Invalid input file: {0}", _file);
				return false;
			}

			if (!System.IO.File.Exists(_file))
			{
				base.Log.LogMessage("File does not exist: {0}", _file);
				return false;
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(File);

			XmlNodeList nodes = doc.SelectNodes(XPath);
			if (nodes != null && nodes.Count == 1)
			{
				XmlNode node = nodes[0];
				if (node is XmlElement)
				{
					node.InnerText = _value;
					base.Log.LogMessage("Replaced element value '{0}' with '{1}'", _xPath, _value);
				}
				else if (node is XmlAttribute)
				{
					node.Value = _value;
					base.Log.LogMessage("Replaced attribute value '{0}' with '{1}'", _xPath, _value);
				}

				doc.Save(File);
				return true;
			}
			else if (nodes == null || nodes.Count == 0)
			{
				base.Log.LogMessage("Xml node(s) not found: {0}", _xPath);
			}
			else
			{
				base.Log.LogMessage("More than one xml node found: {0}", _xPath);
			}

			if (NodeMustExist)
				return false;
			else
				return true;
		}
	}
}
