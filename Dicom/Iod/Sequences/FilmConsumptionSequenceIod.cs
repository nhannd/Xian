#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Film Consumption Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table C4.17, pg 260</remarks>
    public class FilmConsumptionSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FilmConsumptionSequenceIod"/> class.
        /// </summary>
        public FilmConsumptionSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilmConsumptionSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public FilmConsumptionSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Number of films actually printed.
        /// </summary>
        /// <value>The number of films.</value>
        public int NumberOfFilms
        {
            get { return base.DicomAttributeProvider[DicomTags.NumberOfFilms].GetInt32(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.NumberOfFilms].SetInt32(0, value); }
        }

        /// <summary>
        /// Type(s) of medium on which images were printed.
        /// </summary>
        /// <value>The type of the medium.</value>
        public MediumType MediumType
        {
            get { return IodBase.ParseEnum<MediumType>(base.DicomAttributeProvider[DicomTags.MediumType].GetString(0, String.Empty), MediumType.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MediumType], value); }
        }

        /// <summary>
        /// Size(s) of film on which images were printed.
        /// </summary>
        /// <value>The film size id.</value>
        public FilmSize FilmSizeId
        {
            get { return IodBase.ParseEnum<FilmSize>(base.DicomAttributeProvider[DicomTags.FilmSizeId].GetString(0, String.Empty), FilmSize.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.FilmSizeId], value); }
        }
        
        
        #endregion
    }

    
}
