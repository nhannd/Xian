#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A <see cref="TabGroup"/> to be hosted within a <see cref="TabGroupComponentContainer"/>.
	/// </summary>
    public class TabGroup
    {
        private float _weight;
        private TabComponentContainer _tabContainer;

        private ApplicationComponentHost _host;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tabContainer">The owning container.</param>
        /// <param name="initialWeight">Initial weight of the tab group, relative to other tab groups.</param>
        public TabGroup(TabComponentContainer tabContainer, float initialWeight)
        {
            _tabContainer = tabContainer;
            _weight = initialWeight;
        }

        /// <summary>
        /// Gets the owner <see cref="TabComponentContainer"/>.
        /// </summary>
        public TabComponentContainer Component
        {
            get { return _tabContainer; }
        }

        /// <summary>
        /// Gets the weight assigned to this group, relative to the other groups.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Gets or sets the component host for this pane.
        /// </summary>
        /// <remarks>
		/// For internal framework use only.
		/// </remarks>
        public ApplicationComponentHost ComponentHost
        {
            get { return _host; }
            internal set { _host = value; }
        }
    }
}
