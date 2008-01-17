namespace ClearCanvas.Ris.Shreds.Publication
{
    /// <summary>
    /// Processes scheduled <see cref="ClearCanvas.Healthcare.PublicationStep"/> and marks steps as completed
    /// TODO:  only completes the steps now - no processing.
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Begins processing of scheduled <see cref="ClearCanvas.Healthcare.PublicationStep"/>
        /// </summary>
        void Start();

        /// <summary>
        /// Stop processing of <see cref="ClearCanvas.Healthcare.PublicationStep"/>
        /// </summary>
        /// <remarks>
        /// Processing will not stop until the current batch of <see cref="ClearCanvas.Healthcare.PublicationStep"/> is completed.
        /// </remarks>
        void RequestStop();
    }
}
