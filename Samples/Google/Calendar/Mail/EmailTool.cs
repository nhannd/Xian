using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Net.Mail;

namespace ClearCanvas.Samples.Google.Calendar.Mail
{
    /// <summary>
    /// EmailTool allows a user to select an appointment from the appointment list and send a
    /// notification about the appointment via e-mail.
    /// </summary>
    [MenuAction("apply", "scheduling-appointments-contextmenu/MenuSendEmail", "Apply")]
    [ButtonAction("apply", "scheduling-appointments-toolbar/MenuSendEmail", "Apply")]
    [Tooltip("apply", "EmailDialogTitle")]
    [IconSet("apply", IconScheme.Colour, "Icons.EmailToolSmall.png", "Icons.EmailToolMedium.png", "Icons.EmailToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(SchedulingToolExtensionPoint))]
    public class EmailTool : Tool<ISchedulingToolContext>
    {

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public EmailTool()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Called to determine whether this tool is enabled/disabled in the UI.
        /// </summary>
        public bool Enabled
        {
            get { return this.Context.SelectedAppointment != null; }
        }

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { this.Context.SelectedAppointmentChanged += value; }
            remove { this.Context.SelectedAppointmentChanged -= value; }
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            CalendarEvent appt = this.Context.SelectedAppointment;
            if (appt != null)
            {
                try
                {
                    EmailComponent component = new EmailComponent();
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow,
                        component, SR.EmailDialogTitle);

                    if (exitCode == ApplicationComponentExitCode.Normal)
                    {
                        // create a background task to send the mail, so that we can use a progress dialog
                        BackgroundTask sendMailTask = new BackgroundTask(
                            delegate(IBackgroundTaskContext context)
                            {
                                // set a message on the progress dialog
                                context.ReportProgress(new BackgroundTaskProgress(0, SR.SendingMailMessage));

                                // call helper method to send the email
                                SendMail(appt, component.EmailAddress);
                            }, false);

                        // Use marquee style since we have no way of knowing how long it will take
                        ProgressDialog.Show(sendMailTask, this.Context.DesktopWindow, true, ProgressBarStyle.Marquee);
                    }

                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }

        /// <summary>
        /// Helper method to send mail.
        /// </summary>
        /// <param name="appt"></param>
        /// <param name="toAddress"></param>
        private void SendMail(CalendarEvent appt, string toAddress)
        {
            MailMessage message = new MailMessage(MailSettings.Default.FromAddress, toAddress);
            message.Subject = appt.Title;
            message.Body = string.Format(SR.MailBodyMessage, Format.Date(appt.StartTime), appt.Description);

            SmtpClient client = new SmtpClient(MailSettings.Default.SmtpHost);
            client.Send(message);
        }
    }
}
