using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class PatientAddEditToolBase : ToolBase
    {
        protected IWorkspace OpenPatient(string title, PatientProfile patient)
        {
            PatientEditorShellComponent editor = new PatientEditorShellComponent(patient);
            return ApplicationComponent.LaunchAsWorkspace(
                this.DesktopWindow, editor, title, PatientEditorExited);
        }

        private void PatientEditorExited(IApplicationComponent component)
        {
            PatientEditorShellComponent editor = (PatientEditorShellComponent)component;
            EditorClosed(editor.Subject, editor.ExitCode);
        }

        protected abstract IDesktopWindow DesktopWindow { get; }

        protected abstract void EditorClosed(PatientProfile patient, ApplicationComponentExitCode exitCode);
    }
}
