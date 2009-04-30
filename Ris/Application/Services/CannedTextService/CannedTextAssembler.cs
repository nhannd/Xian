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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Application.Services.CannedTextService
{
	public class CannedTextAssembler
	{
		public CannedTextSummary GetCannedTextSummary(CannedText cannedText, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextSummary(
				cannedText.GetRef(),
				cannedText.Name,
				cannedText.Category,
				cannedText.Staff == null ? null : staffAssembler.CreateStaffSummary(cannedText.Staff, context),
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
				cannedText.Text);
		}

		public CannedTextDetail GetCannedTextDetail(CannedText cannedText, IPersistenceContext context)
		{
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();

			return new CannedTextDetail(
				cannedText.Name,
				cannedText.Category,
				cannedText.StaffGroup == null ? null : groupAssembler.CreateSummary(cannedText.StaffGroup),
				cannedText.Text);
		}

		public CannedText CreateCannedText(CannedTextDetail detail, Staff owner, IPersistenceContext context)
		{
			CannedText newCannedText = new CannedText();
			UpdateCannedText(newCannedText, detail, owner, context);
			return newCannedText;
		}

		public void UpdateCannedText(CannedText cannedText, CannedTextDetail detail, Staff owner, IPersistenceContext context)
		{
			cannedText.Name = detail.Name;
			cannedText.Category = detail.Category;
			cannedText.Staff = detail.IsPersonal ? owner : null;
			cannedText.StaffGroup = detail.IsGroup ? context.Load<StaffGroup>(detail.StaffGroup.StaffGroupRef, EntityLoadFlags.Proxy) : null;
			cannedText.Text = detail.Text;
		}
	}
}
