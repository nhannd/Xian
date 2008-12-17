
using System.Runtime.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Query identifier for a composite object instance.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageIdentifier : Identifier
	{
		#region Private Fields

		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private string _sopInstanceUid;
		private string _sopClassUid;
		private int? _instanceNumber;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ImageIdentifier()
		{
		}

		/// <summary>
		/// Creates an instance of <see cref="ImageIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		public ImageIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the level of the query - IMAGE.
		/// </summary>
		public override string QueryRetrieveLevel
		{
			get { return "IMAGE"; }
		}

		/// <summary>
		/// Gets or sets the Study Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Series Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Sop Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
			set { _sopInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Sop Class Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopClassUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SopClassUid
		{
			get { return _sopClassUid; }
			set { _sopClassUid = value; }
		}

		/// <summary>
		/// Gets or sets the Instance Number of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.InstanceNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public int? InstanceNumber
		{
			get { return _instanceNumber; }
			set { _instanceNumber = value; }
		}

		#endregion
	}
}
