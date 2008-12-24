using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Defines the interface of a compiler to generate 
    /// <see cref="BaseImageLevelUpdateCommand"/> from a XML specification.
    /// </summary>
    public interface IWebEditStudyCommandCompiler
    {
        /// <summary>
        /// Name of the command to be generated
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Generates the <see cref="BaseImageLevelUpdateCommand"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        BaseImageLevelUpdateCommand Compile(XmlReader reader);
    }
}