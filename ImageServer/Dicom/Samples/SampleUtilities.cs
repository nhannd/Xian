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
        public static void Log(DicomLogInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat("({0}) {1} {2} ({3}) {4}", info.ThreadId,
                         info.Time.ToShortDateString(), info.Time.ToLongTimeString(),
                         info.Level, info.Message);

            AppendToTextBox(sb.ToString());
        }

        private static void AppendToTextBox(string info)
        {
            if (_tb == null)
                return;

            if (!_tb.InvokeRequired)
            {
                _tb.AppendText(info);
            }
            else
            {
                _tb.BeginInvoke(new Action<string>(AppendToTextBox), new object[] { info });
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
