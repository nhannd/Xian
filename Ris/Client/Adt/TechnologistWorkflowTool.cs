using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.Collections;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class TechnologistWorkflowTool : Tool<ITechnologistWorkflowItemToolContext>, IDropHandler<ModalityWorklistItem>
    {
        protected string _operationName;

        public TechnologistWorkflowTool(string OperationName)
        {
            _operationName = OperationName;
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Context.GetWorkflowOperationEnablement(_operationName);
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectedItemsChanged += value; }
            remove { this.Context.SelectedItemsChanged -= value; }
        }

        public virtual void Apply()
        {
            ModalityWorklistItem item = CollectionUtils.FirstElement<ModalityWorklistItem>(this.Context.SelectedItems);
            bool success = Execute(item, this.Context.DesktopWindow, this.Context.Folders);
            if (success)
            {
                this.Context.SelectedFolder.Refresh();
            }
        }

        public string OperationName
        {
            get { return _operationName; }
        }

        protected abstract bool Execute(ModalityWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders);

        #region IDropHandler<ModalityWorklistItem> Members

        public virtual bool CanAcceptDrop(IDropContext dropContext, ICollection<ModalityWorklistItem> items)
        {
            ITechnologistWorkflowFolderDropContext ctxt = (ITechnologistWorkflowFolderDropContext)dropContext;
            return ctxt.GetOperationEnablement(this.OperationName);
        }

        public virtual bool ProcessDrop(IDropContext dropContext, ICollection<ModalityWorklistItem> items)
        {
            ITechnologistWorkflowFolderDropContext ctxt = (ITechnologistWorkflowFolderDropContext)dropContext;
            ModalityWorklistItem item = CollectionUtils.FirstElement<ModalityWorklistItem>(items);
            bool success = Execute(item, ctxt.DesktopWindow, ctxt.FolderSystem.Folders);
            if (success)
            {
                ctxt.FolderSystem.SelectedFolder.Refresh();
                return true;
            }
            return false;
        }

        #endregion
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Taunt")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Taunt")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.CheckedInTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class TauntTool : TechnologistWorkflowTool
    {
        public TauntTool()
            : base("CheckInProcedure")
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
        {
            Platform.ShowMessageBox("You're a tool!");
            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Start")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Start")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.CheckedInTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class StartTool : TechnologistWorkflowTool
    {
        public StartTool()
            : base("StartProcedure")
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.StartProcedure(new StartProcedureRequest(item.ProcedureStepRef));
                    });

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Complete")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Complete")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.CheckedInTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class CompleteTool : TechnologistWorkflowTool
    {
        public CompleteTool()
            : base("CompleteProcedure")
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.CompleteProcedure(new CompleteProcedureRequest(item.ProcedureStepRef));
                    });

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.CheckedInTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class CancelTool : TechnologistWorkflowTool
    {
        public CancelTool()
            : base("CancelProcedure")
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.CancelProcedure(new CancelProcedureRequest(item.ProcedureStepRef));
                    });

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Suspend")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Suspend")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(Folders.CheckedInTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class SuspendTool : TechnologistWorkflowTool
    {
        public SuspendTool()
            : base("SuspendProcedure")
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.SuspendProcedure(new SuspendProcedureRequest(item.ProcedureStepRef));
                    });

                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }
}
