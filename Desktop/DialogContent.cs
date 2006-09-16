using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class DialogContent
	{
        private IApplicationComponent _component;
        private DialogComponentContainer.DialogContentHost _host;

        /// <summary>
        /// Default constructor.
        /// </summary>
		public DialogContent(IApplicationComponent component)
        {
            _component = component;
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
        public DialogComponentContainer.DialogContentHost ComponentHost
        {
            get { return _host; }
            set { _host = value; }
        }
	}
}
