using System;
using System.Collections.Generic;
using System.Text;

namespace SampleShred2
{
    public static class GlobalStore
    {
        private static double _currentPi;
        private static int _score;
        private static int _darts;
        private static Random _randomGenerator;

        static GlobalStore()
        {
            _randomGenerator = new Random();
        }

        public static Random RandomGenerator
        {
            get { return _randomGenerator; }
            set { _randomGenerator = value; }
        }
	
        public static int Darts
        {
            get { return _darts; }
            set { _darts = value; }
        }
	
        public static int Score
        {
            get { return _score; }
            set { _score = value; }
        }
	
        public static double CurrentPi
        {
            get { return _currentPi; }
            set { _currentPi = value; }
        }
	
    }
}
