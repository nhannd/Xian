namespace ClearCanvas.Utilities.RebuildDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using ClearCanvas.DataStore;

    public class DatabaseRebuilder
    {
        public event EventHandler<ImageInsertCompletingEventArgs> ImageInsertCompletingEvent;
        public event EventHandler<ImageInsertCompletedEventArgs> ImageInsertCompletedEvent;
        public event EventHandler<DatabaseRebuildCompletedEventArgs> DatabaseRebuildCompletedEvent;
       
        public DatabaseRebuilder(String connectionString, String imageStoragePath, Boolean isSearchRecursive)
        {
            _connectionString = connectionString;
            _imageStoragePath = imageStoragePath;
            _isSearchRecursive = isSearchRecursive;
            _fileList = Directory.GetFiles(_imageStoragePath,
                "*",
                _isSearchRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public String ConnectionString
        {
            get { return _connectionString; }
            private set { _connectionString = value; }
        }

        public String ImageStoragePath
        {
            get { return _imageStoragePath; }
            private set { _imageStoragePath = value; }
        }

        public Boolean IsSearchRecursive
        {
            get { return _isSearchRecursive; }
            private set { _isSearchRecursive = value; }
        }

        public Int32 NumberOfFiles
        {
            get { return _fileList.Length; }
        }

        public void StartRebuild()
        {
            DatabaseConnector connector = new DatabaseConnector(new ConnectionString(_connectionString));
            connector.SetupConnector();
            foreach (String file in _fileList)
            {
                OnImageInsertCompletingEvent(new ImageInsertCompletingEventArgs(file));
                connector.InsertSopInstance(file);
                OnImageInsertCompletedEvent(new ImageInsertCompletedEventArgs(file));

            }
            connector.TeardownConnector();
            OnDatabaseRebuildCompletedEvent(new DatabaseRebuildCompletedEventArgs());
        }

        public void StopRebuild()
        {

        }

        protected void OnImageInsertCompletedEvent(ImageInsertCompletedEventArgs args)
        {
            ClearCanvas.Common.EventsHelper.Fire(ImageInsertCompletedEvent, this, args);
        }

        protected void OnImageInsertCompletingEvent(ImageInsertCompletingEventArgs args)
        {
            ClearCanvas.Common.EventsHelper.Fire(ImageInsertCompletingEvent, this, args);
        }

        protected void OnDatabaseRebuildCompletedEvent(DatabaseRebuildCompletedEventArgs args)
        {
            ClearCanvas.Common.EventsHelper.Fire(DatabaseRebuildCompletedEvent, this, args);
        }

        private DatabaseRebuilder() { }

        private String _connectionString;
        private String _imageStoragePath;
        private Boolean _isSearchRecursive;
        private String[] _fileList;
    }
}
