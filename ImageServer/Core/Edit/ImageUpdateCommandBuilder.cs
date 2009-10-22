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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
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
		public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorage storage, IDicomAttributeProvider originalDicomAttributeProvider)
		{
			IList<StudyStorageLocation> storageLocationList = StudyStorageLocation.FindStorageLocations(storage);
			Debug.Assert(storageLocationList != null && storageLocationList.Count > 0);
			StudyStorageLocation storageLocation = storageLocationList[0];
            return BuildCommands<TMappingObject>(storageLocation, originalDicomAttributeProvider);
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
                    SetTagCommand cmd = new SetTagCommand(attribute, attribute.ToString());
                    commandList.Add(cmd);
                }
                else
                {
                    // tag doesn't exist, set to empty
                    SetTagCommand cmd = new SetTagCommand(attribute, string.Empty);
                    commandList.Add(cmd);
                }
            }
			
            return commandList;
        }

        public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorageLocation storageLocation)
        {
            return BuildCommands<TMappingObject>(storageLocation, null);
        }


        public IList<BaseImageLevelUpdateCommand> BuildCommands<TMappingObject>(StudyStorageLocation storageLocation, IDicomAttributeProvider originalDicomAttributeProvider)
		{
			StudyXml studyXml = GetStudyXml(storageLocation);
            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();

            if (studyXml.NumberOfStudyRelatedInstances == 0)
            {
                // StudyXml is empty, resort to the db instead.
                Study study = storageLocation.LoadStudy(ExecutionContext.Current.PersistenceContext);
                IList<BaseImageLevelUpdateCommand> cmds = BuildCommandsFromEntity(study, originalDicomAttributeProvider);

                // find the original values from originalDicomAttributeProvider 
                if (originalDicomAttributeProvider!=null)
                {
                    foreach (BaseImageLevelUpdateCommand cmd in cmds)
                    {
                        IUpdateImageTagCommand theCmd = cmd;
                        if (theCmd != null)
                        {
                            DicomAttribute attribute;
                            if (originalDicomAttributeProvider.TryGetAttribute(theCmd.UpdateEntry.TagPath.Tag, out attribute))
                            {
                                theCmd.UpdateEntry.OriginalValue = attribute.ToString();
                            }
                        }
                    }
                }
                
                commandList.AddRange(cmds);
            }
		    else
            {
                commandList.AddRange(BuildCommandsFromStudyXml(typeof(TMappingObject), studyXml, originalDicomAttributeProvider));
            }

			return commandList;
		}

        private static IList<BaseImageLevelUpdateCommand> BuildCommandsFromStudyXml(Type type, StudyXml studyXml, IDicomAttributeProvider originalDicomAttributeProvider)
		{
			List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
			EntityDicomMap fieldMap = EntityDicomMapManager.Get(type);
			//XmlDocument studyXmlDoc = studyXml.GetMemento(new StudyXmlOutputSettings());

			// Get the First InstanceXml of the first image
        	IEnumerator<SeriesXml> seriesEnumerator = studyXml.GetEnumerator();
        	seriesEnumerator.MoveNext();
        	SeriesXml seriesXml = seriesEnumerator.Current;
			IEnumerator<InstanceXml> instanceEnumerator = seriesXml.GetEnumerator();
			instanceEnumerator.MoveNext();
			InstanceXml instanceXml = instanceEnumerator.Current;

        	foreach (DicomTag tag in fieldMap.Keys)
			{
			    string originalValue = null;
				string newValue = null;
				DicomAttribute attribute;
                
				if (originalDicomAttributeProvider != null && originalDicomAttributeProvider.TryGetAttribute(tag, out attribute))
                    originalValue = attribute.ToString();
				
				if (instanceXml != null)
					attribute = instanceXml[tag];
				else
					attribute = null;
				if (attribute != null)
					newValue = attribute.ToString();
				SetTagCommand cmd = new SetTagCommand(tag.TagValue, originalValue, newValue);
				commandList.Add(cmd);
			}
			return commandList;
		}

        private static IList<BaseImageLevelUpdateCommand> BuildCommandsFromEntity(ServerEntity entity, IDicomAttributeProvider originalDicomAttributeProvider)
        {
            List<BaseImageLevelUpdateCommand> commandList = new List<BaseImageLevelUpdateCommand>();
            EntityDicomMap fieldMap = EntityDicomMapManager.Get(entity.GetType());

            foreach (DicomTag tag in fieldMap.Keys)
            {
                object value =fieldMap[tag].GetValue(entity, null);
                string originalValue = null;
                DicomAttribute attribute;
                if (originalDicomAttributeProvider.TryGetAttribute(tag, out attribute))
                    originalValue = attribute.ToString();
                
                SetTagCommand cmd = new SetTagCommand(tag.TagValue, originalValue, value != null ? value.ToString() : null);
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
	}

	public class EntityDicomMap : Dictionary<DicomTag, PropertyInfo>
	{
        /// <summary>
        /// Populate the corresponding Dicom property in the specified object with the given value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns>true if the target contains the field corresponding to the specified tag and it can be set</returns>
        public  bool Populate(object target, DicomTag tag, string value)
        {
            Platform.CheckForNullReference(target, "target");
            Platform.CheckForNullReference(tag, "tag");

        	PropertyInfo prop;
            if (!TryGetValue(tag, out prop))
                return false;

            if (!prop.CanWrite)
            {
                Platform.Log(LogLevel.Error, "Cannot set readonly property {0}.{1}.", target.GetType().Name, prop.Name);
                return false;
            }

            if (prop.PropertyType == typeof(string))
            {   
                this[tag].SetValue(target, value, null);
            }
            else if (prop.PropertyType == typeof(int) && prop.CanWrite)
            {
                if (value != null)
                {
                    int result;
                    if (int.TryParse(value, out result))
                        this[tag].SetValue(target, result, null);
                    else
                    {
                        Platform.Log(LogLevel.Warn, "Cannot populate {0}.{1} with {2}: Cannot convert String to int.", target.GetType().Name, prop.Name, value);
                        return false;
                    }
                }

            }
            else if (prop.PropertyType == typeof(bool))
            {
                if (value != null)
                {
                    bool result;
                    if (bool.TryParse(value, out result))
                        this[tag].SetValue(target, result, null);
                    else
                    {
                        Platform.Log(LogLevel.Warn, "Cannot populate {0}.{1} with {2}: Cannot convert String to boolean.", target.GetType().Name, prop.Name, value);
                        return false;
                    }
                }
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                if (value != null)
                {
                    DateTime result;
                    if (DateTime.TryParse(value, out result))
                        this[tag].SetValue(target, result, null);
                    else
                    {
                        Platform.Log(LogLevel.Warn, "Cannot populate {0}.{1} with {2}: Cannot convert String to DateTime.", target.GetType().Name, prop.Name, value);
                        return false;
                    }
                }
            }
            else if (prop.PropertyType == typeof(Decimal))
            {
                if (value != null)
                {
                    Decimal result;
                    if (Decimal.TryParse(value, out result))
                        this[tag].SetValue(target, result, null);
                    else
                    {
                        Platform.Log(LogLevel.Warn, "Cannot populate {0}.{1} with {2}: Cannot convert String to Decimal.", target.GetType().Name, prop.Name, value);
                        return false;
                    }
                }
            }
            else
            {
                Platform.Log(LogLevel.Warn, "Cannot populate {0}.{1} with {2}: convertion from String to {3} is not supported.", target.GetType().Name, prop.Name, value, prop.PropertyType);
                return false;
            }

            return true;
        }

	}

	public class EntityDicomMapManager : Dictionary<Type, EntityDicomMap>
	{
		private static readonly EntityDicomMapManager _instance = new EntityDicomMapManager();
		private static readonly object _syncLock = new object();

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
			lock (_syncLock)
			{
				if (Instance.ContainsKey(type))
					return Instance[type];

				AddEntity(type);
				return Instance[type];
			}
		}
	}
}