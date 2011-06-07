#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

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
		private TProperty _value;
		private Converter<TProperty, string> _formatter;
		private Converter<string, TProperty> _parser;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		public HpProperty(string name, string description)
		{
			DisplayName = name;
			Description = description;

			// default formatter
			Formatter = ((TProperty value) => (string)Convert.ChangeType(value, typeof(string)));

			if (typeof(IConvertible).IsAssignableFrom(typeof(TProperty)))
			{
				// specify a default parser, which will work for simple convertible types
				Parser = ((string value) => (TProperty)Convert.ChangeType(value, typeof(TProperty)));
			}
		}

		/// <summary>
		/// Gets or sets a function that formats this property for display as a string value in the user-interface.
		/// </summary>
		public Converter<TProperty, string> Formatter
		{
			get { return _formatter; }
			set { _formatter = value; }
		}

		/// <summary>
		/// Gets or sets a function that parses this property from a string value.
		/// </summary>
		public Converter<string, TProperty> Parser
		{
			get { return _parser; }
			set
			{
				_parser = value;
				CanParseStringValue = (_parser != null);
			}
		}

		/// <summary>
		/// Gets or sets the value of this property.
		/// </summary>
		public TProperty Value
		{
			get { return _value; }
			set { _value = value; }
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
			return Formatter(_value);
		}

		/// <summary>
		/// Sets the value of this property from a string representation, if this property supports parsing.
		/// </summary>
		/// <param name="value"></param>
		public void SetStringValue(string value)
		{
			if (CanParseStringValue)
			{
				_value = Parser(value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether string parsing is supported.
		/// </summary>
		public bool CanParseStringValue { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this property can be edited by a custom dialog box.
		/// </summary>
		public virtual bool HasEditor
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the application component that provides editing of this property, if <see cref="IHpProperty.HasEditor"/> returns true.
		/// </summary>
		/// <returns></returns>
		public virtual IApplicationComponent GetEditorComponent()
		{
			return null;
		}

		#endregion
	}
}
