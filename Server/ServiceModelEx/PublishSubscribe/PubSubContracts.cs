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
using System.ServiceModel;
using System.Runtime.Serialization;

//For transient subscribers
[ServiceContract]
public interface ISubscriptionService
{
   [OperationContract]
   void Subscribe(string eventOperation);

   [OperationContract]
   void Unsubscribe(string eventOperation);
}
//For persistent subscribers
[DataContract]
public struct PersistentSubscription
{
   [DataMember]
   string m_Address;

   [DataMember]
   string m_EventsContract;

   [DataMember]
   string m_EventOperation;

   public string Address
   {
      get
      {
         return m_Address;
      }
      set
      {
         m_Address = value;
      }
   }
   public string EventsContract
   {
      get
      {
         return m_EventsContract;
      }
      set
      {
         m_EventsContract = value;
      }
   }
   public string EventOperation
   {
      get
      {
         return m_EventOperation;
      }
      set
      {
         m_EventOperation = value;
      }
   }
}

[ServiceContract]
public interface IPersistentSubscriptionService
{
   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   void PersistSubscribe(string address,string eventsContract,string eventOperation);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   void PersistUnsubscribe(string address,string eventsContract,string eventOperation);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   PersistentSubscription[] GetAllSubscribers();

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   PersistentSubscription[] GetSubscribersToContract(string eventsContract);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   string[] GetSubscribersToContractEventType(string eventsContract,string eventOperation);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   PersistentSubscription[] GetAllSubscribersFromAddress(string address);
}