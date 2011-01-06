#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides a description of a color map.
	/// </summary>
	/// <seealso cref="IColorMapFactory"/>
	public sealed class ColorMapDescriptor
	{
		#region Private Fields

		private readonly string _name;
		private readonly string _description;

		#endregion

		#region Private Constructor

		private ColorMapDescriptor(string name, string description)
		{
			_name = name;
			_description = description;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the name of the factory.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a <see cref="ColorMapDescriptor"/> given an input <see cref="IColorMapFactory"/>.
		/// </summary>
		public static ColorMapDescriptor FromFactory(IColorMapFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			return new ColorMapDescriptor(factory.Name, factory.Description);
		}

		#endregion
	}
}
