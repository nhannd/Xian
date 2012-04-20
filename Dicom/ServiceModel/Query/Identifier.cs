#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
    public interface IIdentifier
    {
        [DicomField(DicomTags.SpecificCharacterSet)]
        string SpecificCharacterSet { get; }

        [DicomField(DicomTags.RetrieveAeTitle)]
        string RetrieveAeTitle { get; }

        [DicomField(DicomTags.InstanceAvailability)]
        string InstanceAvailability { get; }

        IApplicationEntity RetrieveAE { get; }
    }

    /// <summary>
    /// Base class for Dicom query Identifiers.
    /// </summary>
    [DataContract(Namespace = QueryNamespace.Value)]
    public abstract class Identifier : IIdentifier
    {
        #region Private Fields

        private IApplicationEntity _retrieveAe;
        private string _retrieveAeTitle;

        #endregion

        #region Internal Constructors

        internal Identifier()
        {
            InstanceAvailability = "";
            _retrieveAeTitle = "";
            SpecificCharacterSet = "";
        }

        internal Identifier(IIdentifier other)
            : this()
        {
            if (other == null)
                return;

            SpecificCharacterSet = other.SpecificCharacterSet;
            InstanceAvailability = other.InstanceAvailability;
            RetrieveAeTitle = other.RetrieveAeTitle;
            RetrieveAE = other.RetrieveAE;
        }

        internal Identifier(DicomAttributeCollection attributes)
            : this()
        {
            Initialize(attributes);
        }

        #endregion

        internal void Initialize(DicomAttributeCollection attributes)
        {
            attributes.LoadDicomFields(this);
        }

        #region Public Properties

        /// <summary>
        /// Gets the level of the query.
        /// </summary>
        public abstract string QueryRetrieveLevel { get; }

        /// <summary>
        /// Gets or sets the Specific Character set of the identified instance.
        /// </summary>
        [DicomField(DicomTags.SpecificCharacterSet), DataMember(IsRequired = false)]
        public string SpecificCharacterSet { get; set; }

        /// <summary>
        /// Gets or sets the availability of the identified instance.
        /// </summary>
        [DicomField(DicomTags.InstanceAvailability), DataMember(IsRequired = false)]
        public string InstanceAvailability { get; set; }

        /// <summary>
        /// Gets or sets the AE Title the identified instance can be retrieved from.
        /// </summary>
        /// <remarks>This property's value never takes precedence over <see cref="RetrieveAE"/>,
        /// since it actually provides less information.</remarks>
        /// <exception cref="InvalidOperationException">thrown when trying to set this property and <see cref="RetrieveAE"/> is non-null.</exception>
        [DicomField(DicomTags.RetrieveAeTitle)]
        [DataMember(IsRequired = false)]
        public string RetrieveAeTitle
        {
            get
            {
                if (RetrieveAE != null)
                    return RetrieveAE.AETitle;

                return _retrieveAeTitle;
            }
            set
            {
                if (RetrieveAE != null)
                    throw new InvalidOperationException(
                        "The Retrieve AE Title cannot currently be set because RetrieveAE is non-null.");

                _retrieveAeTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the AE from which the identified instance can be retrieved.
        /// </summary>
        /// <remarks>This property's value always takes precedence over <see cref="RetrieveAeTitle"/>, since it provides more information.</remarks>
        public IApplicationEntity RetrieveAE
        {
            get { return _retrieveAe; }
            set
            {
                if (value == null)
                {
                    _retrieveAe = null;
                }
                else
                {
                    _retrieveAe = value;
                    //AE has more information and takes precedence.
                    _retrieveAeTitle = null;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts this object into a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        public DicomAttributeCollection ToDicomAttributeCollection()
        {
            var attributes = new DicomAttributeCollection();
            if (!string.IsNullOrEmpty(SpecificCharacterSet))
                attributes.SpecificCharacterSet = SpecificCharacterSet;

            attributes[DicomTags.QueryRetrieveLevel].SetStringValue(QueryRetrieveLevel);
            attributes.SaveDicomFields(this);

            return attributes;
        }

        /// <summary>
        /// Factory method to create an <see cref="Identifier"/> of type <typeparamref name="T"/> from
        /// the given <see cref="DicomAttributeCollection"/>.
        /// </summary>
        public static T FromDicomAttributeCollection<T>(DicomAttributeCollection attributes) where T : Identifier, new()
        {
            var identifier = new T();
            identifier.Initialize(attributes);
            return identifier;
        }

        #endregion
    }
}