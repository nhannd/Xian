using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.CannedTextService;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public interface ICannedTextLookupHandler : ILookupHandler
    {
    }

    public class CannedText
    {
        private string _name;
        private string _path;
        private string _text;

        public CannedText(string name, string path, string text)
        {
            _name = name;
            _path = path;
            _text = text;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Path
        {
            get { return _path; }
        }

        public string Text
        {
            get { return _text; }
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
							delegate(CannedTextSummary s) { return new CannedText(s.Name, s.Path, s.Text); });
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
			return string.Format("{0} ({1})", ct.Name, ct.Path);
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
    }
}
