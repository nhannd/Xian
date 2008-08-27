using System;
using System.IO;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a SOP instance-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class SopInstanceNode : StudyBuilderNode, ICloneable
	{
		private readonly DicomFile _dicomFile;
		private string _uid;

		/// <summary>
		/// Constructs a new <see cref="SopInstanceNode"/> using default values.
		/// </summary>
		public SopInstanceNode()
		{
			_dicomFile = new DicomFile("");
			RegenerateUids();
		}

		/// <summary>
		/// Constructs a new <see cref="SopInstanceNode"/> using the given <see cref="DicomFile"/> as a template.
		/// </summary>
		/// <param name="sourceDicomFile">The <see cref="DicomFile"/> from which to initialize this node.</param>
		public SopInstanceNode(DicomMessageBase sourceDicomFile)
		{
			_dicomFile = new DicomFile("", sourceDicomFile.MetaInfo.Copy(true), sourceDicomFile.DataSet.Copy(true));
			_uid = sourceDicomFile.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
			if (_uid == "")
				RegenerateUids();
		}

		#region Data Properties

		/// <summary>
		/// Gets or sets the SOP instance UID.
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

		#endregion

		#region Update and Misc Members

		/// <summary>
		/// Writes the data in this node into the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dataSet">The data set to write data into.</param>
		/// <param name="imageNumber"></param>
		internal void Update(DicomAttributeCollection dataSet, int imageNumber)
		{
			dataSet[DicomTags.SopInstanceUid].SetStringValue(_uid);
			dataSet[DicomTags.InstanceNumber].SetInt32(0, imageNumber);
		}

		/// <summary>
		/// Forces the regeneration of all UIDs owned at the SOP instance-level.
		/// </summary>
		public void RegenerateUids()
		{
			_uid = DicomUid.GenerateUid().UID;
		}

		/// <summary>
		/// Gets the underlying data set of this node.
		/// </summary>
		public DicomAttributeCollection DicomData
		{
			get { return _dicomFile.DataSet; }
		}

		/// <summary>
		/// Exports the contents of the data set to a DICOM file in the specified directory.
		/// </summary>
		/// <remarks>
		/// The filename is automatically generated using the SOP instance uid and the &quot;.dcm&quot; extension.
		/// </remarks>
		/// <param name="path">The directory to export the data to.</param>
		internal void ExportToDirectory(string path)
		{
			string filename = Path.Combine(path, _uid + ".dcm");
			_dicomFile.Save(filename);
		}

		#endregion

		#region Cloning Methods

		/// <summary>
		/// Performs a copy of the node.
		/// </summary>
		/// <returns>A copy of the node.</returns>
		public SopInstanceNode Clone()
		{
			SopInstanceNode node = new SopInstanceNode(_dicomFile);
			return node;
		}

		/// <summary>
		/// Performs a copy of the node.
		/// </summary>
		/// <returns>A copy of the node.</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion

	}
}