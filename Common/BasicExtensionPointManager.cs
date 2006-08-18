using System;
using System.Collections.Generic;

namespace ClearCanvas.Common
{
	/// <summary>
	/// This class serves as a basic container for a collection of unknown extensions.
	/// The class itself helps to decouple the extension point interface (<see cref="IExtensionPoint"/>
	/// from the extension point class that gets used to generate the actual
	/// extensions from the plugins.  This is useful, for example, for testing the <see cref="Auditor"/>
	/// extension points/interfaces that currently have no real implementation to test.
	/// </summary>
	/// <typeparam name="TInterface">The interface that the extensions are expected to implement.</typeparam>
	public abstract class BasicExtensionPointManager<TInterface>
	{
		private List<TInterface> _listExtensions = new List<TInterface>();

		protected List<TInterface> Extensions
		{
			get { return _listExtensions; }
		}

		Type InterfaceType
		{ 
			get { return typeof(TInterface); }
		}

		protected BasicExtensionPointManager()
		{
			//TInterface can't be IExtensionPoint (can't extend and extension point).
			if (InterfaceType == typeof(IExtensionPoint))
				throw new InvalidOperationException();

			//TInterface must be (you guessed it) and interface.
			if (!InterfaceType.IsInterface)
				throw new InvalidOperationException();
		}

		protected void LoadExtensions()
		{
			if (_listExtensions.Count > 0)
				return;

			IExtensionPoint iExtensionPoint = GetExtensionPoint();
			
			object[] objects = iExtensionPoint.CreateExtensions();
			
			foreach (object currentObject in objects)
			{
				TInterface expectedInterface = (TInterface)currentObject;
				_listExtensions.Add(expectedInterface);
			}
		}

		protected abstract IExtensionPoint GetExtensionPoint();
	}
}
