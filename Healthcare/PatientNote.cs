#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientNote
    /// </summary>
    public partial class PatientNote
	{
        /// <summary>
        /// Constructor for creating a new patient note.
        /// </summary>
        /// <param name="author"></param>
        /// <param name="category"></param>
        /// <param name="comment"></param>
        public PatientNote(Staff author, PatientNoteCategory category, string comment)
        {
            _author = author;
            _category = category;
            _comment = comment;

            // valid from now indefinitely
            _creationTime = Platform.Time;
            _validRange = new DateTimeRange(_creationTime, null);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PatientNote"/> is currently valid.
        /// </summary>
        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PatientNote"/> has expired.
        /// </summary>
        public bool IsExpired
        {
            get { return _validRange != null && _validRange.Until < Platform.Time; }
        }

        private void CustomInitialize()
        {
        }
    }
}