#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Codec
{
    /// <summary>
    /// Registry of <see cref="IDicomCodecFactory"/> implementations that extend <see cref="DicomCodecFactoryExtensionPoint"/>.
    /// </summary>
    public static class DicomCodecRegistry
    {
        #region Private Members

    	private static readonly Dictionary<TransferSyntax, IDicomCodecFactory> _dictionary;

		#endregion

        #region Static Constructor
        
		static DicomCodecRegistry()
        {
			_dictionary = new Dictionary<TransferSyntax, IDicomCodecFactory>();

			try
			{
				DicomCodecFactoryExtensionPoint ep = new DicomCodecFactoryExtensionPoint();
				object[] codecFactories = ep.CreateExtensions();

				foreach (IDicomCodecFactory codecFactory in codecFactories)
					_dictionary[codecFactory.CodecTransferSyntax] = codecFactory;
			}
			catch(NotSupportedException)
			{
				Platform.Log(LogLevel.Info, "No dicom codec extension(s) exist.");
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "An error occurred while attempting to register the dicom codec extensions.");
			}
        }

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Gets the <see cref="TransferSyntax"/>es of the available <see cref="IDicomCodecFactory"/> implementations.
		/// </summary>
		public static TransferSyntax[] GetCodecTransferSyntaxes()
		{
			TransferSyntax[] syntaxes = new TransferSyntax[_dictionary.Count];
			_dictionary.Keys.CopyTo(syntaxes, 0);
			return syntaxes;
		}

    	/// <summary>
    	/// Gets an array of <see cref="IDicomCodec"/>s (one from each available <see cref="IDicomCodecFactory"/>).
    	/// </summary>
		public static IDicomCodec[] GetCodecs()
		{
			IDicomCodec[] codecs = new IDicomCodec[_dictionary.Count];
			int i = 0;
			foreach (IDicomCodecFactory factory in _dictionary.Values)
				codecs[i++] = factory.GetDicomCodec();

			return codecs;
		}

		/// <summary>
		/// Gets an array <see cref="IDicomCodecFactory"/> instances.
		/// </summary>
		/// <returns></returns>
		public static IDicomCodecFactory[] GetCodecFactories()
		{
			DicomCodecFactoryExtensionPoint ep = new DicomCodecFactoryExtensionPoint();
			object[] extensions = ep.CreateExtensions();
			IDicomCodecFactory[] codecFactories = new IDicomCodecFactory[extensions.Length];
			extensions.CopyTo(codecFactories, 0);
			return codecFactories;
		}
		
		/// <summary>
        /// Get a codec instance from the registry.
        /// </summary>
        /// <param name="syntax">The transfer syntax to get a codec for.</param>
        /// <returns>null if a codec has not been registered, an <see cref="IDicomCodec"/> instance otherwise.</returns>
        public static IDicomCodec GetCodec(TransferSyntax syntax)
        {
			IDicomCodecFactory factory;
            if (!_dictionary.TryGetValue(syntax, out factory))
                return null;

            return factory.GetDicomCodec();
        }

        /// <summary>
        /// Get default parameters for the codec.
        /// </summary>
        /// <param name="syntax">The transfer syntax to get the parameters for.</param>
        /// <param name="collection">The <see cref="DicomAttributeCollection"/> that the codec will work on.</param>
        /// <returns>null if no codec is registered, the parameters otherwise.</returns>
        public static DicomCodecParameters GetCodecParameters(TransferSyntax syntax, DicomAttributeCollection collection)
        {
			IDicomCodecFactory factory;
			if (!_dictionary.TryGetValue(syntax, out factory))
				return null;

            return factory.GetCodecParameters(collection);
        }
        #endregion
    }
}
