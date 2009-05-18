using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
    public class CacheRemoveOptions
    {
        private string _region = "";

        public CacheRemoveOptions(string region)
        {
            _region = region;
        }

        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }
    }
}
