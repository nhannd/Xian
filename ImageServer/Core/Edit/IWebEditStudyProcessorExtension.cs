#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Defines the interface of a extension to <see cref="StudyEditor"/>
	/// </summary>
	public interface IWebEditStudyProcessorExtension : IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether the extension is enabled.
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// Initializes the extension.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Called when study is about to be updated.
		/// </summary>
		/// <param name="context"></param>
		void OnStudyEditing(WebEditStudyContext context);

		/// <summary>
		/// Called after the study has been updated.
		/// </summary>
		/// <param name="context"></param>
		void OnStudyEdited(WebEditStudyContext context);
	}

	public class WebEditStudyProcessorExtensionPoint:ExtensionPoint<IWebEditStudyProcessorExtension>
	{}
}