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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Helper class to create a list of <see cref="BaseImageLevelUpdateCommand"/> 
	/// that can be applied to a Dicom image.
	/// </summary>
	public class ImageUpdateCommandBuilder
	{

	    /// <summary>
		/// Builds a list of <see cref="BaseImageLevelUpdateCommand"/> for the specified study using the specified mapping template.
		/// </summary>
		/// <typeparam name="TMappingObject"></typeparam>
		/// <param name="storage"></param>
		/// <returns></returns>
		/// <remarks>
		/// This method generates a list of <see cref="BaseImageLevelUpdateCommand"/> based on the mapping in <see cref="TMappingObject"/>.
		/// <see cref="TMappingObject"/> specifies which Dicom fields the application is interested in, using <see cref="DicomFieldAttribute"/>.
		/// For example, if the application needs to update the study instance uid and study date in an image with what's in the database, 
		/// it will define the mapping class as:
		/// <code>
		/// class StudyInfoMapping 
		/// {
		///     [DicomField(DicomTags.StudyInstanceUid)]
		///     public String StudyInstanceUid{
		///         get{ ... }
		///         set{ ... }
		///     }
		/// 
		///     [DicomField(DicomTags.StudyDate)]
		///     public String StudyDate{
		///         get{ ... }
		///         set{ ... }
		///     }
		/// }
		/// 
		/// ImageUpdateCommandBuilder builder = new ImageUpdateCommandBuilder();
		/// IList<BaseImageLevelUpdateCommand> commandList = builder.BuildCommands<StudyInfoMapping>(studystorage);
		/// 
		/// DicomFile file = new DicomFile("file.dcm");
		/// foreach(BaseImageUpdateCommand command in commandList)
		/// {
		///     command.Apply(file);
		/// }
		/// 
		/// 
		/// </code>
		/// 
		/// </remarks>
		public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorage storage)
		{
			IList<StudyStorageLocation> storageLocationList = StudyStorageLocation.FindStorageLocations(storage);
			Debug.Assert(storageLocationList != null && storageLocationList.Count > 0);
			StudyStorageLocation storageLocation = storageLocationList[0];
			return BuildCommands<TMappingObject>(storageLocation);
		}

        public IList<BaseImageLevelUpdateCommand> BuildCommands<TTargetType>(IDicomAttributeProvider attributeProvider)
        {
            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
            EntityDicomMap fieldMap = EntityDicomMapManager.Get(typeof(TTargetType));
            foreach (DicomTag tag in fieldMap.Keys)
            {
	            DicomAttribute attribute;
                if (attributeProvider.TryGetAttribute(tag, out attribute))
                {
                    SetTagCommand cmd = new SetTagCommand(tag.TagValue, attribute.ToString());
                    commandList.Add(cmd);
                }
               
            }
			
            return commandList;
        }


		public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorageLocation storageLocation)
		{
			StudyXml studyXml = GetStudyXml(storageLocation);
            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();

            if (studyXml.NumberOfStudyRelatedInstances == 0)
            {
                // StudyXml is empty, resort to the db instead.
                Study study = storageLocation.LoadStudy(ExecutionContext.Current.PersistenceContext);
                commandList.AddRange(BuildCommandsFromEntity(study));
            }
		    else
            {
                commandList.AddRange(BuildCommandsFromStudyXml(typeof(TMappingObject), studyXml));
            }

			return commandList;
		}

		private static IList<BaseImageLevelUpdateCommand> BuildCommandsFromStudyXml(Type type, StudyXml studyXml)
		{
			List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
			EntityDicomMap fieldMap = EntityDicomMapManager.Get(type);
			XmlDocument studyXmlDoc = studyXml.GetMemento(new StudyXmlOutputSettings());
			foreach (DicomTag tag in fieldMap.Keys)
			{
				SetTagCommand cmd = new SetTagCommand(tag.TagValue, FindAttributeValue(tag, studyXmlDoc));
				commandList.Add(cmd);
			}
			return commandList;
		}

        private static IList<BaseImageLevelUpdateCommand> BuildCommandsFromEntity(ServerEntity entity)
        {
            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
            EntityDicomMap fieldMap = EntityDicomMapManager.Get(entity.GetType());

            foreach (DicomTag tag in fieldMap.Keys)
            {
                object value =fieldMap[tag].GetValue(entity, null);
                SetTagCommand cmd = new SetTagCommand(tag.TagValue, value!=null? value.ToString():null);
                commandList.Add(cmd);
            }
            return commandList;
        }


		private static StudyXml GetStudyXml(StudyStorageLocation storageLocation)
		{
			StudyXml studyXml = new StudyXml();
			string studyXmlPath = Path.Combine(storageLocation.GetStudyPath(), storageLocation.StudyInstanceUid + ".xml");
			using (Stream stream = FileStreamOpener.OpenForRead(studyXmlPath, FileMode.Open))
			{
				XmlDocument doc = new XmlDocument();
				StudyXmlIo.Read(doc, stream);
				studyXml.SetMemento(doc);
				stream.Close();
			}
			return studyXml;
		}

		private static String FindAttributeValue(DicomTag tag, XmlNode xnlRootNode)
		{
			String xpath = String.Format("//Attribute[@Tag='{0}']", tag.HexString);
			XmlNode node = xnlRootNode.SelectSingleNode(xpath);
			if (node == null)
				return null;
			else
				return XmlUtils.DecodeValue(node.InnerText);
		}

	}

	public class EntityDicomMap : Dictionary<DicomTag, PropertyInfo>
	{
	}

	public class EntityDicomMapManager : Dictionary<Type, EntityDicomMap>
	{
		private static readonly EntityDicomMapManager _instance = new EntityDicomMapManager();
		public static EntityDicomMapManager Instance
		{
			get { return _instance; }
		}


		static EntityDicomMapManager()
		{
			AddEntity(typeof(Study));
			AddEntity(typeof(Patient));
			AddEntity(typeof(StudyStorage));
		}

		private static void AddEntity(Type type)
		{
			PropertyInfo[] properties = type.GetProperties();
			EntityDicomMap entry = new EntityDicomMap();
			foreach (PropertyInfo prop in properties)
			{
				object[] attributes = prop.GetCustomAttributes(typeof(DicomFieldAttribute), true);
				foreach (DicomFieldAttribute attr in attributes)
				{
					DicomTag tag = attr.Tag;
					entry.Add(tag, prop);
				}
			}

			Instance.Add(type, entry);
		}

		public static EntityDicomMap Get(Type type)
		{
			if (Instance.ContainsKey(type))
				return Instance[type];
			else
			{
				AddEntity(type);
				return Instance[type];
			}
		}
	}
}