using System;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a patient-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class PatientNode : StudyBuilderNode, ICloneable
	{
		private readonly StudyNodeCollection _studies;
		private string _patientId;
		private string _name;
		private DateTime? _birthdate;
		private PatientSex _sex;

		/// <summary>
		/// Constructs a new <see cref="PatientNode"/> using default values.
		/// </summary>
		public PatientNode()
		{
			_studies = new StudyNodeCollection(this);
			_patientId = string.Format("PN{0}", this.Key);
			_name = "Unnamed Patient";
			_birthdate = null;
			_sex = PatientSex.Undefined;
		}

		/// <summary>
		/// Constructs a new <see cref="PatientNode"/> using the specified patient ID and default values for everything else.
		/// </summary>
		/// <param name="patientId">The desired patient ID.</param>
		public PatientNode(string patientId) : this()
		{
			_patientId = patientId;
		}

		/// <summary>
		/// Constructs a new <see cref="PatientNode"/> using actual values from attributes in the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dicomDataSet">The data set from which to initialize this node.</param>
		public PatientNode(DicomAttributeCollection dicomDataSet)
		{
			_studies = new StudyNodeCollection(this);
			_patientId = dicomDataSet[DicomTags.PatientId].GetString(0, "");
			_name = dicomDataSet[DicomTags.PatientsName].GetString(0, "");
			_birthdate = DicomConverter.GetDateTime(dicomDataSet[DicomTags.PatientsBirthDate].GetDateTime(0), dicomDataSet[DicomTags.PatientsBirthTime].GetDateTime(0));
			_sex = DicomConverter.GetSex(dicomDataSet[DicomTags.PatientsSex].GetString(0, ""));
		}

		#region Data Properties

		/// <summary>
		/// Gets or sets the patient ID (medical record number).
		/// </summary>
		public string PatientId
		{
			get { return _patientId; }
			set
			{
				if (_patientId != value)
				{
					_patientId = value;
					FirePropertyChanged("PatientId");
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					FirePropertyChanged("Name");
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient's birthdate and time.
		/// </summary>
		public DateTime? BirthDate
		{
			get { return _birthdate; }
			set
			{
				if (_birthdate != value)
				{
					_birthdate = value;
					FirePropertyChanged("BirthDate");
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient's gender.
		/// </summary>
		public PatientSex Sex
		{
			get { return _sex; }
			set
			{
				if (_sex != value)
				{
					_sex = value;
					FirePropertyChanged("Sex");
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
			dicomDataSet[DicomTags.PatientId].SetStringValue(_patientId);
			dicomDataSet[DicomTags.PatientsName].SetStringValue(_name);
			dicomDataSet[DicomTags.PatientsBirthDate].SetDateTime(0, _birthdate);
			dicomDataSet[DicomTags.PatientsBirthTime].SetDateTime(0, _birthdate);
			dicomDataSet[DicomTags.PatientsSex].SetStringValue(DicomConverter.SetSex(_sex));
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
		public PatientNode Clone(bool deepCopy)
		{
			PatientNode node = new PatientNode();
			node._birthdate = this._birthdate;
			node._name = this._name;
			node._patientId = this._patientId;
			node._sex = this._sex;
			if (deepCopy)
			{
				foreach (StudyNode study in _studies)
				{
					node.Studies.Add(study.Clone(true));
				}
			}
			return node;
		}

		/// <summary>
		/// Performs a shallow copy of the node.
		/// </summary>
		/// <returns>A shallow copy of the node.</returns>
		public PatientNode Clone()
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
		/// Convenience method to insert SOP instance-level data nodes into the study builder tree under this patient, creating <see cref="StudyNode">study</see> and <see cref="SeriesNode">series</see> nodes as necessary.
		/// </summary>
		/// <param name="sopInstances">An array of <see cref="SopInstanceNode"/>s to insert into the study builder tree.</param>
		public void InsertSopInstance(SopInstanceNode[] sopInstances)
		{
			StudyNode study = new StudyNode();
			this.Studies.Add(study);
			SeriesNode series = new SeriesNode();
			study.Series.Add(series);
			foreach (SopInstanceNode node in sopInstances)
			{
				series.Images.Add(node);
			}
		}

		/// <summary>
		/// Convenience method to insert series-level data nodes into the study builder tree under this patient, creating a <see cref="StudyNode">study</see> node if necessary.
		/// </summary>
		/// <param name="series">An array of <see cref="SeriesNode"/>s to insert into the study builder tree.</param>
		public void InsertSeries(SeriesNode[] series)
		{
			StudyNode study = new StudyNode();
			this.Studies.Add(study);
			foreach (SeriesNode node in series)
			{
				study.Series.Add(node);
			}
		}

		/// <summary>
		/// Convenience method to insert study-level data nodes into the study builder tree under this patient.
		/// </summary>
		/// <param name="studies">An array of <see cref="StudyNode"/>s to insert into the study builder tree.</param>
		public void InsertStudy(StudyNode[] studies)
		{
			foreach (StudyNode node in studies)
			{
				this.Studies.Add(node);
			}
		}

		#endregion

		#region Studies Collection

		/// <summary>
		/// Gets a collection of all the <see cref="StudyNode">studies</see> that belong to this patient.
		/// </summary>
		public StudyNodeCollection Studies
		{
			get { return _studies; }
		}

		#endregion
	}
}