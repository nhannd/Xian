#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

/* ==============================================================================

 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 PARTICULAR PURPOSE.

 � 2005, LaMarvin. All Rights Reserved.
 ============================================================================== */

// Author: Palo Mraz
// Code taken from http://www.vbinfozine.com/a_deh.shtml

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{

  /// <summary>
  /// The class implements 
  /// </summary>
	public sealed class DelayedEventDispatcher
	{

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified
    /// event delegate.
    /// </summary>
    /// <param name="processEventDelegate">
    /// The delegate that actually raises or processes the delayed event.
    /// </param>
    public DelayedEventDispatcher(
      Delegate processEventDelegate) : this(processEventDelegate, 350, false)
    {
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified
    /// event delegate and the amount of milliseconds the event has to be postponed.
    /// </summary>
    /// <param name="processEventDelegate">
    /// The delegate that actually raises or processes the delayed event.
    /// </param>
    /// <param name="delayMilliseconds">
    /// The number of milliseconds the event has to be postponed.
    /// </param>
    public DelayedEventDispatcher(
      Delegate processEventDelegate,
      int delayMilliseconds) : this(processEventDelegate, delayMilliseconds, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified
    /// event delegate and the amount of milliseconds the event has to be postponed.
    /// </summary>
    /// <param name="processEventDelegate">
    /// The delegate that actually raises or processes the delayed event.
    /// </param>
    /// <param name="delayMilliseconds">
    /// The number of milliseconds the event has to be postponed.
    /// </param>
    /// <param name="delayMouseEvents">
    /// The flag indicates whether events should be delayed for events caused by mouse
    /// input.
    /// </param>
		public DelayedEventDispatcher(
      Delegate processEventDelegate,
      int delayMilliseconds, 
      bool delayMouseEvents)
		{
      // Validate the event delegate.
      if (processEventDelegate == null)
        throw new ArgumentNullException("processEventDelegate");
      
      ParameterInfo[] parameters = processEventDelegate.Method.GetParameters();
      if (parameters == null || parameters.Length != 2 || !typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
		  throw new ArgumentException(SR.ExceptionDelayedEventDelegateArgument, "processEventDelegate");

      if (delayMilliseconds < 0)
		  throw new ArgumentException(SR.ExceptionValueMustNotBeNegative, "delayMilliseconds");

      this._processEventDelegate = processEventDelegate;
      this._delay = TimeSpan.FromMilliseconds(delayMilliseconds);
      this._delayMouseEvents = delayMouseEvents;

      // Save the value to be passed to the GetAsyncKeyState API.
      if (SystemInformation.MouseButtonsSwapped)
        this._vkLeftButton = VK_RBUTTON;
      else
        this._vkLeftButton = VK_LBUTTON;
      
      this._timer = new Timer();
      this._timer.Interval = 20;
      this._timer.Tick += new EventHandler(_timer_Tick);
      this._timer.Enabled = true;
		}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="args">
    /// </param>
    public void RegisterAuthenticEvent(object sender, EventArgs args)
    {
      // Save the event state.
      this._recentEventArgs = args;
      this._recentSender = sender;
	  this._recentEventTime = Platform.Time;
      
      // If postponing is disabled, raise the event directly.
      if (!this.DelayEnabled)
      {
        this.HandleEvent();
        return;
      }

      // If the event was caused by a mouse, raise the event "almost" immediately (by pretending
      // the event occured at leat this._delay milliseconds in the past).
      // Please note: I'm saying almost because we're still using the timer to raise the
      // event on the next Tick (at most 20 milliseconds from now). This ensures that
      // a TreeView, for example, will display the new node in a selected state before 
      // the AfterClick event is raised.
      if (!this._delayMouseEvents && (GetAsyncKeyState(this._vkLeftButton) != 0))
		  this._recentEventTime = Platform.Time - this._delay - TimeSpan.FromMilliseconds(100);
    }


    /// <summary>
    /// Sets or returns the interval the associated event has to be delayed.
    /// </summary>
    public TimeSpan Delay
    {
      get
      {
        return this._delay;
      }
      set
      {
        this._delay = value;
      }
    }


    /// <summary>
    /// Sets or returns a flag indicating whether the delayed event handling
    /// feature is active.
    /// </summary>
    public bool DelayEnabled
    {
      get
      {
        return this._timer.Enabled;
      }
      set
      {
        this._timer.Enabled = value;
      }
    }


    /// <summary>
    /// Sets or returns a flag indicating whether the delayed event handling
    /// feature is active for events caused by mouse input.
    /// </summary>
    public bool DelayMouseEvents
    {
      get
      {
        return this._delayMouseEvents;
      }
      set
      {
        this._delayMouseEvents = value;
      }
    }


    /// <summary>
    /// Checks to see if an event to be raised is available and the required amount
    /// of time already passed and raises the event.
    /// </summary>
    private void _timer_Tick(object sender, EventArgs e)
    {
      if (this._recentEventArgs == null)
        return;

	TimeSpan elapsed = Platform.Time - this._recentEventTime;
      if (elapsed < this._delay)
        return;
    
      this.HandleEvent();
    }


    private void HandleEvent()
    {
      Debug.Assert(this._recentEventArgs != null);

      try
      {
        // Prepare the argument array and discard the recent event args BEFORE 
        // invoking the event delegate. Otherwise reentrant calls could occur if the
        // event handling code yields (for instance calls Application.DoEvents).
        // Thanks to Matt Stone [MAStone@osmh.on.ca] to pointing this out to me!
        object[] args = {this._recentSender, this._recentEventArgs};
        this._recentSender = this._recentEventArgs = null;

        // Now safely invoke the delegate.
        this._processEventDelegate.DynamicInvoke(args);
      }
      catch(Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      finally
      {
        // Discard the recent event args so the event won't be raised again.
        this._recentEventArgs = null;
        this._recentSender = null;
      }
    }


    [DllImport("user32")]
    private extern static short GetAsyncKeyState(int nVirtKey);
    
    private const int VK_LBUTTON = 0x01;
    private const int VK_RBUTTON = 0x02;
    private int _vkLeftButton;

    private EventArgs _recentEventArgs;
    private object _recentSender;
    private DateTime _recentEventTime;
    private Timer _timer;

    private bool _delayMouseEvents;

    private TimeSpan _delay;
    private Delegate _processEventDelegate;
  }
}
