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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    public sealed class ManifestVerification
    {
        #region Public Static Properties

        public static bool Valid
        {
            get
            {
                lock (SyncLock)
                {
                    if (!_verificationRun)
                    {
                        ManifestVerification verifier = new ManifestVerification();
                        string reason;
                        _verificationStatus = verifier.Verify(out reason);
                        if (!_verificationStatus)
                            Platform.Log(LogLevel.Info, "Application failed verification: {0}", reason);

                        _verificationRun = true;
                    }
                    return _verificationStatus;
                }
            }
        }

        #endregion Public Static Properties

        #region Private Members

        private static readonly object SyncLock = new object();
        private static bool _verificationRun;
        private static bool _verificationStatus;

        private ClearCanvasManifest _productManifest;
        private ClearCanvasManifest[] _manifests;
        private readonly List<ManifestFile> _ignoredDirectories = new List<ManifestFile>();
        private readonly Dictionary<string, string> _installedFileDictionary = new Dictionary<string,string>();
        private readonly Dictionary<string, ManifestFile> _manifestFileDictionary = new Dictionary<string, ManifestFile>();

        #endregion Private Members

        #region Constructors
        private ManifestVerification()
        {
        }
        #endregion Constructors

        #region Private Methods

        private bool Verify(out string reason)
        {
            try
            {
                LoadManifestFiles();

                ProcessManifestFileList();

                ScanDirectory(Platform.InstallDirectory);

                VerifyProduct();

                VerifyPackageManifests();

                VerifyFileList();

                VerifyChecksums();

                reason = string.Empty;
                return true;                
            }
            catch (Exception e)
            {
                reason = e.Message;
                return false;                
            }
        }


        private void LoadManifestFiles()
        {
            List<string> files = new List<string>();
            FileProcessor.Process(Platform.ManifestDirectory, null,
                                    delegate(string filePath, out bool cancel)
                                        {
                                            files.Add(filePath);
                                            cancel = false;
                                        }, true);

            List<ClearCanvasManifest> manifests = new List<ClearCanvasManifest>();

            bool foundManifest = false;

            foreach (string file in files)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);

                    XmlElement modulusNode =
                        (XmlElement) CollectionUtils.FirstElement(doc.GetElementsByTagName("Modulus"));
                    XmlElement exponentNode =
                        (XmlElement) CollectionUtils.FirstElement(doc.GetElementsByTagName("Exponent"));

                    if (!modulusNode.InnerText.Equals("gRuWfDdo9ZCQ9G4hIt6amEgr1DBPDltP0NoBQI/GpooT6hGh6UkFRpaYQMt4ijxbHI5UDGZvAU66Vjt+FUcRVJ4I8AAuhyI4mU2BIzsfpUsKjxJMNdh3fxjaAOjMc4s5pX0CUvCJuZEhY/+PY/aYnjX2a6wes3nGHTx/YJhh42M="))
                        throw new ApplicationException("Invalid public key in digitial signature.");
                    if (!exponentNode.InnerText.Equals("AQAB"))
                        throw new ApplicationException("Invalid public key in digitial signature.");

                    if (!ManifestSignature.VerifyXmlSignature(doc))
                        throw new ApplicationException("Manifest digital signature failed verification.");

                    ClearCanvasManifest manifest = ClearCanvasManifest.Deserialize(file);

                    if (manifest.ProductManifest != null)
                    {
                        if (string.IsNullOrEmpty(manifest.ProductManifest.Product.Version))
                        {
                            // The ClearCanvas.Common.ProductInformation class uses the 
                            // clearcanvas.common.dll version if a version is not contained in
                            // the ProductSettings.
                            foreach (ManifestFile manFile in manifest.ProductManifest.Files)
                                if (manFile.Filename.ToLower().Equals("common\\clearcanvas.common.dll"))
                                    manifest.ProductManifest.Product.Version = manFile.Version;                            
                        }
                        foundManifest = true;
                        if (!manifest.ProductManifest.Product.Manifest.Equals("Manifest.xml"))
                            throw new ApplicationException("Product manifest not named Manifest.xml");

                        _productManifest = manifest;

                        if (!manifest.ProductManifest.Product.Manifest.Equals(Path.GetFileName(file)))
                            throw new ApplicationException("Manifest name does not match manifest: " +
                                                           Path.GetFileName(file));
                    }
                    else if (manifest.PackageManifest != null)
                    {
                        if (!manifest.PackageManifest.Package.Manifest.Equals(Path.GetFileName(file)))
                            throw new ApplicationException("Package Manifest name does not match manifest: " +
                                                           Path.GetFileName(file));
                    }
        
                    manifests.Add(manifest);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unexpected problem parsing manifest: " + e.Message + " (" + Path.GetFileName(file) + ")");
                }
            }

            if (!foundManifest)
                throw new ApplicationException("Unable to find Manifest.xml");

            _manifests = manifests.ToArray();
        }

        private void ProcessManifestFileList()
        {
            foreach (ClearCanvasManifest manifest in _manifests)
            {
                List<ManifestFile> fileList = null;
                if (manifest.ProductManifest != null)
                    fileList = manifest.ProductManifest.Files;
                else if (manifest.PackageManifest != null)
                    fileList = manifest.PackageManifest.Files;

                if (fileList == null)
                    continue;

                foreach (ManifestFile file in fileList)
                {
                    if (!_manifestFileDictionary.ContainsKey(file.Filename))
                        _manifestFileDictionary.Add(file.Filename, file);

                    if (!file.Ignore) continue;

                    string path = Path.Combine(Platform.InstallDirectory, file.Filename);

                    if (Directory.Exists(path))
                    {
                        file.IsDirectory = true;
                        _ignoredDirectories.Add(file);
                    }               
                }
            }
        }

        private void ScanDirectory(string dir)
        {
            // Process Root Directory Files
            FileProcessor.Process(dir, null, delegate(string filePath, out bool cancel)
                                                 {
                                                     if (!CheckIgnored(filePath))
                                                     {
                                                         string relativePath =
                                                             filePath.Substring(Platform.InstallDirectory.Length);

                                                         _installedFileDictionary.Add(relativePath, filePath);
                                                     }

                                                     cancel = false;
                                                 }, false);

            foreach (string subDir in Directory.GetDirectories(dir))
            {
                if (!CheckIgnored(subDir))
                {
                    // Process Recursive Folders Directory Files
                    ScanDirectory(subDir);                   
                }
            }
        }

        private bool CheckIgnored(string path)
        {
            if (path.StartsWith(Platform.ManifestDirectory))
                return true;

            if (path.StartsWith(Platform.LogDirectory))
                return true;

            string relativePath = path.Substring(Platform.InstallDirectory.Length);

            foreach (ManifestFile dir in _ignoredDirectories)
            {
                if (relativePath.StartsWith(dir.Filename))
                    return true;
            }

            ManifestFile file;
            if (_manifestFileDictionary.TryGetValue(relativePath, out file))
            {
                if (file.Ignore)
                    return true;
            }

            return false;
        }

        private void VerifyProduct()
        {
            if (!ProductInformation.Component.Equals(_productManifest.ProductManifest.Product.Component))
                throw new ApplicationException(string.Format("Component Name does not match the manifest: {0}, {1}",
                                                             _productManifest.ProductManifest.Product.Component,
                                                             ProductInformation.Component));

            if (!ProductInformation.Edition.Equals(_productManifest.ProductManifest.Product.Edition))
                throw new ApplicationException(string.Format("Edition Name does not match the manifest: {0}, {1}",
                                                             _productManifest.ProductManifest.Product.Edition,
                                                             ProductInformation.Edition));

            if (!ProductInformation.Product.Equals(_productManifest.ProductManifest.Product.Name))
                throw new ApplicationException(string.Format("Product Name does not match the manifest: {0}, {1}",
                                                             _productManifest.ProductManifest.Product.Name,
                                                             ProductInformation.Product));

            if (!ProductInformation.Version.ToString().Equals(_productManifest.ProductManifest.Product.Version))
                throw new ApplicationException(string.Format("Product Version does not match the manifest: {0}, {1}",
                                                             _productManifest.ProductManifest.Product.Version,
                                                             ProductInformation.Version));

            if (!ProductInformation.VersionSuffix.Equals(_productManifest.ProductManifest.Product.Suffix))
                throw new ApplicationException(
                    string.Format("Product Version Suffix does not match the manifest: {0}, {1}",
                                  _productManifest.ProductManifest.Product.Suffix,
                                  ProductInformation.VersionSuffix));
        }

        private void VerifyPackageManifests()
        {
            foreach (ClearCanvasManifest manifest in _manifests)
            {
                if (manifest.PackageManifest == null) continue;
             
                if (!_productManifest.ProductManifest.Product.Name.Equals(manifest.PackageManifest.Package.Product.Name))
                    throw new ApplicationException("Product Version Suffix in Package manifes does not match the manifest: "
                                                   + manifest.PackageManifest.Package.Product.Suffix);

                if (!_productManifest.ProductManifest.Product.Version.Equals(manifest.PackageManifest.Package.Product.Version))
                    throw new ApplicationException("Product Version in Package manifes does not match the manifest: "
                                                   + manifest.PackageManifest.Package.Product.Version);

                if (!_productManifest.ProductManifest.Product.Suffix.Equals(manifest.PackageManifest.Package.Product.Suffix))
                    throw new ApplicationException("Product Version Suffix in Package manifes does not match the manifest: "
                                                   + manifest.PackageManifest.Package.Product.Suffix);
            }
        }

        private void VerifyFileList()
        {
            foreach (string relativePath in _installedFileDictionary.Keys)
            {
                ManifestFile file;
                if (!_manifestFileDictionary.TryGetValue(relativePath, out file))
                {
                    throw new AppDomainUnloadedException("File in distribution not in manifest: " + relativePath);
                }
            }

            foreach (ManifestFile file in _manifestFileDictionary.Values)
            {
                if (file.Ignore) continue;

                string absolutePath;
                if (!_installedFileDictionary.TryGetValue(file.Filename,out absolutePath))
                {
                    throw new AppDomainUnloadedException("Manifest file not in distribution: " + file.Filename);
                }
            }
        }

        private void VerifyChecksums()
        {
            foreach (ManifestFile file in _manifestFileDictionary.Values)
            {
                if (string.IsNullOrEmpty(file.Checksum)) continue;

                file.VerifyChecksum(Path.Combine(Platform.InstallDirectory, file.Filename));
            }
        }

        #endregion Private Members
    }
}
