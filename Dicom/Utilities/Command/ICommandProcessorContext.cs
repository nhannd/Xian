

using System;

namespace ClearCanvas.Dicom.Utilities.Command
{
    public interface ICommandProcessorContext : IDisposable
    {
        void PreExecute(ICommand command);

        void Commit();

        void Rollback();

        String TempDirectory { get; }
    }
}
