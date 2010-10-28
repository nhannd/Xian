#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;

namespace $rootnamespace$
{
    /// <summary>
    /// Provides operations to administer $fileinputname$ entities.
    /// </summary>
    [RisServiceProvider]
    [ServiceContract]
    public interface I$fileinputname$AdminService
    {
        /// <summary>
        /// Summary list of all items.
        /// </summary>
        [OperationContract]
        List$fileinputname$sResponse List$fileinputname$s(List$fileinputname$sRequest request);

        /// <summary>
        /// Loads details of specified itemfor editing.
        /// </summary>
        [OperationContract]
        Load$fileinputname$ForEditResponse Load$fileinputname$ForEdit(Load$fileinputname$ForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit an item.
        /// </summary>
        [OperationContract]
        Load$fileinputname$EditorFormDataResponse Load$fileinputname$EditorFormData(Load$fileinputname$EditorFormDataRequest request);

        /// <summary>
        /// Adds a new item.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        Add$fileinputname$Response Add$fileinputname$(Add$fileinputname$Request request);

        /// <summary>
        /// Updates an item.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        Update$fileinputname$Response Update$fileinputname$(Update$fileinputname$Request request);

    }
}
