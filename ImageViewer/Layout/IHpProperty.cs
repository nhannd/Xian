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
	/// <summary>
	/// Defines the interface to a single HP "property", displayed in one of the HP editor property tables.
	/// </summary>
	interface IHpProperty
	{
		/// <summary>
		/// Gets the display name for the property - the name that appears in the property table.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Gets the description for the property - the description appears in the property table to aid the user's understanding.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the value of the property as a string, for display in the property table.
		/// </summary>
		/// <returns></returns>
		string GetStringValue();

		/// <summary>
		/// Sets the value of the property from a string, assuming this property supports string parsing.
		/// <see cref="CanParseStringValue"/>
		/// </summary>
		/// <param name="value"></param>
		void SetStringValue(string value);

		/// <summary>
		/// Gets a value indicating whether this property supports string parsing.
		/// </summary>
		bool CanParseStringValue { get; }

		/// <summary>
		/// Gets a valud indicating whether this property can be edited by a custom dialog box.
		/// </summary>
		bool HasEditor { get; }

		/// <summary>
		/// Gets the application component that provides editing of this property.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetEditorComponent();
	}
}
