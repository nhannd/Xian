#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="TabComponentContainer"/>.
    /// </summary>
    public class TabPage : ContainerPage
    {
		private string _name;
		
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in this page.</param>
        public TabPage(string name, IApplicationComponent component)
            :base(component)
        {
			_name = name;
        }

		/// <summary>
		/// Creates a tab page for the specified component, using the last segment of the supplied path as the name.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="component"></param>
    	public TabPage(Path path, IApplicationComponent component)
			:this(path.LastSegment.LocalizedText, component)
    	{
    	}

		/// <summary>
		/// Gets the name of the page.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>
		/// Gets the <see cref="Name"/> property.
		/// </summary>
		public override string ToString()
		{
			return this.Name;
		}
    }
}
