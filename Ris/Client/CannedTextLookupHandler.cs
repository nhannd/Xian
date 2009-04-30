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
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
    public interface ICannedTextLookupHandler : ILookupHandler
    {
    	string GetFullText(CannedText cannedText);
    }

	/// <summary>
	/// This class is created so it can be shared between model and the view.  This way the summary object does not have to be exposed to the view.
	/// </summary>
    public class CannedText
    {
        private readonly string _name;
		private readonly string _category;
    	private readonly string _staffId;
    	private readonly string _staffGroupName;
		private readonly string _text;

		public CannedText(string name, string category, string staffId, string staffGroupName, string text)
        {
            _name = name;
            _category = category;
			_staffId = staffId;
			_staffGroupName = staffGroupName;
			_text = text;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Category
        {
            get { return _category; }
        }

        public string StaffId
        {
			get { return _staffId; }
        }

        public string StaffGroupName
        {
			get { return _staffGroupName; }
        }

        public string Text
        {
			get { return _text; }
        }

    	public bool IsSnippet
    	{
			get { return _text.Length.Equals(CannedTextSummary.MaxTextLength); }
    	}
    }

    public class CannedTextLookupHandler : ICannedTextLookupHandler
    {
        class CannedTextSuggestionProvider : SuggestionProviderBase<CannedText>
        {
            protected override IList<CannedText> GetShortList(string query)
            {
				List<CannedText> cannedTexts = new List<CannedText>();
				Platform.GetService<ICannedTextService>(
					delegate(ICannedTextService service)
					{
						ListCannedTextResponse response = service.ListCannedText(new ListCannedTextRequest());
						cannedTexts = CollectionUtils.Map<CannedTextSummary, CannedText>(response.CannedTexts,
							delegate(CannedTextSummary s)
								{
									return new CannedText(
										s.Name, 
										s.Category, 
										s.Staff == null ? null : s.Staff.StaffId,
										s.StaffGroup == null ? null : s.StaffGroup.Name,
										s.TextSnippet);
								});
					});

				// sort results in the way that they will be formatted for the suggest box
				cannedTexts.Sort(
					delegate(CannedText x, CannedText y)
					{
						return CannedTextLookupHandler.FormatItem(x).CompareTo(CannedTextLookupHandler.FormatItem(y));
					});

            	return cannedTexts;
			}

            protected override string FormatItem(CannedText item)
            {
				return CannedTextLookupHandler.FormatItem(item);
            }
        }


        private CannedTextSuggestionProvider _suggestionProvider;
        private readonly IDesktopWindow _desktopWindow;


        public CannedTextLookupHandler(IDesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        private static string FormatItem(CannedText ct)
        {
			return string.Format("{0} ({1})", ct.Name, ct.Category);
		}


        #region ILookupHandler Members

        bool ILookupHandler.Resolve(string query, bool interactive, out object result)
        {
            result = null;
            return false;
        }

        string ILookupHandler.FormatItem(object item)
        {
            return FormatItem((CannedText)item);
        }

        ISuggestionProvider ILookupHandler.SuggestionProvider
        {
            get
            {
                if (_suggestionProvider == null)
                {
                    _suggestionProvider = new CannedTextSuggestionProvider();
                }
                return _suggestionProvider;
            }
        }

        #endregion

		#region ICannedTextLookupHandler Members

		public string GetFullText(CannedText cannedText)
		{
			string fullText = null;

			try
			{
				Platform.GetService<ICannedTextService>(
					delegate(ICannedTextService service)
					{
						LoadCannedTextForEditResponse response = service.LoadCannedTextForEdit(
							new LoadCannedTextForEditRequest(
								cannedText.Name,
								cannedText.Category,
								cannedText.StaffId,
								cannedText.StaffGroupName));

						fullText = response.CannedTextDetail.Text;
					});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, _desktopWindow);
			}

			return fullText;
		}

		#endregion
	}
}
