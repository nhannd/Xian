using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class TabGroup
    {
        private float _weight;
        private TabComponentContainer _tabContainer;

        private TabGroupComponentContainer.TabGroupHost _host;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialWeight">Initial weight of the tab group, relative to other tab groups</param>
        public TabGroup(TabComponentContainer tabContainer, float initialWeight)
        {
            _tabContainer = tabContainer;
            _weight = initialWeight;
        }

        /// <summary>
        /// Gets the tabComponentContainer of this tabgroup
        /// </summary>
        public TabComponentContainer Component
        {
            get { return _tabContainer; }
        }

        /// <summary>
        /// Gets the weight that was assigned to this tabgroup
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Gets the component host for this pane.  For internal use only.
        /// </summary>
        public TabGroupComponentContainer.TabGroupHost ComponentHost
        {
            get { return _host; }
            set { _host = value; }
        }
    }
}
