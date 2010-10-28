#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Defines the interface of a compiler to generate 
	/// <see cref="BaseImageLevelUpdateCommand"/> from a XML specification.
	/// </summary>
	public interface IWebEditStudyCommandCompiler
	{
		/// <summary>
		/// Name of the command to be generated
		/// </summary>
		string CommandName { get; }

		/// <summary>
		/// Generates the <see cref="BaseImageLevelUpdateCommand"/>
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		BaseImageLevelUpdateCommand Compile(XmlReader reader);
	}
}