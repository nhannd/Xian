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
	public class LogicalWorkspace : IDrawable
	{
		private DisplaySetCollection _displaySets = new DisplaySetCollection();
		private List<DisplaySet> _linkedDisplaySets = new List<DisplaySet>();
		private ImageWorkspace _parentWorkspace;

		internal LogicalWorkspace(ImageWorkspace parentWorkspace)
		{
			_parentWorkspace = parentWorkspace;
			_displaySets.ItemAdded += new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			_displaySets.ItemRemoved += new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);
		}

		/// <summary>
		/// Gets the parent <see cref="ImageWorkspace"/>
		/// </summary>
		public ImageWorkspace ParentWorkspace
		{
			get { return _parentWorkspace; }
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
		public ReadOnlyCollection<DisplaySet> LinkedDisplaySets
		{
			get { return _linkedDisplaySets.AsReadOnly(); }
		}

		#region IDrawable

		public void Draw(bool paintNow)
		{
			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Draw(paintNow);
		}

		#endregion

		private void OnDisplaySetAdded(object sender, DisplaySetEventArgs e)
		{
			e.DisplaySet.ParentLogicalWorkspace = this;
			e.DisplaySet.LinkageChanged += new EventHandler<LinkageChangedEventArgs>(OnDisplaySetLinkageChanged);

			if (e.DisplaySet.Linked)
				_linkedDisplaySets.Add(e.DisplaySet);
		}

		private void OnDisplaySetRemoved(object sender, DisplaySetEventArgs e)
		{
			e.DisplaySet.LinkageChanged -= new EventHandler<LinkageChangedEventArgs>(OnDisplaySetLinkageChanged);

			if (e.DisplaySet.Linked)
				_linkedDisplaySets.Remove(e.DisplaySet);
		}

		private void OnDisplaySetLinkageChanged(object sender, LinkageChangedEventArgs e)
		{
			DisplaySet displaySet = sender as DisplaySet;
			Platform.CheckForInvalidCast(displaySet, "sender", "DisplaySet");

			if (e.IsLinked)
				_linkedDisplaySets.Add(displaySet);
			else
				_linkedDisplaySets.Remove(displaySet);
		}
	}
}