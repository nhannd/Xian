using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[DataContract(Namespace = QueryNamespace.Value)]
	public abstract class Identifier
	{
		#region Private Fields

		private string _specificCharacterSet = "";
		private string _retrieveAeTitle = "";
		private string _instanceAvailability = "";

		#endregion

		#region Internal Constructors

		internal Identifier()
		{
		}

		internal Identifier(DicomAttributeCollection attributes)
		{
			Initialize(attributes);
		}

		#endregion

		internal void Initialize(DicomAttributeCollection attributes)
		{
			attributes.LoadDicomFields(this);
		}

		#region Public Properties

		public abstract string QueryRetrieveLevel { get; }

		[DicomField(DicomTags.SpecificCharacterSet)] //only include in rq when set explicitly.
		[DataMember(IsRequired = false)]
		public string SpecificCharacterSet
		{
			get { return _specificCharacterSet; }
			set { _specificCharacterSet = value; }
		}

		[DicomField(DicomTags.RetrieveAeTitle)]
		[DataMember(IsRequired = false)]
		public string RetrieveAeTitle
		{
			get { return _retrieveAeTitle; }
			set { _retrieveAeTitle = value; }
		}

		[DicomField(DicomTags.InstanceAvailability)]
		[DataMember(IsRequired = false)]
		public string InstanceAvailability
		{
			get { return _instanceAvailability; }
			set { _instanceAvailability = value; }
		}

		#endregion

		#region Public Methods

		public DicomAttributeCollection ToDicomAttributeCollection()
		{
			DicomAttributeCollection attributes = new DicomAttributeCollection();
			if (!string.IsNullOrEmpty(_specificCharacterSet))
				attributes.SpecificCharacterSet = _specificCharacterSet;

			attributes[DicomTags.SpecificCharacterSet] = null;
			attributes[DicomTags.QueryRetrieveLevel].SetStringValue(QueryRetrieveLevel);

			attributes.SaveDicomFields(this);

			return attributes;
		}

		public static T FromDicomAttributeCollection<T>(DicomAttributeCollection attributes) where T : Identifier, new()
		{
			T identifier = new T();
			identifier.Initialize(attributes);
			return identifier;
		}

		#endregion
	}
}
