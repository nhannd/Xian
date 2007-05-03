using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using System.Collections;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public interface IFolder
    {
        /// <summary>
        /// Gets the text that should be displayed for the folder
        /// </summary>
        string Text { get; }
        
        /// <summary>
        /// Allows the folder to notify that it's text has changed
        /// </summary>
        event EventHandler TextChanged;

        /// <summary>
        /// Gets the iconset that should be displayed for the folder
        /// </summary>
        IconSet IconSet { get; }

        /// <summary>
        /// Gets the resource resolver that is used to resolve the Icon
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Allows the folder to nofity that it's icon has changed
        /// </summary>
        event EventHandler IconChanged;

        /// <summary>
        /// Gets the tooltip that should be displayed for the folder
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Allows the folder to notify that it's tooltip has changed
        /// </summary>
        event EventHandler TooltipChanged;

        /// <summary>
        /// Gets the menu model for the context menu that should be displayed when the user right-clicks on the folder
        /// </summary>
        ActionModelNode MenuModel { get; }

        /// <summary>
        /// Gets the open/close state of the current folder
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Allows the folder to notify when refresh is about to begin
        /// </summary>
        event EventHandler RefreshBegin;

        /// <summary>
        /// Allows the folder to notify when refresh is about to finish
        /// </summary>
        event EventHandler RefreshFinish;

        void Refresh();

        void RefreshCount();

        void OpenFolder();
        void CloseFolder();

        /// <summary>
        /// Asks the folder if it can accept a drop of the specified items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        DragDropKind CanAcceptDrop(object[] items, DragDropKind kind);

        /// <summary>
        /// Instructs the folder to accept the specified items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="kind"></param>
        DragDropKind AcceptDrop(object[] items, DragDropKind kind);

        /// <summary>
        /// Informs the folder that the specified items were dragged from it.  It is up to the implementation
        /// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
        /// </summary>
        /// <param name="items"></param>
        /// <param name="result">The result of the drag drop operation</param>
        void DragComplete(object[] items, DragDropKind result);

        /// <summary>
        /// Gets a table of the items that are contained in this folder
        /// </summary>
        ITable ItemsTable { get; }
    }
}
