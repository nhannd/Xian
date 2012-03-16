using ClearCanvas.Common.Statistics;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Stores the statistics of a <see cref="CommandBase"/>.
    /// </summary>
    public class CommandStatistics : TimeSpanStatistics
    {
        public CommandStatistics(ICommand cmd)
            : base(cmd.Description)
        {

        }
    }
}
