#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark an assembly as being a ClearCanvas Plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PluginAttribute : Attribute
    {
        private string _name;
        private string _description;
    	private string _icon;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PluginAttribute()
		{
		}
		
    	/// <summary>
        /// A friendly name for the plugin.  
        /// </summary>
        /// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

		/// <summary>
		/// A friendly description for the plugin.  
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

		/// <summary>
		/// The name of an icon resource to associate with the plugin.
		/// </summary>
    	public string Icon
    	{
			get { return _icon; }
			set { _icon = value; }
    	}
    }
}
