#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

//2006 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;
using ServiceModelEx.PublishSubscribeDataSetTableAdapters;
using System.Reflection;
using System.Threading;

namespace ServiceModelEx
{
   public abstract class TransientPublishService<T> where T : class
   {
      protected static void FireEvent(string eventName, params object[] args)
      {
		  //StackFrame method of doing things seems to return the wrong method name in Release Build.

		 //StackFrame stackFrame = new StackFrame(1);
		 //string methodName = stackFrame.GetMethod().Name;

         PublishTransient(eventName,args);
      }
      static void PublishTransient(string methodName,params object[] args)
      {
         T[] subscribers = TransientSubscriptionManager<T>.GetTransientList(methodName);
         Publish(subscribers,methodName,args);
      }
      static void Publish(T[] subscribers, string methodName,params object[] args)
      {
         WaitCallback fire = delegate(object subscriber)
                             {
                                Invoke(subscriber as T,methodName,args);
                             };
         Action<T> queueUp = delegate(T subscriber)
                             {
                                ThreadPool.QueueUserWorkItem(fire,subscriber);
                             };
         Array.ForEach(subscribers,queueUp);
      }
      static void Invoke(T subscriber,string methodName,object[] args)
      {
         Debug.Assert(subscriber != null);
         Type type = typeof(T);
         MethodInfo methodInfo = type.GetMethod(methodName);
         try
         {
            methodInfo.Invoke(subscriber,args);
         }
         catch(Exception e)
         {
            Trace.WriteLine(e.Message);
         }
      }
   }
}