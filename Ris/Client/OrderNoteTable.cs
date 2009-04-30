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

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteTable : Table<OrderNoteDetail>
	{
	    public OrderNoteSummaryComponent _component;

        public OrderNoteTable(OrderNoteSummaryComponent component)
			: base(2)
		{
            _component = component;

			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnAuthor,
				delegate(OrderNoteDetail n) { return n.Author == null ? SR.LabelMe : StaffNameAndRoleFormat.Format(n.Author); },
				0.25f));
			this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnPostTime,
				delegate(OrderNoteDetail n) { return n.PostTime == null ? SR.LabelNew : Format.DateTime(n.PostTime); },
				0.25f));

            TableColumn<OrderNoteDetail, string> linkColumn = new TableColumn<OrderNoteDetail, string>(" ",
                delegate(OrderNoteDetail n) { return SR.ColumnMore; },
                0.05f);
            linkColumn.ClickLinkDelegate = delegate(OrderNoteDetail n) { component.UpdateNoteDetail(n); };
            this.Columns.Add(linkColumn);

            this.Columns.Add(new TableColumn<OrderNoteDetail, string>(SR.ColumnComments,
                delegate(OrderNoteDetail n) { return RemoveLineBreak(n.NoteBody); },
				0.5f, 1));
		}

        private static string RemoveLineBreak(string input)
        {
			if (string.IsNullOrEmpty(input))
				return input;

            string newString = input.Replace("\r\n", " ");
            newString = newString.Replace("\r", " ");
            newString = newString.Replace("\n", " ");
            return newString;
        }
	}
}
