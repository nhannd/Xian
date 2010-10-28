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
	/// A <see cref="SplitPane"/> hosts a single <see cref="IApplicationComponent"/> in one
	/// side of a <see cref="SplitComponentContainer"/>.
	/// </summary>
	public class SplitPane
	{
        private IApplicationComponent _component;
		private string _name;
        private float _weight;
		private bool _fixed;

		private ApplicationComponentHost _host;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the <see cref="SplitPane"/>.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted.</param>
		/// <param name="fix">Whether or not the pane should be fixed (based on size).  Only one of the two <see cref="SplitPane"/>s can be fixed.</param>
		public SplitPane(string name, IApplicationComponent component, bool fix)
		{
			_name = name;
			_component = component;
			_weight = 0F;
			_fixed = fix;
		}

		/// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="name">The name of the <see cref="SplitPane"/>.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted.</param>
		/// <param name="initialWeight">The initial weighting factor for determing the <see cref="SplitPane"/>'s initial size.</param>
		public SplitPane(string name, IApplicationComponent component, float initialWeight)
        {
            _name = name;
            _component = component;
            _weight = initialWeight;
			_fixed = false;
        }

		/// <summary>
		/// Gets the name of the <see cref="SplitPane"/>.
		/// </summary>
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

		/// <summary>
		/// Gets whether or not this pane should be 'fixed', based on its size.
		/// </summary>
		public bool Fixed
		{
			get { return _fixed; }
		}

        /// <summary>
        /// Gets the component host for this pane.
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
