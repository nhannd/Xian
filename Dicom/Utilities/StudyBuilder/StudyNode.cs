using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a study-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class StudyNode : StudyBuilderNode, ICloneable
	{
		private readonly SeriesNodeCollection _series;
		private string _uid;
		private string _description;
		private DateTime? _dateTime;
		private string _accessionNum;
		private string _studyId;

		/// <summary>
		/// Constructs a new <see cref="StudyNode"/> using default values.
		/// </summary>
		public StudyNode()
		{
			_series = new SeriesNodeCollection(this);
			_studyId = string.Format("ST{0}", this.Key);
			_description = "Untitled Study";
			_dateTime = System.DateTime.Now;
			_accessionNum = "";
			RegenerateUids();
		}

		/// <summary>
		/// Constructs a new <see cref="StudyNode"/> using the specified study ID and default values for everything else.
		/// </summary>
		/// <param name="studyId">The desired study ID.</param>
		public StudyNode(string studyId)
		{
			_series = new SeriesNodeCollection(this);
			_studyId = studyId;
			_description = "";
			_dateTime = System.DateTime.Now;
			_accessionNum = "";
			RegenerateUids();
		}

		/// <summary>
		/// Constructs a new <see cref="StudyNode"/> using actual values from attributes in the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dicomDataSet">The data set from which to initialize this node.</param>
		public StudyNode(DicomAttributeCollection dicomDataSet)
		{
			_series = new SeriesNodeCollection(this);
			_studyId = dicomDataSet[DicomTags.StudyId].GetString(0, "");
			_description = dicomDataSet[DicomTags.StudyDescription].GetString(0, "");
			_dateTime = DicomConverter.GetDateTime(dicomDataSet[DicomTags.StudyDate].GetDateTime(0), dicomDataSet[DicomTags.StudyTime].GetDateTime(0));
			_accessionNum = dicomDataSet[DicomTags.AccessionNumber].GetString(0, "");
			_uid = dicomDataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			if (_uid == "")
				RegenerateUids();
		}

		#region Data Properties

		/// <summary>
		/// Gets or sets the study instance UID.
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
		/// Gets or sets the study ID.
		/// </summary>
		public string StudyId
		{
			get { return _studyId; }
			set
			{
				if(_studyId != value)
				{
					_studyId = value;
					FirePropertyChanged("StudyId");
				}
			}
		}

		/// <summary>
		/// Gets or sets the study description.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set
			{
				if(_description != value)
				{
					_description = value;
					FirePropertyChanged("Description");
				}
			}
		}

		/// <summary>
		/// Gets or sets the study date/time stamp.
		/// </summary>
		public DateTime? DateTime
		{
			get { return _dateTime; }
			set
			{
				if(_dateTime != value)
				{
					_dateTime = value;
					FirePropertyChanged("DateTime");
				}
			}
		}

		/// <summary>
		/// Gets or sets the accession number.
		/// </summary>
		public string AccessionNumber
		{
			get { return _accessionNum; }
			set
			{
				if(_accessionNum != value)
				{
					_accessionNum = value;
					FirePropertyChanged("AccessionNumber");
				}
			}
		}

		#endregion

		#region Update Methods

		/// <summary>
		/// Writes the data in this node into the given <see cref="DicomAttributeCollection"/>
		/// </summary>
		/// <param name="dicomDataSet">The data set to write data into.</param>
		internal void Update(DicomAttributeCollection dicomDataSet)
		{
			dicomDataSet[DicomTags.StudyId].SetStringValue(_studyId);
			dicomDataSet[DicomTags.StudyDescription].SetStringValue(_description);
			dicomDataSet[DicomTags.StudyDate].SetDateTime(0, _dateTime);
			dicomDataSet[DicomTags.StudyTime].SetDateTime(0, _dateTime);
			dicomDataSet[DicomTags.AccessionNumber].SetStringValue(_accessionNum);
			dicomDataSet[DicomTags.StudyInstanceUid].SetStringValue(_uid);
		}

		/// <summary>
		/// Forces the regeneration of all UIDs owned at the study-level.
		/// </summary>
		public void RegenerateUids()
		{
			this.Uid = DicomUid.GenerateUid().UID;
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
		public StudyNode Clone(bool deepCopy)
		{
			StudyNode node = new StudyNode();
			node._description = this._description;
			node._dateTime = this._dateTime;
			node._accessionNum = this._accessionNum;
			node._studyId = this._studyId;
			node._uid = this._uid;
			if (deepCopy)
			{
				foreach (SeriesNode series in _series)
				{
					node.Series.Add(series.Clone(true));
				}
			}
			return node;
		}

		/// <summary>
		/// Performs a shallow copy of the node.
		/// </summary>
		/// <returns>A shallow copy of the node.</returns>
		public StudyNode Clone()
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
		/// Convenience method to insert SOP instance-level data nodes into the study builder tree under this study, creating a <see cref="SeriesNode">series</see> node if necessary.
		/// </summary>
		/// <param name="sopInstances">An array of <see cref="SopInstanceNode"/>s to insert into the study builder tree.</param>
		public void InsertSopInstance(SopInstanceNode[] sopInstances)
		{
			SeriesNode series = new SeriesNode();
			this.Series.Add(series);
			foreach (SopInstanceNode node in sopInstances)
			{
				series.Images.Add(node);
			}
		}

		/// <summary>
		/// Convenience method to insert series-level data nodes into the study builder tree under this study.
		/// </summary>
		/// <param name="series">An array of <see cref="SeriesNode"/>s to insert into the study builder tree.</param>
		public void InsertSeries(SeriesNode[] series)
		{
			foreach (SeriesNode node in series)
			{
				this.Series.Add(node);
			}
		}

		#endregion

		#region Series Collection

		/// <summary>
		/// Gets a list of all the <see cref="SeriesNode"/>s that belong to this study.
		/// </summary>
		public SeriesNodeCollection Series
		{
			get { return _series; }
		}

		#endregion
	}
}