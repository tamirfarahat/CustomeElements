using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace MyRealDWG
{
    class MyHost : HostApplicationServices
    {
        // we can use a list to store the files which could not be resolved
        // to make consequent searches faster in those cases
        private List<string> mUnresolved = new List<string>();

        public MyHost()
        {
            RuntimeSystem.Initialize(this, 1033);
        }

        private string SearchPath(
            string fileName)
        {
            // check if the file is already with full path
            if (System.IO.File.Exists(fileName))
                return fileName;

            // check application folder
            string applicationPath =
              Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
              );
            if (File.Exists(applicationPath + "\\" + fileName))
                return applicationPath + "\\" + fileName;

            // we can also check the Acad path
            string acad = Environment.GetEnvironmentVariable("ACAD");
            if (File.Exists(acad + "\\" + fileName))
                return acad + "\\" + fileName;

            // search folders in %PATH%
            string[] paths =
              Environment.GetEnvironmentVariable(
                "Path").Split(new char[] { ';' }
              );
            foreach (string path in paths)
            {
                // some folders end with \\, some don't
                string validatedPath
                  = Path.GetFullPath(path.TrimEnd('\\') + "\\" + fileName);
                if (File.Exists(validatedPath))
                    return validatedPath;
            }

            // check the Fonts folders
            string systemFonts =
              Environment.GetEnvironmentVariable(
                "SystemRoot"
              ) + "\\Fonts\\";
            if (File.Exists(systemFonts + fileName))
                return systemFonts + fileName;

            // if we installed fonts in our own folder  
            string rdwgFonts = applicationPath + "\\Fonts\\";
            if (File.Exists(rdwgFonts + fileName))
                return rdwgFonts + fileName;

            // we did not find the file :-(
            System.Windows.Forms.MessageBox.Show("The file '" + fileName + "' could not be found and therefore could cause RealDWG problems in its ability to correctly read this drawing", fileName);

            return "";
        }

        public override string FindFile(
            string fileName,
            Database database,
            FindFileHint hint)
        {
            if (!MyDebugger.isOverriden("FindFile", fileName))
                return MyDebugger.logReturn(""); 

            // avoid repeated search for unresolvable files
            // file names cannot contain the pipe character "|", that's why we use it
            if (mUnresolved.Contains(fileName + "|" + hint.ToString()))
                return MyDebugger.logReturn(""); 

            // add extension if not already part of the file name
            if (!fileName.Contains("."))
            {
                string extension;
                switch (hint)
                {
                    case FindFileHint.CompiledShapeFile:
                        extension = ".shx";
                        break;
                    case FindFileHint.TrueTypeFontFile:
                        extension = ".ttf";
                        break;
                    case FindFileHint.PatternFile:
                        extension = ".pat";
                        break;
                    case FindFileHint.ArxApplication:
                        extension = ".dbx";
                        break;
                    case FindFileHint.FontMapFile:
                        extension = ".fmp";
                        break;
                    case FindFileHint.XRefDrawing:
                        extension = ".dwg";
                        break;
                    // Fall through. These could have
                    // various extensions
                    case FindFileHint.FontFile:
                    case FindFileHint.EmbeddedImageFile:
                    default:
                        extension = "";
                        break;
                }

                fileName += extension;
            }

            // add it to the unresolved items if it could not be resolved
            string ret = SearchPath(fileName);
            if (ret == "")
                mUnresolved.Add(fileName + "|" + hint.ToString());

            return MyDebugger.logReturn(ret);
        }

        public override string GetPassword(
            string dwgName, 
            PasswordOptions options)
        {
            return MyDebugger.logReturn("GetPassword", base.GetPassword(dwgName, options));
        }

        public override string GetRemoteFile(
            System.Uri url, 
            bool ignoreCache)
        {
            return MyDebugger.logReturn("GetRemoteFile", base.GetRemoteFile(url, ignoreCache));
        }

        public override System.Uri GetUrl(
            string localFile)
        {
            return base.GetUrl(localFile);
        }

        public override bool IsUrl(
            string filePath)
        {
            return base.IsUrl(filePath);
        }

        public override void LoadApplication(
            string appName, 
            ApplicationLoadReasons why, 
            bool printIt, 
            bool asCmd)
        {
            base.LoadApplication(appName, why, printIt, asCmd);
        }

        public override void PutRemoteFile(
            System.Uri url, 
            string localFile)
        {
            base.PutRemoteFile(url, localFile);
        }

        public override string AlternateFontName
        {
            get
            {
                if (!MyDebugger.isOverriden("get_AlternateFontName", ""))
                    return MyDebugger.logReturn(base.AlternateFontName);

                // we return our own Alternate Font, which is installed as part of our application
                return MyDebugger.logReturn("txt.shx");
            }
        }

        public override string CompanyName
        {
            get
            {
                return MyDebugger.logReturn("CompanyName", base.CompanyName);
            }
        }

        public override string FontMapFileName
        {
            get
            {
                return MyDebugger.logReturn("FontMapFileName", base.FontMapFileName);
            }
        }

        public override string LocalRootFolder
        {
            get
            {
                return MyDebugger.logReturn("LocalRootFolder", base.LocalRootFolder);
            }
        }

        public override ModelerFlavor ModelerFlavor
        {
            get
            {
                ModelerFlavor flavour = base.ModelerFlavor;
                return flavour;
            }
        }

        public override string Product
        {
            get
            {
                return MyDebugger.logReturn("Product", base.Product);
            }
        }

        public override string Program
        {
            get
            {
                return MyDebugger.logReturn("Program", base.Program);
            }
        }

        public override string MachineRegistryProductRootKey
        {
            get
            {
                if (!MyDebugger.isOverriden("get_MachineRegistryProductRootKey", ""))
                    return MyDebugger.logReturn(base.MachineRegistryProductRootKey);

                // this should be the same as the value of ODBXHOSTAPPREGROOT in our msi file
                return MyDebugger.logReturn(@"Software\MyRealDWG\1.0");
            }
        }

        public override string UserRegistryProductRootKey
        {
            get
            {
                if (!MyDebugger.isOverriden("get_UserRegistryProductRootKey", ""))
                    return MyDebugger.logReturn(base.UserRegistryProductRootKey);

                // this should be the same as the value of ODBXHOSTAPPREGROOT in our msi file
                return MyDebugger.logReturn(@"Software\MyRealDWG\1.0");
            }
        }

        public override string RoamableRootFolder
        {
            get
            {
                return MyDebugger.logReturn("RoamableRootFolder", base.RoamableRootFolder);
            }
        }
    }
}