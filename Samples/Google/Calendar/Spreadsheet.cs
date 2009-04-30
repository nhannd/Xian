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
using System.Text;
using Google.GData.Spreadsheets;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using Google.GData.Client;

namespace ClearCanvas.Samples.Calendar
{
    [MenuAction("show", "global-menus/Google/Spreadsheet")]
    [ClickHandler("show", "Show")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class SpreadsheetTool : Tool<IDesktopToolContext>
    {
        public void Show()
        {
            Spreadsheet sheet = Spreadsheet.GetByTitle("DevConf");
            Worksheet worksheet = sheet.FirstWorksheet;
            List<WorksheetRow> rows = worksheet.ListRows(new Dictionary<string, string>());
            foreach (WorksheetRow row in rows)
            {
                Console.WriteLine(row["Name"] + row["Age"]);
                row["Age"] += "1";
                row.Save();
            }
        }
    }

    public class Spreadsheet
    {
        const string URL = "http://spreadsheets.google.com/feeds/spreadsheets/private/full";

        private SpreadsheetEntry _entry;
        private SpreadsheetsService _service;


        public static List<Spreadsheet> ListAll()
        {
            SpreadsheetsService service = new SpreadsheetsService("ClearCanvas-Workstation-1.0");
            service.setUserCredentials("jresnick", "bl00b0lt");
            
            SpreadsheetQuery query = new SpreadsheetQuery();
            SpreadsheetFeed feed = service.Query(query);

            return CollectionUtils.Map<SpreadsheetEntry, Spreadsheet, List<Spreadsheet>>(
                feed.Entries,
                delegate(SpreadsheetEntry entry) { return new Spreadsheet(service, entry); });
        }

        public static Spreadsheet GetByTitle(string title)
        {
            SpreadsheetsService service = new SpreadsheetsService("ClearCanvas-Workstation-1.0");
            service.setUserCredentials("jresnick", "bl00b0lt");

            SpreadsheetQuery query = new SpreadsheetQuery();
            query.Title = title;

            SpreadsheetFeed feed = service.Query(query);
            return feed.Entries.Count > 0 ? new Spreadsheet(service, (SpreadsheetEntry)feed.Entries[0]) : null;
        }


        internal Spreadsheet(SpreadsheetsService service, SpreadsheetEntry entry)
        {
            _entry = entry;
            _service = service;
        }

        public string Title
        {
            get { return _entry.Title.Text; }
        }

        public Worksheet FirstWorksheet
        {
            get
            {
                List<Worksheet> worksheets = ListWorksheets();
                return worksheets.Count > 0 ? worksheets[0] : null;
            }
        }

        public List<Worksheet> ListWorksheets()
        {
            AtomLink link = _entry.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

            WorksheetQuery query = new WorksheetQuery(link.HRef.ToString());
            WorksheetFeed feed = _service.Query(query);

            return CollectionUtils.Map<WorksheetEntry, Worksheet, List<Worksheet>>(
                feed.Entries,
                delegate(WorksheetEntry entry) { return new Worksheet(_service, entry); });
        }
    }

    public class Worksheet
    {
        private WorksheetEntry _entry;
        private SpreadsheetsService _service;

        internal Worksheet(SpreadsheetsService service, WorksheetEntry entry)
        {
            _entry = entry;
            _service = service;
        }

        public string Title
        {
            get { return _entry.Title.Text; }
        }

        public List<WorksheetRow> ListRows(Dictionary<string, string> parameters)
        {
            AtomLink listFeedLink = _entry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            ListQuery query = new ListQuery(listFeedLink.HRef.ToString());

            string sq = CollectionUtils.Reduce<string, string>(parameters.Keys, "",
                delegate(string key, string memo)
                {
                    return (string.IsNullOrEmpty(memo) ? "" : " and ")
                        + string.Format("{0}={1}", key, parameters[key]);
                });

            query.SpreadsheetQuery = sq;

            ListFeed feed = _service.Query(query);

            return CollectionUtils.Map<ListEntry, WorksheetRow, List<WorksheetRow>>(
                feed.Entries,
                delegate(ListEntry entry) { return new WorksheetRow(feed, entry); });
        }
    }

    public class WorksheetRow
    {
        private bool _unsaved;
        private ListEntry _entry;
        private ListFeed _feed;

        internal WorksheetRow(ListFeed feed, ListEntry entry)
        {
            _entry = entry;
            _feed = feed;
        }

        internal WorksheetRow(ListFeed feed)
        {
            // TODO need to pre-init listEntry columns
            _feed = feed;
            _entry = new ListEntry();
            _unsaved = true;
        }

        public void Save()
        {
            if (_unsaved)
            {
                _entry = _feed.Insert(_entry) as ListEntry;
                _unsaved = false;
            }
            else
            {
                _entry = _entry.Update() as ListEntry;
            }
        }

        public string this[string key]
        {
            get
            {
                ListEntry.Custom entry = CollectionUtils.SelectFirst<ListEntry.Custom>(_entry.Elements,
                    delegate(ListEntry.Custom e) { return e.LocalName.Equals(key, StringComparison.CurrentCultureIgnoreCase); });
                return entry == null ? null : entry.Value;
            }
            set
            {
                ListEntry.Custom entry = CollectionUtils.SelectFirst<ListEntry.Custom>(_entry.Elements,
                    delegate(ListEntry.Custom e) { return e.LocalName.Equals(key, StringComparison.CurrentCultureIgnoreCase); });
                if (entry == null)
                {
                    entry = new ListEntry.Custom();
                    _entry.Elements.Add(entry);
                }
                entry.Value = value;
            }
        }
    }

}
