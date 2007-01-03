using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ClearCanvas.Desktop.Actions;
using System.Collections;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Common
{
    public interface IFolder
    {
        string Text { get; }
        event EventHandler TextChanged;

        Image Icon { get; }
        event EventHandler IconChanged;

        string Tooltip { get; }
        event EventHandler TooltipChanged;

        ActionModelNode MenuModel { get; }

        void Refresh();

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

        ITable ItemsTable { get; }
    }
}
