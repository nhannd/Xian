namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    public interface IServerTreeNode
    {
        //TODO: when the server tree is refactored/rewritten, remove this.  It should really be at the app component level.
        bool IsChecked { get; }
        bool IsLocalDataStore { get; }
        bool IsServer { get; }
        bool IsServerGroup { get; }
        bool IsRoot { get; }
        string ParentPath { get; }
        string Path { get; }
        string Name { get; }
        string DisplayName { get; }
    }
}