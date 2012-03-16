#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Rules engine for applying rules against DICOM files and performing actions.
    /// </summary>
    /// <remarks>
    /// The ServerRulesEngine encapsulates code to apply rules against DICOM file 
    /// objects.  It will load the rules from the persistent store, maintain them by type,
    /// and then can apply them against specific files.
    /// </remarks>
    /// <seealso cref="ActionContext"/>
    /// <example>
    /// Here is an example rule for routing all images with Modality set to CT to an AE
    /// Title CLEARCANVAS.
    /// <code>
    /// <rule id="CT Rule">
    ///   <condition expressionLanguage="dicom">
    ///     <equal test="$Modality" refValue="CT"/>
    ///   </condition>
    ///   <action>
    ///     <auto-route device="CLEARCANVAS"/>
    ///   </action>
    /// </rule>
    /// </code>
    /// </example>
    public class RulesEngine<TContext, TTypeEnum>
        where TContext : ActionContext
    {
        protected readonly List<TTypeEnum> _omitList = new List<TTypeEnum>();
        protected readonly List<TTypeEnum> _includeList = new List<TTypeEnum>();
        protected readonly Dictionary<TTypeEnum, RuleTypeCollection<TContext, TTypeEnum>> _typeList =
            new Dictionary<TTypeEnum, RuleTypeCollection<TContext, TTypeEnum>>();

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="RulesEngineStatistics"/> for the rules engine.
        /// </summary>
        public RulesEngineStatistics Statistics { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a specific {TTypeEnum} to be omitted from the rules engine.
        /// </summary>
        /// <remarks>
        /// This method can be called multiple times, however, the <see cref="AddIncludeType"/> method
        /// cannot be called if this method has already been called.  Note that this method must be 
        /// called before <see cref="Load"/> to have an affect.
        /// </remarks>
        /// <param name="type">The type to omit</param>
        public void AddOmittedType(TTypeEnum type)
        {
            if (_includeList.Count > 0)
                throw new ApplicationException("Include list already has values, cannot add ommitted type.");
            _omitList.Add(type);
        }

        /// <summary>
        /// Limit the rules engine to only include specific Type types.
        /// </summary>
        /// <remarks>
        /// This methad can be called multiple times to include multiple types, however, the
        /// <see cref="AddOmittedType"/> method cannot be called if this method has already been 
        /// called.  Note that this method must be called before <see cref="Load"/> to have an affect.
        /// </remarks>
        /// <param name="type">The type to incude.</param>
        public void AddIncludeType(TTypeEnum type)
        {
            if (_omitList.Count > 0)
                throw new ApplicationException("Omitted list already has values, cannot add included type.");
            _includeList.Add(type);
        }


        /// <summary>
        /// Execute the rules against the context for the rules.
        /// </summary>
        /// <param name="context">A class containing the context for applying the rules.</param>
        public void Execute(TContext context)
        {
            Statistics.ExecutionTime.Start();

            foreach (RuleTypeCollection<TContext, TTypeEnum> typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context, false);
            }

            Statistics.ExecutionTime.End();
        }

        /// <summary>
        /// Execute the rules against the context for the rules.
        /// </summary>
        /// <param name="context">A class containing the context for applying the rules.</param>
        /// <param name="stopOnFirst">Stop on first valid rule of type.</param>
        public void Execute(TContext context, bool stopOnFirst)
        {
            Statistics.ExecutionTime.Start();

            foreach (RuleTypeCollection<TContext, TTypeEnum> typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context, stopOnFirst);
            }

            Statistics.ExecutionTime.End();
        }
        #endregion
    }
}