using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="TabComponent"/>.
    /// </summary>
    public class TabPage
    {
        private IApplicationComponent _component;
        private TabComponentContainer.PageHost _host;
		private string _name;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public TabPage(string name, IApplicationComponent component)
        {
			Platform.CheckForNullReference(component, "component");

			_name = name;
            _component = component;
        }

		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>
        /// Gets the component that is displayed on this page.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets the component host for this page.  For internal use only.
        /// </summary>
        public TabComponentContainer.PageHost ComponentHost
        {
            get { return _host; }
            set 
			{
				Platform.CheckForNullReference(value, "ComponentHost");
				_host = value; 
			}
        }

		public override string ToString()
		{
			return this.Name;
		}
    }
}
