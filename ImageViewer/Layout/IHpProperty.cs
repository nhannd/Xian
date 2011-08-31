#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout
{
    public interface IHpPropertyEditContext
    {
		/// <summary>
		/// Launches the specified editor component as a modal dialog.
		/// </summary>
		/// <param name="editorComponent"></param>
		/// <returns></returns>
        ApplicationComponentExitCode ShowModalEditor(IApplicationComponent editorComponent);
    }

	/// <summary>
	/// Defines the interface to a single HP "property", displayed in one of the HP editor property tables.
	/// </summary>
	public interface IHpProperty
	{
        /// <summary>
        /// The type of the underlying property.
        /// </summary>
        Type Type { get; }
        
        /// <summary>
		/// Gets the display name of this property for display in the user-interface.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Gets the description of this property for display in the user-interface.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the category of this property for display in the user-interface.
		/// </summary>
		string Category { get; }

		/// <summary>
		/// Gets the type converter for this property, or null if this property does not require a custom converter.
		/// </summary>
		TypeConverter Converter { get; }

        /// <summary>
        /// Gets whether or not <see cref="Value"/> can be set.
        /// </summary>
        bool CanSetValue { get; }
        
        /// <summary>
        /// Gets the value for this property.
        /// </summary>
        object Value { get; set; }

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
