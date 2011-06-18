#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Provides a default implementation of <see cref="IHpProperty"/>.
	/// </summary>
	/// <remarks>
	/// This class can be used as-is or subclassed for advanced functionality.
	/// </remarks>
	/// <typeparam name="TProperty"></typeparam>
	public class HpProperty<TProperty> : IHpProperty
	{
		public delegate TProperty ValueGetter();
		public delegate void ValueSetter(TProperty value);

		private readonly ValueGetter _getter;
		private readonly ValueSetter _setter;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="getter"></param>
		/// <param name="setter"></param>
		public HpProperty(string name, string description, ValueGetter getter, ValueSetter setter)
		{
			DisplayName = name;
			Description = description;
			_getter = getter;
			_setter = setter;
		}

		#region Implementation of IHpProperty

		/// <summary>
		/// Gets the display name of this property for display in the user-interface.
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Gets the description of this property for display in the user-interface.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Gets string representation of this property for display in the user-interface.
		/// </summary>
		/// <returns></returns>
		public string GetStringValue()
		{
			return Format(this.Value);
		}

		/// <summary>
		/// Sets the value of this property from a string representation, if this property supports parsing.
		/// </summary>
		/// <param name="value"></param>
		public void SetStringValue(string value)
		{
			if (CanParseStringValue)
			{
				this.Value = Parse(value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether string parsing is supported.
		/// </summary>
		public virtual bool CanParseStringValue
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this property can be edited by a custom dialog box.
		/// </summary>
		public virtual bool HasEditor
		{
			get { return false; }
		}

		/// <summary>
		/// Called to invoke custom editing of this property, if <see cref="IHpProperty.HasEditor"/> returns true. 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual bool EditProperty(IHpPropertyEditContext context)
		{
			return false;
		}

		#endregion

		/// <summary>
		/// Override this method to control formatting of value for display in property user-interface.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string Format(TProperty value)
		{
		    return TypeDescriptor.GetConverter(typeof (TProperty)).ConvertToString(value);
		}

		/// <summary>
		/// Override this method to control parsing of value entered through property user-interface.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual TProperty Parse(string value)
		{
			if(!CanParseStringValue)
				throw new NotSupportedException("This property does not support string parsing.");
		    try
		    {
                return (TProperty)TypeDescriptor.GetConverter(typeof(TProperty)).ConvertFromString(value);
		    }
		    catch (Exception e)
		    {
                Platform.Log(LogLevel.Info, e);
		    }

		    return default(TProperty);
		}

		/// <summary>
		/// Gets or sets the value associated with this property.
		/// </summary>
		protected TProperty Value
		{
			get { return _getter(); }
			set { _setter(value); }
		}
	}
}
