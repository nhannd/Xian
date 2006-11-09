using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class DocumentManager
    {
        private static Dictionary<string, Document> _documents = new Dictionary<string, Document>();

        public static void Set(string docID, Document doc)
        {
            _documents[docID] = doc;
        }

        public static Document Get(string docID)
        {
            return _documents.ContainsKey(docID) ? _documents[docID] : null;
        }

        public static void Remove(string docID)
        {
            _documents.Remove(docID);
        }

    }
}
