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
    public class TabPage : ContainerPage
    {
		private string _name;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public TabPage(string name, IApplicationComponent component)
            :base(component)
        {
			_name = name;
        }

		public string Name
		{
			get { return _name; }
		}
		
		public override string ToString()
		{
			return this.Name;
		}
    }
}
