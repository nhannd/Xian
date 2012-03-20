using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using DicomServerConfigurationContract = ClearCanvas.ImageViewer.Common.DicomServer.DicomServerConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    /*
    // TODO (Marmot): Not sure about all this. Seems complicated.
    internal partial class DicomServerConfiguration
    {
        partial class ChangePublisher
        {
            static ChangePublisher()
            {
                Instance = new ChangePublisher();
            }

            public static ChangePublisher Instance { get; private set; }
        }

        internal partial class ChangePublisher
        {
            private readonly object _syncLock = new object();
            private event EventHandler _configurationChanged;
            private int _listenerCount = 0;

            private bool _resetSignalPending = false;
            private int _signalSetTimeTicks;

            private EventWaitHandle _changeSignal;
            private const string _changeSignalName = "ClearCanvas_Workstation_DicomServer_Configuration";

            private Thread _publishingThread;

            public void SignalChanged()
            {
                lock (_syncLock)
                {
                    _signalSetTimeTicks = Environment.TickCount;
                    if (_resetSignalPending)
                        return;

                    if (_changeSignal == null)
                        _changeSignal = CreateWaitHandle(_changeSignalName);

                    _changeSignal.Set();
                    ThreadPool.QueueUserWorkItem(d => ResetSignal());
                }
            }

            private void ResetSignal()
            {
                lock (_syncLock)
                {
                    while (_resetSignalPending)
                    {
                        _resetSignalPending = false;
                        int waitTime = Environment.TickCount - _signalSetTimeTicks;
                        Monitor.Wait(_syncLock, waitTime);
                    }

                    _changeSignal.Reset();
                    if (_listenerCount != 0)
                        return;

                    _changeSignal.Close();
                    _changeSignal = null;
                }
            }

            public event EventHandler ConfigurationChanged
            {
                add
                {
                    lock (_syncLock)
                    {
                        ++_listenerCount;
                        _configurationChanged += value;

                        if (_listenerCount == 1)
                            StartListening();

                    }
                }
                remove
                {
                    lock (_syncLock)
                    {
                        --_listenerCount;
                        _configurationChanged -= value;

                        if (_listenerCount == 0)
                            StopListening();
                    }
                }
            }

            private void StopListening()
            {
                _changeSignal.Close();
                _changeSignal = null;

                Monitor.Pulse(_syncLock);
                _publishingThread.Join();
            }

            private void StartListening()
            {
                _changeSignal = CreateWaitHandle(_changeSignalName);
                _publishingThread = new Thread(Listen);
                _publishingThread.Start();
            }

            private void PublishEvent()
            {
                Platform.Log(LogLevel.Debug, "Change in DICOM server configuration detected.");

                Delegate[] listeners;
                lock (_syncLock)
                {
                    if (_listenerCount == 0)
                        return;

                    listeners = _configurationChanged.GetInvocationList();
                }

                foreach (var listener in listeners)
                {
                    try
                    {
                        listener.DynamicInvoke(null, EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Warn, e, "Failed to inform listener of DICOM server configuration change.");
                    }
                }
            }

            private void Listen(object ignore)
            {
                while (true)
                {
                    lock (_syncLock)
                    {
                        
                        if (_listenerCount == 0)
                            break;

                        Monitor.Wait(_syncLock, TimeSpan.FromMilliseconds(2000));
                    }

                    try
                    {
                        DicomServerConfigurationContract before = null;
                        Platform.GetService<IDicomServerConfiguration>(
                            s => before = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration);

                        _changeSignal.WaitOne();

                        DicomServerConfigurationContract after = null;

                        Platform.GetService<IDicomServerConfiguration>(
                            s => after = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration);

                        if (before.AETitle != after.AETitle || before.Port != after.Port)
                            PublishEvent();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Debug, e, "Error waiting for change signal mutex.");
                        throw;
                    }
                }
            }

            private static EventWaitHandle CreateWaitHandle(string name)
            {
                // TODO (CR Mar 2012): Move this somewhere shared.
                const bool initialState = true;
                try
                {
                    return EventWaitHandle.OpenExisting(name);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    var sec = new EventWaitHandleSecurity();
                    var rule = new EventWaitHandleAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        EventWaitHandleRights.FullControl, AccessControlType.Allow);
                    sec.AddAccessRule(rule);

                    bool created;
                    return new EventWaitHandle(initialState, EventResetMode.ManualReset, name, out created);
                }
                catch (UnauthorizedAccessException)
                {
                    // The named mutex exists, but the user does not have the security access required to use it.
                    try
                    {
                        var handle = EventWaitHandle.OpenExisting(name, EventWaitHandleRights.ReadPermissions | EventWaitHandleRights.ChangePermissions);

                        var sec = new EventWaitHandleSecurity();
                        var rule = new EventWaitHandleAccessRule(
                            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                            EventWaitHandleRights.FullControl, AccessControlType.Allow);
                        sec.AddAccessRule(rule);

                        // Update the ACL. This requires MutexRights.ChangePermissions.
                        handle.SetAccessControl(sec);
                        return EventWaitHandle.OpenExisting(name);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return new EventWaitHandle(initialState, EventResetMode.ManualReset, name);
                    }
                }
            }
        }
    }
     * */
}
