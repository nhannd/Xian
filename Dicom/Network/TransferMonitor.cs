using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public static class Format
    {
        public static string ByteCount(double bytes)
        {
            if (bytes > 1099511627776)
            {
                return String.Format("{0:0.00} TB", bytes / 1099511627776);
            }
            if (bytes > 1073741824)
            {
                return String.Format("{0:0.00} GB", bytes / 1073741824);
            }
            if (bytes > 1048576)
            {
                return String.Format("{0:0.00} MB", bytes / 1048576);
            }
            if (bytes > 1024)
            {
                return String.Format("{0:0.00} KB", bytes / 1024);
            }
            return String.Format("{0} B", bytes);
        }

        public static string AddSpaces(string str)
        {
            if (str.Length <= 2)
                return str;
            StringBuilder sb = new StringBuilder();
            sb.Append(str[0]);
            int count = str.Length - 1;
            for (int i = 1; i < count; i++)
            {
                bool prevUpper = Char.IsUpper(str[i - 1]);
                bool nextLower = Char.IsLower(str[i + 1]);
                bool currUpper = Char.IsUpper(str[i]);

                if (prevUpper && currUpper && nextLower)
                    sb.Append(' ');
                else if (!prevUpper && currUpper)
                    sb.Append(' ');
                sb.Append(str[i]);
            }
            sb.Append(str[str.Length - 1]);
            return sb.ToString();
        }
    }

    public class TransferMonitor
    {
        #region Private Members
        private const int HistorySize = 8;
        private const int UpdateSpan = 750;
        private double _bytesTemp;
        private int _bytesTransfered;
        private int _bytesTotal;
        private double _speedCurrent;
        private double _speedAverage;
        private double[] _speedHistory;
        private int _speedHistoryIndex;
        private int _lastUpdateTick;
        #endregion

        #region Public Constructors
        public TransferMonitor()
        {
            _lastUpdateTick = Environment.TickCount;
            _bytesTotal = 0;
            _bytesTransfered = 0;
            _bytesTemp = 0;
            _speedCurrent = 0.0;
            _speedAverage = 0.0;
            _speedHistory = new double[HistorySize];
            _speedHistoryIndex = 0;
        }
        #endregion

        #region Public Properties
        public double AverageSpeed
        {
            get { return _speedAverage; }
        }
        public double CurrentSpeed
        {
            get { return _speedCurrent; }
        }
        public string AverageSpeedString
        {
            get { return Format.ByteCount(AverageSpeed) + "/s"; }
        }
        public string CurrentSpeedString
        {
            get { return Format.ByteCount(CurrentSpeed) + "/s"; }
        }
        public int BytesTransfered
        {
            get { return _bytesTransfered; }
        }
        public int EstimatedBytesTotal
        {
            get { return _bytesTotal; }
        }
        public string EstimatedPercentTransfered
        {
            get { return String.Format("{0:0.0}%", ((double)BytesTransfered / (double)EstimatedBytesTotal) * 100); }
        }
        #endregion

        #region Internal Members
        internal void Tick(int bytes, int total)
        {
            _bytesTemp += bytes;
            _bytesTransfered += bytes;
            _bytesTotal = total;

            int currentTime = Environment.TickCount;
            int difference = currentTime - _lastUpdateTick;

            if (difference <= 0)
                difference = UpdateSpan + 1;

            if (difference <= UpdateSpan)
                return;

            _speedCurrent = _bytesTemp / (difference / 1000.0);

            _speedHistory[_speedHistoryIndex++] = _speedCurrent;

            if (_speedHistoryIndex == HistorySize)
                _speedHistoryIndex = 0;

            int count = 0;
            double history = 0.0;
            for (int i = 0; i < HistorySize; i++)
            {
                if (_speedHistory[i] != 0)
                {
                    history += _speedHistory[i];
                    count++;
                }
            }

            _speedAverage = history / count;

            _bytesTemp = 0;
            _lastUpdateTick = currentTime;
        }
        #endregion
    }
}
