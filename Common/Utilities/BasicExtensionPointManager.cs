#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// This class serves as a basic container for a collection of unknown extensions.
	/// </summary>
	/// <remarks>
	/// The class itself helps to decouple the extension point interface (<see cref="IExtensionPoint"/>)
	/// from the extension point class that is used to generate the actual extensions from the plugins.
	/// </remarks>
	/// <typeparam name="TInterface">The interface that the extensions are expected to implement.</typeparam>
	public abstract class BasicExtensionPointManager<TInterface>
	{
		private readonly List<TInterface> _listExtensions;
		private bool _loaded;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected BasicExtensionPointManager()
		{
			//TInterface can't be IExtensionPoint (can't extend and extension point).
			if (InterfaceType == typeof(IExtensionPoint))
				throw new InvalidOperationException(SR.MessageCannotExtendIExtensionPoint);

			//TInterface must be (you guessed it) an interface.
			if (!InterfaceType.IsInterface)
				throw new InvalidOperationException(String.Format(SR.FormatTypeMustBeInterface, InterfaceType.FullName));

			_listExtensions = new List<TInterface>();
			_loaded = false;
		}

		private static Type InterfaceType
		{
			get { return typeof(TInterface); }
		}

		/// <summary>
		/// Gets the internal list of extensions.
		/// </summary>
		/// <remarks>
		/// When <see cref="LoadExtensions"/> is called is left up to the derived class.  This property does
		/// <b>not</b> call <see cref="LoadExtensions"/> automatically.
		/// </remarks>
		protected IEnumerable<TInterface> Extensions
		{
			get { return _listExtensions; }	
		}

		/// <summary>
		/// Loads the extensions.
		/// </summary>
		/// <remarks>
		/// Repeated calls to this method will do nothing.
		/// </remarks>
		protected void LoadExtensions()
		{
			if (_loaded)
				return;

			_loaded = true;

			IExtensionPoint iExtensionPoint = GetExtensionPoint();
			if (iExtensionPoint == null)
				return;

			object[] objects = iExtensionPoint.CreateExtensions();
			
			foreach (object currentObject in objects)
			{
				if (currentObject is TInterface)
					_listExtensions.Add((TInterface)currentObject);
				else
					Platform.Log(LogLevel.Warn, String.Format(SR.FormatTypeMustImplementInterface, currentObject.GetType().FullName, InterfaceType.FullName));
			}
		}

		/// <summary>
		/// Inheritors must implement this method and return a valid <see cref="IExtensionPoint"/>-derived object.
		/// </summary>
		protected abstract IExtensionPoint GetExtensionPoint();
	}
}
