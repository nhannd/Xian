using System;
using System.Drawing;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

using SmIa = Philips.PSP.SmIa;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LogComponent"/>
    /// </summary>
    public partial class LogComponentControl : ApplicationComponentUserControl
    {
        private delegate void UpdateLogDelegate(string message, SmIaErrorLevel severity);
        private delegate void UpdateCommandDelegate(string grammar, string symbol, string commandText, double confidence);
        
        /// <summary>
        /// Constructor
        /// </summary>
        public LogComponentControl(IApplicationComponent component)
            :base(component)
        {
            InitializeComponent();

            _commandLogs.Columns.Add("Time", 75, HorizontalAlignment.Left);
            _commandLogs.Columns.Add("Grammar", 75, HorizontalAlignment.Center);
            _commandLogs.Columns.Add("Symbol", 75, HorizontalAlignment.Center);
            _commandLogs.Columns.Add("Text", 120, HorizontalAlignment.Center);
            _commandLogs.Columns.Add("Confidence", 70, HorizontalAlignment.Center);

            _logs.Columns.Add("Time", 75, HorizontalAlignment.Left);
            _logs.Columns.Add("Severity", 75, HorizontalAlignment.Center);
            _logs.Columns.Add("Message", _logs.Width - 75 - 75, HorizontalAlignment.Left);

            SmIaManager.Instance.LogUpdated += Instance_OnLogUpdated;
            SmIaManager.Instance.CommandRecognized += Instance_OnCommandRecognized;

            this.Disposed += LogComponentControl_Disposed;
        }

        void LogComponentControl_Disposed(object sender, EventArgs e)
        {
            SmIaManager.Instance.LogUpdated -= Instance_OnLogUpdated;
            SmIaManager.Instance.CommandRecognized -= Instance_OnCommandRecognized;
        }

        private void Instance_OnCommandRecognized(
            string grammar,
            string symbol,
            string commandText,
            double confidence,
            ref SmIa.SSemanticAttribute[] semanticAttributes,
            ref string[] textNonterminalTexts,
            SmIa.ICommandManipulation commandManipulator)
        {
            UpdateCommand(grammar, symbol, commandText, confidence);
        }

        private void Instance_OnLogUpdated(string message, SmIaErrorLevel severity)
        {
            UpdateLog(message, severity);
        }

        private void UpdateCommand(string grammar, string symbol, string commandText, double confidence)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new UpdateCommandDelegate(UpdateCommand), new object[] { grammar, symbol, commandText, confidence });
                return;
            }

            ListViewItem newItem = _commandLogs.Items.Add(Platform.Time.ToString("HH:mm:ss.fff"));
            newItem.SubItems.Add(grammar);
            newItem.SubItems.Add(symbol);
            newItem.SubItems.Add(commandText);
            newItem.SubItems.Add(Convert.ToString(confidence));
            newItem.EnsureVisible();
        }

        private void UpdateLog(string message, SmIaErrorLevel severity)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new UpdateLogDelegate(UpdateLog), new object[] { message, severity });
                return;
            }

            ListViewItem newItem = _logs.Items.Add(Platform.Time.ToString("HH:mm:ss.fff"));
            newItem.SubItems.Add(severity.ToString());
            newItem.SubItems.Add(message);
            newItem.ToolTipText = message;
            newItem.EnsureVisible();

            switch (severity)
            {
                case SmIaErrorLevel.Warning:
                    newItem.BackColor = Color.Yellow;
                    break;
                case SmIaErrorLevel.Error:
                case SmIaErrorLevel.Fatal:
                case SmIaErrorLevel.Lethal:
                    newItem.BackColor = Color.Red;
                    break;
                case SmIaErrorLevel.None:
                case SmIaErrorLevel.Information:
                default:
                    break;
            }
        }

        private void _clearLog_Click(object sender, EventArgs e)
        {
            _logs.Items.Clear();
        }

        private void _clearCommands_Click(object sender, EventArgs e)
        {
            _commandLogs.Items.Clear();
        }
    }
}
