//
// Author: Bartłomiej Gordon sem.5 2022/2023
//
// Topic: Implementation of High - Pass image filter in both: CPPand
//        ASM with time comparison using given convolution mask:
//
//		-1 - 1 - 1
//		-1   9 - 1
//		-1 - 1 - 1
//
// Class for paths management  

namespace JA_Projekt.Utility
{
    internal class ProjectPaths
    {
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
