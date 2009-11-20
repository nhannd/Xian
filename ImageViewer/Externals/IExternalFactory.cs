#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Externals
{
	[ExtensionPoint]
	public sealed class ExternalFactoryExtensionPoint : ExtensionPoint<IExternalFactory> {}

	public interface IExternalFactory
	{
		string Description { get; }
		Type ExternalType { get; }
		IExternal CreateNew();
		IExternalPropertiesComponent CreatePropertiesComponent();
	}

	public abstract class ExternalFactoryBase<T> : IExternalFactory where T : IExternal, new()
	{
		private readonly string _description;

		protected ExternalFactoryBase(string description)
		{
			_description = description;
		}

		public string Description
		{
			get { return _description; }
		}

		public Type ExternalType
		{
			get { return typeof (T); }
		}

		public IExternal CreateNew()
		{
			T t = new T();
			t.Enabled = true;
			t.Name = t.Label = SR.StringNewExternal;
			t.WindowStyle = WindowStyle.Normal;
			return t;
		}

		public abstract IExternalPropertiesComponent CreatePropertiesComponent();
	}
}