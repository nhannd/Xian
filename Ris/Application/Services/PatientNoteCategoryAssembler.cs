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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientNoteCategoryAssembler
    {
        public PatientNoteCategoryDetail CreateNoteCategoryDetail(PatientNoteCategory category, IPersistenceContext context)
        {
        	return new PatientNoteCategoryDetail(
        		category.GetRef(),
        		category.Name,
        		category.Description,
        		EnumUtils.GetEnumValueInfo(category.Severity, context),
        		category.Deactivated);
        }

        public PatientNoteCategorySummary CreateNoteCategorySummary(PatientNoteCategory category, IPersistenceContext context)
        {
            return new PatientNoteCategorySummary(
                category.GetRef(),
                category.Name,
                category.Description,
                EnumUtils.GetEnumValueInfo(category.Severity, context),
                category.Deactivated);
        }

        public void UpdateNoteCategory(PatientNoteCategoryDetail detail, PatientNoteCategory category)
        {
            category.Name = detail.Category;
            category.Description = detail.Description;
            category.Severity = EnumUtils.GetEnumValue<NoteSeverity>(detail.Severity);
            category.Deactivated = detail.Deactivated;
        }
    }
}
