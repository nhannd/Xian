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
        /// <value>The currently selected <see cref="ImageBox"/>, or <b>null</b> if 
		/// no <see cref="ImageBox"/> is currently selected.</value>
        ImageBox SelectedImageBox { get; }

        /// <summary>
        /// Gets the currently selected <see cref="Tile"/>
        /// </summary>
        /// <value>The currently selected <see cref="Tile"/>, or <b>null</b> if 
		/// no <see cref="Tile"/> is currently selected.</value>
        Tile SelectedTile { get; }

        /// <summary>
        /// Gets the currently selected <see cref="PresentationImage"/>
        /// </summary>
        /// <value>The currently selected <see cref="PresentationImage"/>, or <b>null</b> if 
		/// no <see cref="PresentationImage"/> is no currently selected.</value>
        PresentationImage SelectedPresentationImage { get; }

        /// <summary>
        /// Gets the <see cref="MouseButtonToolMap"/>
        /// </summary>
		/// <value>The <see cref="MouseButtonToolMap"/></value>
        /// <remarks>
        /// A <i>Mouse tool</i> is a tool that when selected
        /// causes a mouse button to be mapped to the tool's function; a mouse tool that
        /// is already mapped to the same button becomes deselected.  Examples
        /// of mouse tools in ClearCanvas include Window/Level, Stack, Zoom, Pan, etc.
        /// </remarks>
        MouseButtonToolMap MouseButtonToolMap { get; }

        /// <summary>
        /// Gets the <see cref="CommandHistory"/>.
        /// </summary>
        /// <value>The <see cref="CommandHistory"/>.</value>
        CommandHistory CommandHistory { get; }
    }
}
