#region License

//MWL Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.Mwl
{
    internal class MwlGetItemsContext : IMwlGetItemsContext
    {
        public MwlGetItemsContext(SearchResultPage page)
        {
            ResultPage = page;
        }

        public SearchResultPage ResultPage { get; private set; }
    }

    public class RisQuerier
    {
        private class ConvertMessageContext : IConvertMessageContext
        {
            public ConvertMessageContext(DicomMessage message)
            {
                Message = message;
            }

            public DicomMessage Message { get; private set; }
        }

        private class ConvertResultContext : ConvertMessageContext, IConvertResultContext
        {
            public ConvertResultContext(MwlWorklistItem item, DicomMessage message)
                : base(message)
            {
                Item = item;
            }

            public MwlWorklistItem Item { get; private set; }
        }

        private readonly MessagePipeline _messagePipeline;

        public RisQuerier()
        {
            _messagePipeline = new MessagePipeline();
        }

        public IList<DicomMessage> Query(DicomMessage queryMessage, string callingAE)
        {
            var criteria = _messagePipeline.ConvertMessage(new List<MwlWorklistItemSearchCriteria>(),
                                                           new ConvertMessageContext(queryMessage));


            IList<MwlWorklistItem> items;
            if (CollectionUtils.TrueForAll(criteria, c => c.IsEmpty))
                items = new List<MwlWorklistItem>();
            else
            {
                using (var scope = new PersistenceScope(PersistenceContextType.Read))
                {
                    items = PersistenceScope.CurrentContext.GetBroker<IMwlWorklistItemBroker>().GetItems(
                        criteria.ToArray(), MwlWorklistItemProjection.Projection,
                        new MwlGetItemsContext(
                            new SearchResultPage
                                {
                                    FirstRow = 0,
                                    MaxRows = MwlSettings.Default.MaxQueryResults
                                })
                        );
                    scope.Complete();
                }
            }

            Platform.Log(LogLevel.Info, String.Format("Number Of Query Results Returned: {0}", items.Count));

            return items.Select(
                item =>
                _messagePipeline.ConvertResult(new DicomMessage(), new ConvertResultContext(item, queryMessage))
                ).ToList();
        }
    }
}