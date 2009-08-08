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
using System.Xml;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Helper class to compile an XML specification into a list of <see cref="BaseImageLevelUpdateCommand"/>
	/// to be used by the <see cref="StudyEditor"/>
	/// </summary>
	public class WebEditStudyCommandCompiler
	{
		private readonly Dictionary<string, IWebEditStudyCommandCompiler> _commandCompilers = new Dictionary<string, IWebEditStudyCommandCompiler>();

		public WebEditStudyCommandCompiler()
		{
			LoadCommandCompilers();
		}

		private void LoadCommandCompilers()
		{
			// TODO: Make this plugin
			SetTagCommandCompiler compiler = new SetTagCommandCompiler();
			_commandCompilers.Add(compiler.CommandName, compiler);
		}

		public List<BaseImageLevelUpdateCommand> Compile(XmlNode node)
		{
			List<BaseImageLevelUpdateCommand> commands = new List<BaseImageLevelUpdateCommand>();

			foreach (XmlNode subNode in node.ChildNodes)
			{
				IList<BaseImageLevelUpdateCommand> list = Compile(new XmlNodeReader(subNode));
				if (list != null && list.Count > 0)
					commands.AddRange(list);
			}

			return commands;
		}

		public List<BaseImageLevelUpdateCommand> Compile(XmlReader reader)
		{
			List<BaseImageLevelUpdateCommand> commands = new List<BaseImageLevelUpdateCommand>();

			if (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						{
							if (_commandCompilers.ContainsKey(reader.Name))
							{
								IWebEditStudyCommandCompiler plugin = _commandCompilers[reader.Name];
								commands.Add(plugin.Compile(reader));
							}
							else
							{
								throw new NotSupportedException(reader.Name);
							}
						}
						break;
				}
			}


			return commands;
		}
	}

	/// <summary>
	/// Compile an XML node into a <see cref="SetTagCommand"/>.
	/// </summary>
	internal class SetTagCommandCompiler : IWebEditStudyCommandCompiler
	{
		#region IWebEditStudyCommandCompiler Members

		public string CommandName
		{
			get { return "SetTag"; }
		}


		#endregion

		#region IWebEditStudyCommandCompiler Members

		#endregion

		/// <summary>
		/// Compiles an XML specification into a <see cref="SetTagCommand"/> object.
		/// </summary>
		/// <param name="reader">Reference to a <see cref="XmlReader"/> to read the Xml node.</param>
		/// <returns>An instance of <see cref="SetTagCommand"/></returns>
		/// <remarks>
		/// The <cref="XmlReader"/> must be positioned at an Xml node named "SetTag".
		/// </remarks>
		public BaseImageLevelUpdateCommand Compile(XmlReader reader)
		{
			SetTagCommand command = XmlUtils.Deserialize<SetTagCommand>(reader);
			return command;
		}
	}
}