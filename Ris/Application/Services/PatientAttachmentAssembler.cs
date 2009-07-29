#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientAttachmentAssembler
    {
        class PatientAttachmentSynchronizeHelper : CollectionSynchronizeHelper<PatientAttachment, PatientAttachmentSummary>
        {
            private readonly PatientAttachmentAssembler _assembler;
            private readonly IPersistenceContext _context;
            private readonly Staff _currentUserStaff;

            public PatientAttachmentSynchronizeHelper(PatientAttachmentAssembler assembler, Staff currentUserStaff, IPersistenceContext context)
                :base(true, true)
            {
                _assembler = assembler;
                _context = context;
                _currentUserStaff = currentUserStaff;
            }

            protected override bool CompareItems(PatientAttachment domainItem, PatientAttachmentSummary sourceItem)
            {
                return Equals(domainItem.Document.GetRef(), sourceItem.Document.DocumentRef);
            }

            protected override void AddItem(PatientAttachmentSummary sourceItem, ICollection<PatientAttachment> domainList)
            {
                domainList.Add(_assembler.CreatePatientAttachment(sourceItem, _currentUserStaff, _context));
            }

            protected override void UpdateItem(PatientAttachment domainItem, PatientAttachmentSummary sourceItem, ICollection<PatientAttachment> domainList)
            {
                _assembler.UpdatePatientAttachment(domainItem, sourceItem, _context);
            }

            protected override void RemoveItem(PatientAttachment domainItem, ICollection<PatientAttachment> domainList)
            {
                domainList.Remove(domainItem);
            }
        }

        public void Synchronize(IList<PatientAttachment> domainList, IList<PatientAttachmentSummary> sourceList, Staff currentUserStaff, IPersistenceContext context)
        {
            PatientAttachmentSynchronizeHelper synchronizer = new PatientAttachmentSynchronizeHelper(this, currentUserStaff, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

        public PatientAttachmentSummary CreatePatientAttachmentSummary(PatientAttachment attachment, IPersistenceContext context)
        {
            AttachedDocumentAssembler attachedDocAssembler = new AttachedDocumentAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();

            return new PatientAttachmentSummary(
                EnumUtils.GetEnumValueInfo(attachment.Category),
                staffAssembler.CreateStaffSummary(attachment.AttachedBy, context),
                attachedDocAssembler.CreateAttachedDocumentSummary(attachment.Document));
        }

        public PatientAttachment CreatePatientAttachment(PatientAttachmentSummary summary, Staff currentUserStaff, IPersistenceContext context)
        {
            return new PatientAttachment(
                EnumUtils.GetEnumValue<PatientAttachmentCategoryEnum>(summary.Category, context),
                summary.AttachedBy == null ? currentUserStaff : context.Load<Staff>(summary.AttachedBy.StaffRef),
                context.Load<AttachedDocument>(summary.Document.DocumentRef));
        }

        public void UpdatePatientAttachment(PatientAttachment attachment, PatientAttachmentSummary summary, IPersistenceContext context)
        {
            AttachedDocumentAssembler attachedDocAssembler = new AttachedDocumentAssembler();
            attachment.Category = EnumUtils.GetEnumValue<PatientAttachmentCategoryEnum>(summary.Category, context);
            attachedDocAssembler.UpdateAttachedDocumentSummary(attachment.Document, summary.Document);
        }
    }
}
