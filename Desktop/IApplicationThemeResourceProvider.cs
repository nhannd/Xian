#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	[ExtensionPoint]
	public sealed class ApplicationThemeResourceProviderExtensionPoint : ExtensionPoint<IApplicationThemeResourceProvider> {}

	public interface IApplicationThemeResourceProvider
	{
		string Id { get; }
		string Name { get; }
		string Description { get; }
		string Icon { get; }

		IApplicationThemeColors Colors { get; }

		bool HasResource(string resourceFullName, Assembly originalAssemblyHint);
		Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint);
	}

	public interface IApplicationThemeColors
	{
		Color BasicColor { get; }
		Color BasicColorDark { get; }
		Color BasicColorLight { get; }
	}

	public abstract class ApplicationThemeResourceProviderBase : IApplicationThemeResourceProvider
	{
		private readonly IResourceResolver _resourceResolver;
		private readonly IApplicationThemeColors _colors;
		private readonly string _id;
		private readonly string _name;
		private readonly string _description;
		private readonly string _icon;

		protected ApplicationThemeResourceProviderBase(string id, string name, string description)
			: this(id, name, description, null) {}

		protected ApplicationThemeResourceProviderBase(string id, string name, string description, string icon)
		{
			Platform.CheckForEmptyString(id, @"id");
			_id = id;
			_name = !string.IsNullOrEmpty(name) ? name : id;
			_description = description ?? string.Empty;
			_icon = icon ?? string.Empty;
			_resourceResolver = new ResourceResolver(GetType(), false);
			_colors = new ApplicationThemeColors(this);
		}

		public string Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _resourceResolver.LocalizeString(_name); }
		}

		public string Description
		{
			get { return _resourceResolver.LocalizeString(_description); }
		}

		public string Icon
		{
			get { return _icon; }
		}

		public IApplicationThemeColors Colors
		{
			get { return _colors; }
		}

		protected virtual Color BasicColor
		{
			get { return Color.FromArgb(124, 177, 221); }
		}

		protected virtual Color BasicColorDark
		{
			get { return Color.FromArgb(61, 152, 209); }
		}

		protected virtual Color BasicColorLight
		{
			get { return Color.FromArgb(186, 210, 236); }
		}

		protected virtual string MapResourceName(string resourceFullName, Assembly originalAssemblyHint)
		{
			return _id + '.' + resourceFullName;
		}

		public virtual bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var resourceName = MapResourceName(resourceFullName, originalAssemblyHint);
				_resourceResolver.ResolveResource(resourceName);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public virtual Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var resourceName = MapResourceName(resourceFullName, originalAssemblyHint);
				return _resourceResolver.OpenResource(resourceName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private class ApplicationThemeColors : IApplicationThemeColors
		{
			private readonly ApplicationThemeResourceProviderBase _owner;

			public ApplicationThemeColors(ApplicationThemeResourceProviderBase owner)
			{
				_owner = owner;
			}

			public Color BasicColor
			{
				get { return _owner.BasicColor; }
			}

			public Color BasicColorDark
			{
				get { return _owner.BasicColorDark; }
			}

			public Color BasicColorLight
			{
				get { return _owner.BasicColorLight; }
			}
		}
	}
}