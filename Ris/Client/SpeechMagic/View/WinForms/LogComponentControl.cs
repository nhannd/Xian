using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common;

using SmIa = Philips.PSP.SmIa;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LogComponent"/>
    /// </summary>
    public partial class LogComponentControl : ApplicationComponentUserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LogComponentControl(LogComponent component)
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
            _logs.Columns.Add("Message", _logs.Width, HorizontalAlignment.Left);

            SmIaManager.Instance.OnLogUpdated += Instance_OnLogUpdated;
            SmIaManager.Instance.OnCommandRecognized += Instance_OnCommandRecognized;

            this.Disposed += LogComponentControl_Disposed;
        }

        void LogComponentControl_Disposed(object sender, EventArgs e)
        {
            SmIaManager.Instance.OnLogUpdated -= Instance_OnLogUpdated;
            SmIaManager.Instance.OnCommandRecognized -= Instance_OnCommandRecognized;
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
            string createdTime = Platform.Time.ToString("");

            ListViewItem newItem;
            newItem = _commandLogs.Items.Add(createdTime);
            newItem.SubItems.Add(grammar);
            newItem.SubItems.Add(symbol);
            newItem.SubItems.Add(commandText);
            newItem.SubItems.Add(Convert.ToString(confidence));
        }

        private void Instance_OnLogUpdated(string message, SmIaErrorLevel severity)
        {
            string createdTime = Platform.Time.ToString("");

            ListViewItem newItem;
            newItem = _logs.Items.Add(createdTime);
            newItem.SubItems.Add(severity.ToString());
            newItem.SubItems.Add(message);
        }

        private void _clearLog_Click(object sender, EventArgs e)
        {
            _commandLogs.Items.Clear();
        }

        private void _clearCommands_Click(object sender, EventArgs e)
        {
            _logs.Items.Clear();
        }
    }
}
