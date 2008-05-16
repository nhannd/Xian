using System;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Rules
{

    /// <summary>
    /// Defines constants for time unit used in defining server rule actions
    /// </summary>
    public enum TimeUnit
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }

    

    /// <summary>
    /// Base class for all server rule actions implementing <see cref="IActionItem{ServerActionContext}"/> 
    /// </summary>
    public abstract class ServerActionItemBase: IActionItem<ServerActionContext>
    {
        #region Private Members
        private string _failureReason = "Success";
        private string _name;
        #endregion

        #region Constructors
        public ServerActionItemBase(string name)
        {
            Name = name;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the description of the failure when the action execution fails.
        /// </summary>
        public string FailureReason
        {
            get { return _failureReason; }
            set { _failureReason = value; }
        }

        /// <summary>
        /// Gets or sets the name of the action
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        /// <summary>
        /// Returns the time calculated off a dicom tag referenceValue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="offset"></param>
        /// <param name="unit"></param>
        /// <param name="referenceValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected DateTime? ResolveTime(ServerActionContext context, int offset, TimeUnit unit, ReferenceValue referenceValue, DateTime? defaultValue)
        {
            DateTime? time = defaultValue;

            if (referenceValue == null)
            {
                time = defaultValue;
            }
            else if (referenceValue.IsDicomTag)
            {
                time = referenceValue.GetDicomValue(defaultValue);
            }
            else
            {
                object rawValue = referenceValue.Value;
                if (rawValue == null)
                    time = defaultValue;
                else
                {
                    if (rawValue.GetType() == typeof(DateTime))
                        time = (DateTime)rawValue ;
                    else if (rawValue.GetType() == typeof(string))
                    {
                        DateTime temp;
                        if (DateTime.TryParse((string)rawValue, out temp))
                            time = temp;
                    }
                        
                }
            }

            if (time != null)
            {
                switch (unit)
                {
                    case TimeUnit.Minutes:
                        time = time.Value.AddMinutes(offset);
                        break;

                    case TimeUnit.Hours:
                        time = time.Value.AddHours(offset);
                        break;

                    case TimeUnit.Days:
                        time = time.Value.AddDays(offset);
                        break;

                    case TimeUnit.Weeks:
                        time = time.Value.AddDays(offset * 7);
                        break;

                    case TimeUnit.Months:
                        time = time.Value.AddMonths(offset);
                        break;

                    case TimeUnit.Years:
                        time = time.Value.AddYears(offset);
                        break;

                    default:
                        throw new ServerActionException(context, String.Format("Unexpected time units for {0} action item: {1}", Name, unit));
                }
            }


            return time;
        }

       

        
        #region IActionItem<ServerActionContext> Members

        public bool Execute(ServerActionContext context)
        {
            try
            {
                return OnExecute(context);
            }
            catch(Exception e)
            {
                FailureReason = String.Format("{0} {1}", e.Message, e.StackTrace);
                return false;
            }
            
        }

        protected abstract bool OnExecute(ServerActionContext context);

        #endregion
    }



    
}