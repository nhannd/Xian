#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A base implementation for Color Map factories.
	/// </summary>
	/// <typeparam name="T">Must be derived from <see cref="ColorMap"/> and have a parameterless default constructor.</typeparam>
	/// <seealso cref="IColorMapFactory"/>
	/// <seealso cref="ColorMap"/>
	public abstract class ColorMapFactoryBase<T> : IColorMapFactory
		where T : ColorMap, new()
	{
		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ColorMapFactoryBase()
		{
		}

		#endregion

		#region IColorMapFactory Members

		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should not be a resource string, as it should be language-independent.
		/// </remarks>
		public abstract string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Creates and returns a <see cref="ColorMap"/>.
		/// </summary>
		public IDataLut Create()
		{
			return new T();
		}

		#endregion
	}
}
