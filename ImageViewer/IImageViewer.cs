using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
    public interface IImageViewer
    {
        /// <summary>
        /// Gets the <see cref="PhysicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="PhysicalWorkspace"/>.</value>
        PhysicalWorkspace PhysicalWorkspace { get; }

        /// <summary>
        /// Gets the <see cref="LogicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="LogicalWorkspace"/>.</value>
        LogicalWorkspace LogicalWorkspace { get; }

        /// <summary>
        /// Gets the <see cref="EventBroker"/>.
        /// </summary>
        /// <value>The <see cref="EventBroker"/>.</value>
        EventBroker EventBroker { get; }

        /// <summary>
        /// Gets the currently selected <see cref="ImageBox"/>
        /// </summary>
        /// <value>The currently selected <see cref="ImageBox"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        ImageBox SelectedImageBox { get; }

        /// <summary>
        /// Gets the currently selected <see cref="Tile"/>
        /// </summary>
        /// <value>The currently selected <see cref="Tile"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        Tile SelectedTile { get; }

        /// <summary>
        /// Gets the currently selected <see cref="PresentationImage"/>
        /// </summary>
        /// <value>The currently selected <see cref="PresentationImage"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        PresentationImage SelectedPresentationImage { get; }

        /// <summary>
        /// Gets the workspace's currently selected mappable modal tools.
        /// </summary>
        /// <value>The workspace's current selected mappable modal tools.</value>
        /// <remarks>
        /// A <i>Mappable modal tool</i> or <i>MMT</i> is a tool that when selected
        /// causes a mouse button to be mapped to the tool's function; an MMT that
        /// is already mapped to the same button becomes deselected.  Examples
        /// of MMTs in ClearCanvas include Window/Level, Stack, Zoom, Pan, etc.  This
        /// property gets an index that stores which mouse buttons are currently
        /// mapped to which MMT.
        /// </remarks>
        MouseToolMap MouseToolMap { get; }

        /// <summary>
        /// Gets the <see cref="CommandHistory"/>.
        /// </summary>
        /// <value>The <see cref="CommandHistory"/>.</value>
        CommandHistory CommandHistory { get; }
    }
}
