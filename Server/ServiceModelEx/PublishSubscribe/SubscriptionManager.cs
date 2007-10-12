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
using System.ServiceModel.Channels;

namespace ServiceModelEx
{
   public abstract class SubscriptionManager<T> where T : class
   {
      static Dictionary<string,List<T>> m_TransientStore;

      static SubscriptionManager()
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
      static Binding GetBindingFromAddress(string address)
      {
         if(address.StartsWith("http:") || address.StartsWith("https:"))
         {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.Message,true);
            binding.ReliableSession.Enabled = true;
            binding.TransactionFlow = true;
            return binding;
         }
         if(address.StartsWith("net.tcp:"))
         {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Message,true);
            binding.ReliableSession.Enabled = true;
            binding.TransactionFlow = true;
            return binding;
         }
         if(address.StartsWith("net.pipe:"))
         {
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            binding.TransactionFlow = true;
            return binding;
         }
         if(address.StartsWith("net.msmq:"))
         {
            NetMsmqBinding binding = new NetMsmqBinding();
            binding.Security.Mode = NetMsmqSecurityMode.None; 
            return binding;
         }
         Debug.Assert(false,"Unsupported protocol specified");
         return null;
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

      public void Subscribe(string eventOperation)
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
      
      public void Unsubscribe(string eventOperation)
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
      
      //Persistent subscriptions management 
      static bool ContainsPersistent(string address,string eventsContract,string eventOperation)
      {
         string[] addresses = GetSubscribersToContractEventOperation(eventsContract,eventOperation);
         Predicate<string> exists = delegate(string addressToMatch)
                                    {
                                       return addressToMatch == address;
                                    };
         return Array.Exists(addresses,exists);
      }

      static void AddPersistent(string address,string eventsContract,string eventOperation)
      {
         bool exists = ContainsPersistent(address,eventsContract,eventOperation);
         if(exists)
         {
            return;
         }
         PersistentSubscribersTableAdapter adapter = new PersistentSubscribersTableAdapter();
         adapter.Insert(address,eventOperation,eventsContract);
      }
     
      static void RemovePersistent(string address,string eventsContract,string eventOperation)
      {
         PersistentSubscribersTableAdapter adapter = new PersistentSubscribersTableAdapter();

         PublishSubscribeDataSet.PersistentSubscribersDataTable subscribers = adapter.GetSubscribersByAddressContractOperation(address,eventsContract,eventOperation);
         foreach(PublishSubscribeDataSet.PersistentSubscribersRow subscriber in subscribers)
         {
            adapter.Delete(subscriber.Address,subscriber.Operation,subscriber.Contract,subscriber.ID);
         }
      }
      
      static PersistentSubscription[] Convert(PublishSubscribeDataSet.PersistentSubscribersDataTable subscribers)
      {
         Converter<PublishSubscribeDataSet.PersistentSubscribersRow,PersistentSubscription> converter;
         converter = delegate(PublishSubscribeDataSet.PersistentSubscribersRow row)
                     {
                        PersistentSubscription subscription = new PersistentSubscription();
                        subscription.Address = row.Address;
                        subscription.EventsContract = row.Contract;
                        subscription.EventOperation = row.Operation;
                        return subscription;
                     };
         if(subscribers.Rows.Count == 0)
         {
            return new PersistentSubscription[]{};
         }
         return Collection.UnsafeToArray(subscribers.Rows,converter);
      } 
      internal static T[] GetPersistentList(string eventOperation)
      {
         string[] addresses =  GetSubscribersToContractEventOperation(typeof(T).ToString(),eventOperation);

         List<T> subscribers = new List<T>(addresses.Length);

         foreach(string address in addresses)
         {
            Binding binding = GetBindingFromAddress(address);
            T proxy  = ChannelFactory<T>.CreateChannel(binding,new EndpointAddress(address));
            subscribers.Add(proxy);
         }
         return subscribers.ToArray();
      }
      
      static string[] GetSubscribersToContractEventOperation(string eventsContract,string eventOperation)
      {
         PublishSubscribeDataSet.PersistentSubscribersDataTable subscribers = new PublishSubscribeDataSet.PersistentSubscribersDataTable();
         PersistentSubscribersTableAdapter adapter = new PersistentSubscribersTableAdapter();
         subscribers = adapter.GetSubscribersToContractOperation(eventsContract,eventOperation);

         List<string> list = new List<string>();
         foreach(PublishSubscribeDataSet.PersistentSubscribersRow row in subscribers)
         {
            list.Add(row.Address);
         }
         return list.ToArray();
      }
     
      [OperationBehavior(TransactionScopeRequired = true)]   
      public PersistentSubscription[] GetAllSubscribers()
      {
         PublishSubscribeDataSet.PersistentSubscribersDataTable subscribers = new PublishSubscribeDataSet.PersistentSubscribersDataTable();
         PersistentSubscribersTableAdapter adapter = new PersistentSubscribersTableAdapter();
         subscribers = adapter.GetAllSubscribers();
         return Convert(subscribers);
      }
      [OperationBehavior(TransactionScopeRequired = true)]   
      public PersistentSubscription[] GetSubscribersToContract(string eventContract)
      {
         PublishSubscribeDataSet.PersistentSubscribersDataTable subscribers = new PublishSubscribeDataSet.PersistentSubscribersDataTable();
         PersistentSubscribersTableAdapter adapter = new PersistentSubscribersTableAdapter();
         subscribers = adapter.GetSubscribersToContract(eventContract);
         return Convert(subscribers);
      }
      [OperationBehavior(TransactionScopeRequired = true)]
      public string[] GetSubscribersToContractEventType(string eventsContract,string eventOperation)
      {
         return GetSubscribersToContractEventOperation(eventsContract,eventOperation);
      }
      [OperationBehavior(TransactionScopeRequired = true)]
      public PersistentSubscription[] GetAllSubscribersFromAddress(string address)
      {
         VerifyAddress(address);

         PublishSubscribeDataSet.PersistentSubscribersDataTable subscribers = new PublishSubscribeDataSet.PersistentSubscribersDataTable();

         PersistentSubscribersTableAdapter adapter = new PersistentSubscribersTableAdapter();
         subscribers = adapter.GetSubscribersFromAddress(address);

         return Convert(subscribers);
      }
      [OperationBehavior(TransactionScopeRequired = true)]
      public void PersistUnsubscribe(string address,string eventsContract,string eventOperation)
      {
         VerifyAddress(address);

         if(String.IsNullOrEmpty(eventOperation) == false)
         {
            RemovePersistent(address,eventsContract,eventOperation);
         }
         else
         {
            string[] methods = GetOperations();
            Action<string> removePersistent = delegate(string methodName)
                                              {
                                                 RemovePersistent(address,eventsContract,methodName);
                                              };
            Array.ForEach(methods,removePersistent);
         }
      }
      [OperationBehavior(TransactionScopeRequired = true)]
      public void PersistSubscribe(string address,string eventsContract,string eventOperation)
      {
         VerifyAddress(address);

         if(String.IsNullOrEmpty(eventOperation) == false)
         {
            AddPersistent(address,eventsContract,eventOperation);
         }
         else
         {
            string[] methods = GetOperations();
            Action<string> addPersistent = delegate(string methodName)
                                           {
                                              AddPersistent(address,eventsContract,methodName);
                                           };
            Array.ForEach(methods,addPersistent);
         }
      }     
   }
}