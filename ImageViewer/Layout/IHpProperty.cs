#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout
{
	public interface IHpPropertyEditContext
	{
		ApplicationComponentExitCode ShowModalEditor(IApplicationComponent editorComponent);
	}

	/// <summary>
	/// Defines the interface to a single HP "property", displayed in one of the HP editor property tables.
	/// </summary>
	public interface IHpProperty
	{
		/// <summary>
		/// Gets the display name of this property for display in the user-interface.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Gets the description of this property for display in the user-interface.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets string representation of this property for display in the user-interface.
		/// </summary>
		/// <returns></returns>
		string GetStringValue();

		/// <summary>
		/// Sets the value of this property from a string representation, if this property supports parsing.
		/// </summary>
		/// <param name="value"></param>
		void SetStringValue(string value);

		/// <summary>
		/// Gets a value indicating whether string parsing is supported.
		/// </summary>
		bool CanParseStringValue { get; }

		/// <summary>
		/// Gets a value indicating whether this property can be edited by a custom dialog box.
		/// </summary>
		bool HasEditor { get; }

		/// <summary>
		/// Called to invoke custom editing of this property, if <see cref="HasEditor"/> returns true. 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool EditProperty(IHpPropertyEditContext context);
	}
}
