using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JA_Projekt.Utility
{
    internal class ProjectPaths
    {
        public const string AssemblyDllPath = "D:\\Projekty\\HighPassFilter\\x64\\Debug\\ASM.dll";
        public const string CppDllPath = "D:\\Projekty\\HighPassFilter\\x64\\Release\\CPP.dll";

        string imageFileName;
        string directoryPath;
        string exportImagePath;
        string importImagePath;
        string csvPath;

        public string ExportImagePath { get => exportImagePath; }
        public string ImportImagePath { get => importImagePath; }
        public string DirectoryPath { get => directoryPath; }
        public string ImageFileName { get => imageFileName;
            set { 
                    imageFileName = value;
                    InitializePaths(); } 
                }
        public string CsvPath { get => csvPath; set => csvPath = value; }


        private static ProjectPaths? instance = null;
        public static ProjectPaths Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProjectPaths();
                }
                return instance;
            }
        }


        public ProjectPaths()
        {
            InitializePaths();
        }

        private void InitializePaths()
        {
            string workingDirectory = Environment.CurrentDirectory;
            directoryPath = Directory.GetParent(path: workingDirectory).Parent.Parent.Parent.FullName;
            exportImagePath = directoryPath + "\\Img_exports\\";

            if (!Directory.Exists(exportImagePath))
            {
                Directory.CreateDirectory(exportImagePath);
            }
            exportImagePath = directoryPath + "\\Img_exports\\" + imageFileName;

            importImagePath = directoryPath + "\\Img_imports\\" + imageFileName;
            CsvPath = directoryPath + "\\CSVs\\";
        }
    }
}
