namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Defines the interface of an alert service
    /// </summary>
    public interface IAlertService
    {
        /// <summary>
        /// Generates an alert record based on the specified <see cref="Alert"/>
        /// </summary>
        /// <param name="alert"></param>
        void GenerateAlert(Alert alert);
    }
}
