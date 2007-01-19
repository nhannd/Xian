using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Server.ShredHostClientUI.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ShredHostClientComponent"/>
    /// </summary>
    public partial class ShredHostClientComponentControl : ApplicationComponentUserControl
    {
        private ShredHostClientComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ShredHostClientComponentControl(ShredHostClientComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            _runningStateLabel.DataBindings.Add("Text", _component, "IsShredHostRunning", true, DataSourceUpdateMode.OnPropertyChanged);
            _shredCollectionTable.Table = _component.ShredCollection;
            _shredCollectionTable.SelectionChanged += new EventHandler(OnShredTableSelectionChanged);
            _shredCollectionTable.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            _shredCollectionTable.ToolbarModel = _component.ToolbarModel;
            _shredCollectionTable.MenuModel = _component.ContextMenuModel;

            _toggleButton.Click += delegate(object source, EventArgs args)
            {
                _component.Toggle();
            };
        }

        void OnShredTableSelectionChanged(object source, EventArgs args)
        {
            _component.TableSelection = _shredCollectionTable.Selection;
        }

    }
}
