namespace ClearCanvas.ImageViewer.Automation
{
    public interface IStack
    {
        void StackBy(int delta);
        void StackTo(int instanceNumber, int? frameNumber);
    }
}