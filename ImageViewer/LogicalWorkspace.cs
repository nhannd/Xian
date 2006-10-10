using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// Describes a logical workspace.
	/// </summary>
	public class LogicalWorkspace : ILogicalWorkspace
	{
		private DisplaySetCollection _displaySets = new DisplaySetCollection();
		private List<IDisplaySet> _linkedDisplaySets = new List<IDisplaySet>();
        private IImageViewer _imageViewer;

        internal LogicalWorkspace(IImageViewer imageViewer)
		{
            _imageViewer = imageViewer;
			_displaySets.ItemAdded += new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			_displaySets.ItemRemoved += new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);
		}

		/// <summary>
        /// Gets the parent <see cref="ImageViewerComponent"/>
		/// </summary>
        public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Gets a collection of display sets.
		/// </summary>
		public DisplaySetCollection DisplaySets
		{
			get { return _displaySets; }
		}

		/// <summary>
		/// Gets a collection of linked <see cref="DisplaySets"/>
		/// </summary>
		/// <value>A collection of linked <see cref="DisplaySets"/></value>
		public ReadOnlyCollection<IDisplaySet> LinkedDisplaySets
		{
			get { return _linkedDisplaySets.AsReadOnly(); }
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeDisplaySets();
			}
		}

		private void DisposeDisplaySets()
		{
			if (this.DisplaySets == null)
				return;

			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Dispose();

			_displaySets.ItemAdded -= new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			_displaySets.ItemRemoved -= new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);
			_displaySets = null;
		}

		public void Draw()
		{
			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Draw();
		}

		internal void LinkDisplaySet(DisplaySet displaySet)
		{
			_linkedDisplaySets.Add(displaySet);
		}

		internal void UnlinkDisplaySet(DisplaySet displaySet)
		{
			_linkedDisplaySets.Remove(displaySet);
		}

		private void OnDisplaySetAdded(object sender, DisplaySetEventArgs e)
		{
			DisplaySet displaySet = e.DisplaySet as DisplaySet;

			displaySet.ParentLogicalWorkspace = this;
			displaySet.ImageViewer = this.ImageViewer;

			if (e.DisplaySet.Linked)
				_linkedDisplaySets.Add(e.DisplaySet);
		}

		private void OnDisplaySetRemoved(object sender, DisplaySetEventArgs e)
		{
			if (e.DisplaySet.Linked)
				_linkedDisplaySets.Remove(e.DisplaySet);
		}
	}
}
