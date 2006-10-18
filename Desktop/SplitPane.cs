using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class SplitPane
	{
        private IApplicationComponent _component;
		private string _name;
        private float _weight;
		private bool _fixed;

        private SplitComponentContainer.SplitPaneHost _host;

		public SplitPane(string name, IApplicationComponent component, bool fix)
		{
			_name = name;
			_component = component;
			_weight = 0F;
			_fixed = fix;
		}

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="component">Component that the pane will host</param>
        /// <param name="initialWeight">Initial weight of the pane, relative to other panes</param>
        public SplitPane(string name, IApplicationComponent component, float initialWeight)
        {
            _name = name;
            _component = component;
            _weight = initialWeight;
			_fixed = false;
        }

		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>
        /// Gets the component that is displayed on this pane.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets the weight that was assigned to this pane
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

		public bool Fixed
		{
			get { return _fixed; }
		}

        /// <summary>
        /// Gets the component host for this pane.  For internal use only.
        /// </summary>
        public SplitComponentContainer.SplitPaneHost ComponentHost
        {
            get { return _host; }
            set { _host = value; }
        }
	}
}
