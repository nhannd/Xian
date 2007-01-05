using System;
using System.Collections.Generic;
using System.Text;

namespace SampleShred1
{
    public static class GlobalStore
    {
        private static int _currentPrime;

        public static int CurrentPrime
        {
            get { return _currentPrime; }
            set { _currentPrime = value; }
        }
	
    }
}
