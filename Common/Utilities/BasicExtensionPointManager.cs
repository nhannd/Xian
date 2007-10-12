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

namespace ClearCanvas.Common.Utilities
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

		private Type InterfaceType
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
