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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Defines an extension point to provide implementations of <see cref="IProcedureStepBuilder"/>.
    /// </summary>
    [ExtensionPoint]
    public class ProcedureStepBuilderExtensionPoint : ExtensionPoint<IProcedureStepBuilder>
    {
    }

    /// <summary>
    /// Defines an interface to a procedure-step builder.  A procedure-step builder is an object
    /// that is responsible for instantiating a given class of procedure step from an XML plan.
    /// </summary>
    /// <remarks>
    /// Do not implement this interface directly. Instead use the abstract base class
    /// <see cref="ProcedureStepBuilderBase"/>.
    /// </remarks>
    public interface IProcedureStepBuilder
    {
        /// <summary>
        /// Gets the class of procedure step that this builder is responsible for.
        /// </summary>
        Type ProcedureStepClass { get; }

        /// <summary>
        /// Creates an instance of a procedure-step class from XML.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="procedure"></param>
        /// <returns></returns>
        /// <remarks>
        /// The procedure is provided for reference only.  For example, the builder
        /// may need to create another object that refers to the procedure.  The
        /// builder is NOT responsible for adding the created <see cref="ProcedureStep"/>
        /// to the procedure, and must NOT do so.
        /// </remarks>
        ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure);

        /// <summary>
        /// Creates an XML representation of the specified procedure-step prototype.
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="xmlNode"></param>
        void SaveInstance(ProcedureStep prototype, XmlElement xmlNode);
    }

    /// <summary>
    /// Abstract base implementation of <see cref="IProcedureStepBuilder"/>.
    /// </summary>
    public abstract class ProcedureStepBuilderBase : IProcedureStepBuilder
    {
        #region IProcedureStepBuilder Members

        /// <summary>
        /// Gets the class of procedure step that this builder is responsible for.
        /// </summary>
        public abstract Type ProcedureStepClass { get; }

        /// <summary>
        /// Creates an instance of a procedure-step class from XML.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="procedure"></param>
        /// <returns></returns>
        /// <remarks>
        /// The procedure is provided for reference only.  For example, the builder
        /// may need to create another object that refers to the procedure.  The
        /// builder is NOT responsible for adding the created <see cref="ProcedureStep"/>
        /// to the procedure, and must NOT do so.
        /// </remarks>
        public abstract ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure);

        /// <summary>
        /// Creates an XML representation of the specified procedure-step prototype.
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="xmlNode"></param>
        public abstract void SaveInstance(ProcedureStep prototype, XmlElement xmlNode);

        #endregion

        /// <summary>
        /// Utility method to get the value of an attribute from an XML node, optionally 
        /// throwing an exception if the attribute does not exist.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="attribute"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        protected static string GetAttribute(XmlElement xmlNode, string attribute, bool required)
        {
            string value = xmlNode.GetAttribute(attribute);
            if(required && string.IsNullOrEmpty(value))
                throw new ProcedureBuilderException(string.Format("Required attribute '{0}' is missing.", attribute));
            return value;
        }
    }

    #region ProcedureBuilderException class

    /// <summary>
    /// Defines an exception class for errors that occur in the <see cref="ProcedureBuilder"/>.
    /// </summary>
    public class ProcedureBuilderException: Exception
    {
        public ProcedureBuilderException(string message)
            :base(message)
        {
        }

        public ProcedureBuilderException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }

    #endregion

    /// <summary>
    /// Internal class that assembles <see cref="Procedure"/> objects according to a plan
    /// specified by a <see cref="ProcedureType"/>.
    /// </summary>
    internal class ProcedureBuilder
    {

        #region Static Cache

        /// <summary>
        /// Cache of <see cref="IProcedureStepBuilder"/> for each class of procedure step.
        /// </summary>
        private static readonly Dictionary<Type, IProcedureStepBuilder> _mapClassToBuilder;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ProcedureBuilder()
        {
            _mapClassToBuilder = new Dictionary<Type, IProcedureStepBuilder>();
            foreach (IProcedureStepBuilder builder in (new ProcedureStepBuilderExtensionPoint().CreateExtensions()))
            {
                _mapClassToBuilder.Add(builder.ProcedureStepClass, builder);
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds procedure steps to the specified <see cref="Procedure"/>,
        /// according to the plan specified by its <see cref="Procedure.Type"/> property.
        /// </summary>
        /// <remarks>
        /// This builds the specified procedure according to its <see cref="Procedure.Type"/>.
        /// It also takes into account any procedure-type inheritance, adding inherited procedure
        /// steps as well.
        /// </remarks>
        /// <param name="procedure"></param>
        internal void BuildProcedureFromPlan(Procedure procedure)
        {
            BuildProcedureFromPlan(procedure, procedure.Type);
        }

        /// <summary>
        /// Uses the specified <see cref="Procedure"/> as a prototype
        /// to create and save a plan in the <see cref="Procedure.Type"/> property.
        /// </summary>
        /// <remarks>
        /// This method generates the procedure plan XML by simply iterating over
        /// all procedure steps in the <see cref="Procedure.ProcedureSteps"/>
        /// property of the specified procedure.  It does not take procedure-plan
        /// inheritance into account. Therefore, it should not be considered an
        /// inverse of <see cref="BuildProcedureFromPlan(Procedure)"/>.
        /// </remarks>
        /// <param name="procedure"></param>
        internal XmlDocument CreatePlanFromProcedure(Procedure procedure)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement stepsNode = xmlDoc.CreateElement("procedure-steps");
            xmlDoc.AppendChild(stepsNode);
            foreach (ProcedureStep step in procedure.ProcedureSteps)
            {
                IProcedureStepBuilder builder = GetBuilderForClass(step.GetClass());
                XmlElement stepNode = xmlDoc.CreateElement("procedure-step");
                stepNode.SetAttribute("class", step.GetClass().FullName);
                builder.SaveInstance(step, stepNode);

                stepsNode.AppendChild(stepNode);
            }
            return xmlDoc;
        }

        #endregion

        private void BuildProcedureFromPlan(Procedure procedure, ProcedureType type)
        {
            // first apply the base procedure type recursively
            if(type.BaseType != null)
            {
                BuildProcedureFromPlan(procedure, type.BaseType);
            }

            // plan may not exist
            if(type.GetPlanXml().DocumentElement == null)
                return;

            XmlNodeList stepNodes = type.GetPlanXml().SelectNodes("procedure-plan/procedure-steps/procedure-step");
            foreach (XmlElement stepNode in stepNodes)
            {
                string className = stepNode.GetAttribute("class");
                if(string.IsNullOrEmpty(className))
                    throw new ProcedureBuilderException("Required attribute 'class' is missing.");

                Type stepClass = Type.GetType(className);
                if(stepClass == null)
                    throw new ProcedureBuilderException(string.Format("Unable to resolve class {0}.", className));

                IProcedureStepBuilder builder = GetBuilderForClass(stepClass);
                ProcedureStep step = builder.CreateInstance(stepNode, procedure);
				
				//Bug# 2505: do not create pre-steps for procedures in downtime recovery mode
				if (procedure.DowntimeRecoveryMode && step.IsPreStep)
					continue;
                
				procedure.AddProcedureStep(step);
            }
        }

        private static IProcedureStepBuilder GetBuilderForClass(Type stepClass)
        {
            IProcedureStepBuilder builder;
            if(_mapClassToBuilder.TryGetValue(stepClass, out builder))
                return builder;

            throw new ProcedureBuilderException(string.Format("No builder found for class {0}.", stepClass.FullName));
        }
    }
}
