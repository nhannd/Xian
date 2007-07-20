using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.WorklistTest;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("runTest", "global-menus/Test/Worklists")]
    [ClickHandler("runTest", "RunTest")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class WorklistTestTool : Tool<IDesktopToolContext>
    {
        public void RunTest()
        {
            Platform.GetService<IWorklistTestService>(
                delegate(IWorklistTestService service)
                {
                    GetAWorklistResponse worklistResponse = service.GetAWorklist(new GetAWorklistRequest());
                    DoWorklistTestResponse testResponse = service.DoWorklistTest(new DoWorklistTestRequest(worklistResponse.Summary.EntityRef));

                    this.Context.DesktopWindow.ShowMessageBox(testResponse.Message, MessageBoxActions.Ok);
                });
        }
    }
}