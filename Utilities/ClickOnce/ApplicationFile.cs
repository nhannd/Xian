using System;
using System.ComponentModel;

namespace ClickOncePublisher
{
   /// <summary>
   /// Business entity class for the items in the application manifest file collection
   /// </summary>
   public class ApplicationFile : INotifyPropertyChanged, ISupportInitialize
   {
      #region Member variables
      protected bool m_IsDirty = false;
      protected bool m_Initialized = true;
      string m_FileName = string.Empty;
      string m_RelativePath = string.Empty;
      bool m_DataFile = false;
      bool m_EntryPoint = false;
      #endregion

      #region Public properties
      public bool IsDirty
      {
         get
         {
            return m_IsDirty;
         }
      }


      public string FileName
      {
         get
         {
            return m_FileName;
         }
         set
         {
            bool changed = CheckPropertyChanged(m_FileName, value);
            m_FileName = value;
            if (changed)
            {
               FirePropertyChanged("FileName");
            }
         }
      }

      public string RelativePath
      {
         get
         {
            return m_RelativePath;
         }
         set
         {
            bool changed = CheckPropertyChanged(m_RelativePath, value);
            m_RelativePath = value;
            if (changed)
            {
               FirePropertyChanged("RelativePath");
            }
         }
      }

      public bool DataFile
      {
         get
         {
            return m_DataFile;
         }
         set
         {
            bool changed = m_DataFile.Equals(value);
            m_DataFile = value;
            if (changed)
            {
               FirePropertyChanged("DataFile");
            }
         }
      }

      public bool EntryPoint
      {
         get
         {
            return m_EntryPoint;
         }
         set
         {
            bool changed = m_EntryPoint.Equals(value);
            m_EntryPoint = value;
            if (changed)
            {
               FirePropertyChanged("EntryPoint");
            }
         }
      }
      #endregion

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region ISupportInitialize Members

      public void BeginInit()
      {
         m_Initialized = false;
      }

      public void EndInit()
      {
         m_Initialized = true;
      }

      #endregion

      #region Helper Methods
      void FirePropertyChanged(string propName)
      {
         if (m_Initialized)
         {
            m_IsDirty = true;
            if (PropertyChanged != null)
            {
               PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
         }
      }

      protected virtual bool CheckPropertyChanged<T>(T member, T value) where T : class
      {
         if (member == null && value == null)
         {
            return false;
         }
         if (member != null)
         {
            return !member.Equals(value);
         }
         else if (value != null)
         {
            return !value.Equals(member);
         }
         else
         {
            return false;
         }
      }
      #endregion
   }
}
