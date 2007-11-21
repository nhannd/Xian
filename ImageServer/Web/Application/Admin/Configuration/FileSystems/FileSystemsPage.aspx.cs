using System;
using ClearCanvas.ImageServer.Model;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Common.Data;


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems
{
    /// <summary>
    /// FileSystems Configuration Web Page.
    /// </summary>
    public partial class FileSystemsPage : System.Web.UI.Page
    {
        #region Private members
        
        // the controller used for database interaction
        private FileSystemsConfigurationController _controller = new FileSystemsConfigurationController();

        #endregion Private members

        #region Protected methods

        /// <summary>
        /// Set up the event handlers for child controls.
        /// </summary>
        protected void SetupEventHandlers()
        {
            FileSystemsPanel1.AddFileSystemDelegate = delegate
                                                          {
                                                              AddEditFileSystemDialog1.EditMode = false;
                                                              AddEditFileSystemDialog1.Show();
                                                          };

            FileSystemsPanel1.EditFileSystemDelegate = delegate(FileSystemsConfigurationController controller, Filesystem fs)
                                                          {
                                                              AddEditFileSystemDialog1.EditMode = true;
                                                              AddEditFileSystemDialog1.FileSystem = fs;
                                                              AddEditFileSystemDialog1.Show();
                                                          };
            AddEditFileSystemDialog1.OKClicked += delegate(Filesystem fs)
                                               {
                                                   if (AddEditFileSystemDialog1.EditMode)
                                                   {
                                                       // Commit the new FileSystems into database
                                                       if (_controller.UpdateFileSystem(fs))
                                                       {
                                                           FileSystemsPanel1.UpdateUI();
                                                       }  
                                                   }
                                                   else
                                                   {
                                                       // Commit the new FileSystems into database
                                                       if (_controller.AddFileSystem(fs))
                                                       {
                                                           FileSystemsPanel1.UpdateUI();
                                                       }  
                                                   }
                                                   
                                               };

            
        }

        

        /// <summary>
        /// Retrieves the Filesystems to be rendered in the page.
        /// </summary>
        /// <returns></returns>
        private IList<Filesystem> GetFilesystems()
        {
            // TODO We may want to add context or user preference here to specify which partitions to load

            IList<Filesystem> list = _controller.GetAllFileSystems();
            return list;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _controller = new FileSystemsConfigurationController();

            SetupControls();
            SetupEventHandlers();
        }

        protected void SetupControls()
        {
            FileSystemsPanel1.FileSystems = GetFilesystems();
            AddEditFileSystemDialog1.FilesystemTiers = _controller.GetFileSystemTiers();
                                                              
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        #endregion  Protected methods
        
    }
}
