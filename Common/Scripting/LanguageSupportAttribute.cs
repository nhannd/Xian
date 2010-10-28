#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// Used to specify that a class (for example, an <see cref="IScriptEngine"/>) 
	/// supports a certain scripting language.
	/// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class LanguageSupportAttribute : Attribute
    {
        private string _language;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="language">A string describing the language.</param>
		public LanguageSupportAttribute(string language)
        {
            _language = language;
        }

		/// <summary>
		/// Gets a string describing the language.
		/// </summary>
        public string Language
        {
            get { return _language; }
        }

		/// <summary>
		/// Determines whether or not this instance is the same as <paramref name="obj"/>, which is itself an <see cref="Attribute"/>.
		/// </summary>
		public override bool Match(object obj)
        {
            LanguageSupportAttribute that = obj as LanguageSupportAttribute;
            return that != null && that.Language.ToLower().Equals(this.Language.ToLower());
        }
    }
}
