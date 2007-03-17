//2006 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel.Channels;

namespace ServiceModelEx
{
	public abstract class TransientSubscriptionManager<T> where T : class
   {
      static Dictionary<string,List<T>> m_TransientStore;

      static TransientSubscriptionManager()
      {
         m_TransientStore = new Dictionary<string, List<T>>();
         string[] methods = GetOperations();
         Action<string> insert = delegate(string methodName)
                                 {
                                    m_TransientStore.Add(methodName,new List<T>());
                                 };
         Array.ForEach(methods,insert);
      }
      
      //Helper methods 
      static void VerifyAddress(string address)
      {
         if(address.StartsWith("http:") || address.StartsWith("https:"))
         {
            return;
         }
         if(address.StartsWith("net.tcp:"))
         {
            return;
         }
         if(address.StartsWith("net.pipe:"))
         {
            return;
         }
         if(address.StartsWith("net.msmq:"))
         {
            return;
         }
         throw new InvalidOperationException("Unsupported protocol specified");
      }

      static string[] GetOperations()
      {
         MethodInfo[] methods = typeof(T).GetMethods(BindingFlags.Public|BindingFlags.FlattenHierarchy|BindingFlags.Instance);
         List<string> operations = new List<string>(methods.Length);

         Action<MethodInfo> add =  delegate(MethodInfo method)
                                   {
                                      Debug.Assert(! operations.Contains(method.Name));
                                      operations.Add(method.Name);
                                   };
         Array.ForEach(methods,add);
         return operations.ToArray();
      }

      //Transient subscriptions management 
      internal static T[] GetTransientList(string eventOperation)
      {
         lock(typeof(SubscriptionManager<T>))
         {
            List<T> list = m_TransientStore[eventOperation];
            return list.ToArray();
         }
      }
      static void AddTransient(T subscriber,string eventOperation)
      {
         lock(typeof(SubscriptionManager<T>))
         {
            List<T> list = m_TransientStore[eventOperation];
            if(list.Contains(subscriber))
            {
               return;
            }
            list.Add(subscriber);
         }
      }
      static void RemoveTransient(T subscriber,string eventOperation)
      {
         lock(typeof(SubscriptionManager<T>))
         {
            List<T> list = m_TransientStore[eventOperation];
            list.Remove(subscriber);
         }
      }

      public virtual void Subscribe(string eventOperation)
      {
         lock(typeof(SubscriptionManager<T>))
         {
            T subscriber = OperationContext.Current.GetCallbackChannel<T>();
            if(String.IsNullOrEmpty(eventOperation) == false)
            {
               AddTransient(subscriber,eventOperation);
            }
            else
            {
               string[] methods = GetOperations();
               Action<string> addTransient = delegate(string methodName)
                                             {
                                                AddTransient(subscriber,methodName);
                                             };
               Array.ForEach(methods,addTransient);
            }
         }
      }
      
      public virtual void Unsubscribe(string eventOperation)
      {
         lock(typeof(SubscriptionManager<T>))
         {
            T subscriber = OperationContext.Current.GetCallbackChannel<T>();
            if(String.IsNullOrEmpty(eventOperation) == false)
            {
               RemoveTransient(subscriber,eventOperation);
            }
            else
            {
               string[] methods = GetOperations();
               Action<string> removeTransient = delegate(string methodName)
                                                {
                                                   RemoveTransient(subscriber,methodName);
                                                };
               Array.ForEach(methods,removeTransient);
            }
         }
      }
	}
}