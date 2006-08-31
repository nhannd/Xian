using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class SplitPane
	{
        private IApplicationComponent _component;
        private SplitComponentContainer.SplitPaneHost _host;
		private string _name;

        /// <summary>
        /// Default constructor.
        /// </summary>
		public SplitPane(string name, IApplicationComponent component)
        {
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
        public SplitComponentContainer.SplitPaneHost ComponentHost
        {
            get { return _host; }
            set { _host = value; }
        }
	}
}
