using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class DialogContent
	{
        private IApplicationComponent _component;

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
	}
}
