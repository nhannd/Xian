#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Color Map Manager, which is responsible for managing installation and restoration
	/// of color maps via the Memento pattern.
	/// </summary>
	public sealed class ColorMapManager : IColorMapManager
	{
		#region Private Fields

		private readonly IColorMapInstaller _colorMapInstaller;
		
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ColorMapManager(IColorMapInstaller colorMapInstaller)
		{
			Platform.CheckForNullReference(colorMapInstaller, "colorMapInstaller");
			_colorMapInstaller = colorMapInstaller;
		}

		#region IColorMapManager Members

		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		public IColorMap GetColorMap()
		{
			return _colorMapInstaller.ColorMap;
		}

		#endregion

		#region IColorMapInstaller Members

		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		public IColorMap ColorMap
		{
			get { return _colorMapInstaller.ColorMap; }
		}

		/// <summary>
		/// Installs a color map by name.
		/// </summary>
		public void InstallColorMap(string name)
		{
			_colorMapInstaller.InstallColorMap(name);
		}

		/// <summary>
		/// Installs a color map by <see cref="ColorMapDescriptor">descriptor</see>.
		/// </summary>
		public void InstallColorMap(ColorMapDescriptor descriptor)
		{
			_colorMapInstaller.InstallColorMap(descriptor);
		}

		/// <summary>
		/// Installs a color map.
		/// </summary>
		public void InstallColorMap(IColorMap colorMap)
		{
			_colorMapInstaller.InstallColorMap(colorMap);
		}

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s for all the different types of available color maps.
		/// </summary>
		public IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				return _colorMapInstaller.AvailableColorMaps;
			}
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures enough information to restore the currently installed color map.
		/// </summary>
		public object CreateMemento()
		{
			return new ColorMapMemento(_colorMapInstaller.ColorMap);
		}

		/// <summary>
		/// Restores the previously installed color map and/or it's state.
		/// </summary>
		public void SetMemento(object memento)
		{
			ColorMapMemento colorMapMemento = (ColorMapMemento) memento;

			if (_colorMapInstaller.ColorMap != colorMapMemento.Originator)
				_colorMapInstaller.InstallColorMap(colorMapMemento.Originator);

			if (colorMapMemento.InnerMemento != null)
				_colorMapInstaller.ColorMap.SetMemento(colorMapMemento.InnerMemento);
		}

		#endregion
	}
}
