using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Test.Common;

namespace WSClient
{
    public partial class Form1 : Form
    {
        ChannelFactory<ITestService> _testServiceFactory;

        public Form1()
        {
            InitializeComponent();

            _testServiceFactory = new ChannelFactory<ITestService>(
                new BasicHttpBinding(),
                new EndpointAddress("http://localhost:8000/ClearCanvas.Ris.Test.Common.ITestService/"));
        }

        private void _sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                ITestService testService = _testServiceFactory.CreateChannel();
                using (testService as IDisposable)
                {
                    _result.Text = testService.GetName(0);
                }
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine("The service operation timed out. " + timeProblem.Message);
            }
            catch (ItsMondayException realMonday)
            {
                Console.WriteLine(realMonday.Message);
            }
            catch (FaultException<ItsMondayException> mondayException)
            {
                Console.WriteLine(mondayException.Detail.Message);
            }
            catch (FaultException unknownFault)
            {
                Console.WriteLine("An unknown exception was received. " + unknownFault.Message);
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message + commProblem.StackTrace);
            }
        }
    }
}