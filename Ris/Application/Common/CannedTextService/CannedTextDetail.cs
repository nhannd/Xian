using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [DataContract]
    public class CannedTextDetail : DataContractBase
    {
		public CannedTextDetail()
		{
		}

		public CannedTextDetail(string name, string path, string text)
        {
        	this.Name = name;
        	this.Path = path;
        	this.Text = text;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Path;

        [DataMember]
        public string Text;
    }
}
