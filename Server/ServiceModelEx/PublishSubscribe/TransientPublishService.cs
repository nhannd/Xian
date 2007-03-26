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