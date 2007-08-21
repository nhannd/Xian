using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using System.Net.Mail;
using ClearCanvas.Desktop;

namespace ClearCanvas.Samples.Google.Calendar.Mail
{
    [MenuAction("mail", "scheduling-appointments-contextmenu/E-mail", "SendMail")]
    [ExtensionOf(typeof(SchedulingToolExtensionPoint))]
    public class AppointmentEmailTool : Tool<ISchedulingToolContext>
    {
        public void SendMail()
        {
            CalendarEvent appt = this.Context.SelectedAppointment;
            if (appt != null)
            {
                try
                {
                    EmailComponent component = new EmailComponent();
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow,
                        component, "E-mail Appointment Notification");

                    if (exitCode == ApplicationComponentExitCode.Normal)
                    {
                        MailMessage message = new MailMessage("demo@clearcanvas.ca", component.EmailAddress);
                        message.Subject = appt.Title;
                        message.Body = string.Format("Appointment scheduled for {0}: {1}", Format.Date(appt.StartTime), appt.Description);

                        SmtpClient client = new SmtpClient("mail.clearcanvas.ca");
                        client.Send(message);

                        this.Context.DesktopWindow.ShowMessageBox("Sent mail", MessageBoxActions.Ok);
                    }

                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }
    }
}
