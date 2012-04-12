using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;

namespace ClearCanvas.ImageViewer.Dicom.Core.Rules.AutoRoute
{
    /// <summary>
    /// Class for implementing auto-route action as specified by <see cref="IActionItem{T}"/>
    /// </summary>
    public class AutoRouteActionItem : ActionItemBase<ViewerActionContext>
    {
        readonly private string _device;
        private readonly DateTime? _startTime;
        private readonly DateTime? _endTime;
        #region Constructors

        public AutoRouteActionItem(string device)
            : base("AutoRoute Action")
        {
            _device = device;
        }

        public AutoRouteActionItem(string device, DateTime startTime, DateTime endTime)
            : base("AutoRoute Action")
        {
            _device = device;
            _startTime = startTime;
            _endTime = endTime;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(ViewerActionContext context)
        {
            InsertAutoRouteCommand command;

            if (_startTime != null && _endTime != null)
            {
                DateTime now = Platform.Time;
                TimeSpan nowTimeOfDay = now.TimeOfDay;
                if (_startTime.Value > _endTime.Value)
                {
                    if (nowTimeOfDay > _startTime.Value.TimeOfDay
                        || nowTimeOfDay < _endTime.Value.TimeOfDay)
                    {
                        command = new InsertAutoRouteCommand(context, _device);
                    }
                    else
                    {
                        DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
                        command = new InsertAutoRouteCommand(context, _device, scheduledTime);
                    }
                }
                else
                {
                    if (nowTimeOfDay > _startTime.Value.TimeOfDay
                        && nowTimeOfDay < _endTime.Value.TimeOfDay)
                    {
                        command = new InsertAutoRouteCommand(context, _device);
                    }
                    else
                    {
                        if (nowTimeOfDay < _startTime.Value.TimeOfDay)
                        {
                            DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
                            command = new InsertAutoRouteCommand(context, _device, scheduledTime);
                        }
                        else
                        {
                            DateTime scheduledTime = now.Date.Date.AddDays(1d).Add(_startTime.Value.TimeOfDay);
                            command = new InsertAutoRouteCommand(context, _device, scheduledTime);
                        }
                    }
                }
            }
            else
                command = new InsertAutoRouteCommand(context, _device);

            if (context.CommandProcessor != null)
                context.CommandProcessor.AddCommand(command);
            else
            {
                try
                {
                    command.Execute(context.CommandProcessor);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when inserting auto-route request");

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
