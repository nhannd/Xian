using System.ComponentModel;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// The abstract base class for nodes within the study builder tree represented by the <see cref="StudyBuilder"/> class.
	/// </summary>
	/// <remarks>
	/// This class should not be (and cannot be) inherited directly. To add a node to the study builder tree, instantiate a node at the desired tree level instead.
	/// </remarks>
	/// <see cref="PatientNode"/>
	/// <see cref="StudyNode"/>
	/// <see cref="SeriesNode"/>
	/// <see cref="SopInstanceNode"/>
	/// <see cref="StudyBuilder"/>
	public abstract class StudyBuilderNode
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private StudyBuilderNode _parent = null;
		private readonly string _key;

		internal StudyBuilderNode()
		{
			_key = serialGener.GetNext().ToString().PadLeft(8, '0');
		}

		/// <summary>
		/// Fires the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property on this node that changed.</param>
		protected virtual void FirePropertyChanged(string propertyName)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Raised when a property on this node has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Internally used key to uniquely identify the node.
		/// </summary>
		internal string Key
		{
			get { return _key; }
		}

		/// <summary>
		/// Gets the parent of this node, or null if the node is not in a study builder tree.
		/// </summary>
		public StudyBuilderNode Parent
		{
			get { return _parent; }
			internal set { _parent = value; }
		}

		private static readonly SerialGenerator serialGener = new SerialGenerator();

		/// <summary>
		/// A unique serial number generator for generating unique keys
		/// </summary>
		private class SerialGenerator
		{
			private volatile int _next;

			public int GetNext()
			{
				lock (this)
				{
					return _next++;
				}
			}
		}
	}
}