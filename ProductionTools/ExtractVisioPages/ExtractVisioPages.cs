using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Visio = Microsoft.Office.Interop.Visio;

namespace ExtractVisioPages
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Invalid number of arguments.");
                Usage();
                return 3;
            }

            FileInfo sourceFileInfo = new FileInfo(args[0]);
            if (!sourceFileInfo.Exists)
            {
                Console.Error.WriteLine("Source file \"{0}\"doesn't exist.", sourceFileInfo.FullName);
                Usage();
                return 3;
            }

            string[] validExtensions = new string[] { ".vst", ".vsd" };
            if (!validExtensions.Contains(sourceFileInfo.Extension, StringComparer.InvariantCultureIgnoreCase))
            {
                Console.Error.WriteLine(String.Format("Source file extension must be one of: {0}", String.Join(", ", validExtensions)));
                Usage();
                return 3;
            }

            string referencePageNames = args[1];

            FileInfo targetFileInfo = new FileInfo(args[2]);
            if (targetFileInfo.Exists && targetFileInfo.IsReadOnly)
            {
                Console.Error.WriteLine("Target file exists and is readonly.");
                Usage();
                return 3;
            }
            if (!targetFileInfo.Directory.Exists)
            {
                Console.Error.WriteLine("Target file directory doesn't exist.");
                Usage();
                return 3;
            }
            if (!validExtensions.Contains(targetFileInfo.Extension, StringComparer.InvariantCultureIgnoreCase))
            {
                Console.Error.WriteLine(String.Format("Target file extension must be one of: {0}", String.Join(", ", validExtensions)));
                Usage();
                return 3;
            }
            if (String.Compare(sourceFileInfo.FullName, targetFileInfo.FullName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                Console.Error.WriteLine("Source and targer file are identical.");
                Usage();
                return 3;
            }

            string targetFile = targetFileInfo.FullName;
            File.Copy(sourceFileInfo.FullName, targetFile, true);
            File.SetAttributes(targetFile, FileAttributes.Normal);
            //Visio.Application visioApp = new Visio.Application(); // Visible Application
            Visio.InvisibleApp visioApp = new Visio.InvisibleApp();
            Visio.Document visioTargetDocument = visioApp.Documents.OpenEx(targetFile, (short)Microsoft.Office.Interop.Visio.VisOpenSaveArgs.visAddMacrosDisabled);

            Array pageNameArray;
            visioTargetDocument.Pages.GetNames(out pageNameArray);
            IList<string> pageNames = pageNameArray.Cast<string>().ToList();
            foreach (string referencePageName in referencePageNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                bool referencePageContained = pageNames.Remove(referencePageName);
                if (!referencePageContained)
                {
                    visioApp.Quit();
                    File.Delete(targetFile);
                    Console.Error.WriteLine(String.Format("ReferencePageName \"{0}\" not found in \"{1}\".", referencePageName, sourceFileInfo.FullName));
                    Usage();
                    return 3;
                }
            }

            foreach (string pageName in pageNames)
            {
                Visio.Page page = visioTargetDocument.Pages[pageName];
                page.Delete(1);
            }

            visioTargetDocument.Save();

            if (referencePageNames != "Template")
            {
                foreach (Visio.Page page in visioTargetDocument.Pages)
                {
                    foreach (Visio.Shape shape in page.Shapes)
                    {
                        if (shape.get_CellExists("User.StateChartType", (short)0) != 0)
                        {
                            string stateChartType = shape.Cells["User.StateChartType"].FormulaU;
                            if (stateChartType == "GUARD(0)")
                            {
                                Console.WriteLine("Name={0} NameID={1} NameU={2}", shape.Name, shape.NameID, shape.NameU);

                                shape.get_Cells("Prop.TargetFile").FormulaU = "";

                                Console.WriteLine("Prop.TargetFile Formula={0} Value={1}", shape.get_Cells("Prop.TargetFile").FormulaU, shape.get_Cells("Prop.TargetFile").get_ResultStrU((short)Visio.VisToParts.visNone));
                            }
                        }
                    }
                }
            }

            visioTargetDocument.Save();

            visioTargetDocument.Close();

            visioApp.Quit();

            Console.WriteLine(String.Format("File written to \"{0}\"", targetFileInfo.FullName));

            return 0;
        }

        private static void Usage()
        {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("ExtractVisioPages <sourceFilePath> <referencePageName> <targetFilePath>                           Copies source file to target file and removes all but the referenced page from target visio file");
            Console.Error.WriteLine("ExtractVisioPages <sourceFilePath> <referencePageName1>,<referencePageName2> <targetFilePath>     Copies source file to target file and removes all but the referenced pages from target visio file");
        }
    }
}
