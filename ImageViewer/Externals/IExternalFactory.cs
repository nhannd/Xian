#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Externals
{
	[ExtensionPoint]
	public sealed class ExternalFactoryExtensionPoint : ExtensionPoint<IExternalFactory>
	{
		public IExternalFactory CreateExtension(string externalType)
		{
			var extensions = CreateExtensions();
			var result = (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType.FullName == externalType);
			result = result ?? (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType.Name == externalType);
			result = result ?? (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType.AssemblyQualifiedName == externalType);
			return result;
		}

		public IExternalFactory CreateExtension(Type externalType)
		{
			var extensions = CreateExtensions();
			return (IExternalFactory) CollectionUtils.SelectFirst(extensions, x => ((IExternalFactory) x).ExternalType == externalType);
		}
	}

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