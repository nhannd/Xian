using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class DocumentManager
    {
        private static Dictionary<object, Document> _documents = new Dictionary<object, Document>();

        public static void Set(object key, Document doc)
        {
            _documents[key] = doc;
        }

        public static Document Get(object key)
        {
            return _documents.ContainsKey(key) ? _documents[key] : null;
        }

        public static void Remove(object key)
        {
            _documents.Remove(key);
        }

    }
}
