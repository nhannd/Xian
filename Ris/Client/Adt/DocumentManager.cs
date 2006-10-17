using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class DocumentManager
    {
        private static Dictionary<long, Document> _documents = new Dictionary<long, Document>();

        public static void Set(long docID, Document doc)
        {
            _documents[docID] = doc;
        }

        public static Document Get(long docID)
        {
            return _documents.ContainsKey(docID) ? _documents[docID] : null;
        }

        public static void Remove(long docID)
        {
            _documents.Remove(docID);
        }

    }
}
