using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
    public class CachePutOptions
    {
        private string _region = "";
        private TimeSpan _expiration;
        private bool _sliding;

        public CachePutOptions()
        {
        }

        public CachePutOptions(string region, TimeSpan expiration, bool sliding)
        {
            _region = region;
            _expiration = expiration;
            _sliding = sliding;
        }

        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public TimeSpan Expiration
        {
            get { return _expiration; }
            set { _expiration = value; }
        }

        public bool Sliding
        {
            get { return _sliding; }
            set { _sliding = value; }
        }
    }
}
