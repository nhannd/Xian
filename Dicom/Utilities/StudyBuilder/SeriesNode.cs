using System;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a series-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class SeriesNode : StudyBuilderNode, ICloneable
	{
		private readonly SopInstanceNodeCollection _images;
		private string _uid;
		private string _description;
		private DateTime? _dateTime;

		/// <summary>
		/// Constructs a new <see cref="SeriesNode"/> using default values.
		/// </summary>
		public SeriesNode()
		{
			_images = new SopInstanceNodeCollection(this);
			_description = "Untitled Series";
			_dateTime = System.DateTime.Now;
			RegenerateUids();
		}

		/// <summary>
		/// Constructs a new <see cref="SeriesNode"/> using actual values from attributes from the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dicomDataSet">The data set from which to initialize this node.</param>
		public SeriesNode(DicomAttributeCollection dicomDataSet)
		{
			_images = new SopInstanceNodeCollection(this);
			_description = dicomDataSet[DicomTags.SeriesDescription].GetString(0, "");
			_dateTime =
				DicomConverter.GetDateTime(dicomDataSet[DicomTags.SeriesDate].GetDateTime(0),
				                           dicomDataSet[DicomTags.SeriesTime].GetDateTime(0));
			_uid = dicomDataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
			if (_uid == "")
				RegenerateUids();
		}

		#region Data Properties

		/// <summary>
		/// Gets or sets the series instance UID.
		/// </summary>
		public string Uid
		{
			get { return _uid; }
			set
			{
				if (_uid != value)
				{
					_uid = value;
					FirePropertyChanged("Uid");
				}
			}
		}

		/// <summary>
		/// Gets or sets the series description.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					FirePropertyChanged("Description");
				}
			}
		}

		/// <summary>
		/// Gets or sets the series date/time stamp.
		/// </summary>
		public DateTime? DateTime
		{
			get { return _dateTime; }
			set
			{
				if (_dateTime != value)
				{
					_dateTime = value;
					FirePropertyChanged("DateTime");
				}
			}
		}

		#endregion

		#region Update Methods

		/// <summary>
		/// Writes the data in this node into the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dicomDataSet">The data set to write data into.</param>
		/// <param name="seriesNum"></param>
		internal void Update(DicomAttributeCollection dicomDataSet, int seriesNum)
		{
			dicomDataSet[DicomTags.SeriesInstanceUid].SetStringValue(_uid);
			dicomDataSet[DicomTags.SeriesDescription].SetStringValue(_description);
			dicomDataSet[DicomTags.SeriesDate].SetDateTime(0, _dateTime);
			dicomDataSet[DicomTags.SeriesTime].SetDateTime(0, _dateTime);
			dicomDataSet[DicomTags.SeriesNumber].SetInt32(0, seriesNum);
		}

		/// <summary>
		/// Forces the regeneration of all UIDs owned at the series-level.
		/// </summary>
		public void RegenerateUids()
		{
			_uid = DicomUid.GenerateUid().UID;
		}

		#endregion

		#region Cloning Methods

		/// <summary>
		/// Performs a copy of the node.
		/// </summary>
		/// <remarks>
		/// Clones of all decendant nodes are generated if the operation is a deep copy. If a shallow copy is performed, the new
		/// node is generated without child nodes.</remarks>
		/// <param name="deepCopy">True if the clone should be a deep copy; False if the clone should be a shallow copy.</param>
		/// <returns>A copy of the node.</returns>
		public SeriesNode Clone(bool deepCopy)
		{
			SeriesNode node = new SeriesNode();
			node._description = this._description;
			node._dateTime = this._dateTime;
			node._uid = this._uid;
			if (deepCopy)
			{
				foreach (SopInstanceNode sopInstance in this._images)
				{
					node._images.Add(sopInstance.Clone());
				}
			}
			return node;
		}

		/// <summary>
		/// Performs a shallow copy of the node.
		/// </summary>
		/// <returns>A shallow copy of the node.</returns>
		public SeriesNode Clone()
		{
			return this.Clone(false);
		}

		/// <summary>
		/// Performs a shallow copy of the node.
		/// </summary>
		/// <returns>A shallow copy of the node.</returns>
		object ICloneable.Clone()
		{
			return this.Clone(false);
		}

		#endregion

		#region Insert Methods

		/// <summary>
		/// Convenience method to insert SOP instance-level data nodes into the study builder tree under this series.
		/// </summary>
		/// <param name="sopInstances">An array of <see cref="SopInstanceNode"/>s to insert into the study builder tree.</param>
		public void InsertSopInstances(SopInstanceNode[] sopInstances)
		{
			foreach (SopInstanceNode node in sopInstances)
			{
				this.Images.Add(node);
			}
		}

		#endregion

		#region Images Collection

		/// <summary>
		/// Gets a list of all the <see cref="SopInstanceNode"/>s that belong to this series.
		/// </summary>
		public SopInstanceNodeCollection Images
		{
			get { return _images; }
		}

		#endregion
	}
}