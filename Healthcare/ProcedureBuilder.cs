using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    [ExtensionPoint]
    public class ProcedureStepBuilderExtensionPoint : ExtensionPoint<IProcedureStepBuilder>
    {
    }

    public interface IProcedureStepBuilder
    {
        Type ProcedureStepClass { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="procedure"></param>
        /// <returns></returns>
        /// <remarks>
        /// The procedure is provided for reference only.  For example, the builder
        /// may need to create another object that refers to the procedure.  The
        /// builder is NOT responsible for adding the created <see cref="ProcedureStep"/>
        /// to the procedure.
        /// </remarks>
        ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure);
        void SavePrototype(ProcedureStep prototype, XmlElement xmlNode);
    }

    public abstract class ProcedureStepBuilderBase : IProcedureStepBuilder
    {
        #region IProcedureStepBuilder Members

        public abstract Type ProcedureStepClass { get; }

        public abstract ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure);

        public abstract void SavePrototype(ProcedureStep prototype, XmlElement xmlNode);

        #endregion

        protected string GetAttribute(XmlElement xmlNode, string attribute, bool required)
        {
            string value = xmlNode.GetAttribute(attribute);
            if(required && string.IsNullOrEmpty(value))
                throw new ProcedureBuilderException(string.Format("Required attribute '{0}' is missing.", attribute));
            return value;
        }
    }

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

    internal class ProcedureBuilder
    {

        #region Static Cache

        private static readonly Dictionary<Type, IProcedureStepBuilder> _mapClassToBuilder;

        static ProcedureBuilder()
        {
            _mapClassToBuilder = new Dictionary<Type, IProcedureStepBuilder>();
            foreach (IProcedureStepBuilder builder in (new ProcedureStepBuilderExtensionPoint().CreateExtensions()))
            {
                _mapClassToBuilder.Add(builder.ProcedureStepClass, builder);
            }
        }

        #endregion

        public ProcedureBuilder()
        {
        }

        internal void SetPlanFromPrototype(ProcedureType type, Procedure prototype)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement stepsNode = xmlDoc.CreateElement("procedure-steps");
            xmlDoc.AppendChild(stepsNode);
            foreach (ProcedureStep step in prototype.ProcedureSteps)
            {
                IProcedureStepBuilder builder = GetBuilderForClass(step.GetClass());
                XmlElement stepNode = xmlDoc.CreateElement("procedure-step");
                stepNode.SetAttribute("class", step.GetClass().FullName);
                builder.SavePrototype(step, stepNode);

                stepsNode.AppendChild(stepNode);
            }

            StringBuilder sb = new StringBuilder();
            using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb)))
            {
                writer.Formatting = System.Xml.Formatting.Indented;
                xmlDoc.Save(writer);
                type.PlanXml = sb.ToString();
            }
        }

        internal void BuildProcedure(Procedure procedure)
        {
            BuildProcedure(procedure, procedure.Type);
        }

        private void BuildProcedure(Procedure procedure, ProcedureType type)
        {
            // first apply the base procedure type recursively
            if(type.BaseType != null)
            {
                BuildProcedure(procedure, type.BaseType);
            }

            // plan may not exist
            if(string.IsNullOrEmpty(type.PlanXml))
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(type.PlanXml);

            XmlNodeList stepNodes = xmlDoc.SelectNodes("procedure-steps/procedure-step");
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
