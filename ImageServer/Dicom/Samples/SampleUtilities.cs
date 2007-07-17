using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Dicom.Samples
{
    public static class SampleUtilities
    {
        private static TextBox _tb;
        private static int _threadId;
        private static StringBuilder _saveLogs = null;
        public static void Log(DicomLogInfo info)
        {
            if (_threadId != Thread.CurrentThread.ManagedThreadId)
            {
                if (_saveLogs == null)
                    _saveLogs = new StringBuilder();

                _saveLogs.AppendLine();
                _saveLogs.AppendFormat("({0}) {1} {2} ({3}) {4}", info.ThreadId, 
                             info.Time.ToShortDateString(), info.Time.ToLongTimeString(),
                             info.Level, info.Message);
            }
            else
            {
                if (_saveLogs != null)
                {
                    _tb.AppendText(_saveLogs.ToString());
                    _saveLogs = null;
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendFormat("({0}) {1} {2} ({3}) {4}", info.ThreadId,
                             info.Time.ToShortDateString(), info.Time.ToLongTimeString(),
                             info.Level, info.Message);

                _tb.AppendText( sb.ToString() );
            }
        }

        public static void RegisterLogHandler(TextBox tb)
        {
            _tb = tb;
            DicomLogger.LogDelegates += SampleUtilities.Log;
            _threadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
