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
// User interface class 

using JA_Projekt.Utility.Benchmark;
using System.Drawing;
using JA_Projekt.DLL;
using JA_Projekt.Utility;

enum ProgramOptions
{
    ApplyAlgorithm,
    AnalizeSpeed,
    GoBackToMenu
}

namespace JA_Projekt.Main
{
    internal class UserInterface
    {
        bool _closing = false;
        string _pathToFile = "";
        ProgramOptions programOptions;
        Algorithm algorithm;
        int threadCount;
        int sampleSize;
        


        int imageWidth = 0;
        int imageHeight = 0;

        

        private static UserInterface? instance = null;
        public static UserInterface Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserInterface();
                }
                return instance;


            }
        }
                
        public void OpenMenu()
        {
            while (!_closing)
            {
                if (!AskForPath())
                    continue;

                WaitForInput();
                Console.Clear();

                CustomBitmap bmp = new CustomBitmap();
                switch (AskIfAnalize())
                {
                    case ProgramOptions.AnalizeSpeed:
                        bmp.LoadFromFile(_pathToFile);
                        SpecifySampleSize();
                        Console.Clear();
                        Console.WriteLine("Wait for the analize to complete...");
                        StatisticsManager.Instance.StartBenchmark(bmp, sampleSize);
                        WaitForInput();
                        break;
                    case ProgramOptions.ApplyAlgorithm:
                        bmp.LoadFromFile(_pathToFile);
                        SpecifyAlgorithm();
                        SpecifyThreadCount();
                        Console.Clear();
                        ThreadsManager tm = new ThreadsManager(algorithm, bmp, threadCount);
                        DisplayStats(tm.ProcessImage());
                        bmp.SaveToFile(ProjectPaths.Instance.ExportImagePath);
                        Console.WriteLine($"Output image was saved in default save location which is:\n{ProjectPaths.Instance.ExportImagePath}");
                        WaitForInput();
                        break;
                    case ProgramOptions.GoBackToMenu:
                        break;
                }                   
            }
        }

        void SpecifySampleSize()
        {
            bool succes = false;
            int input;

            while (!succes)
            {
                Console.Write("Specify sample size: ");            
                if (int.TryParse(Console.ReadLine(), out input))
                {
                    if (input > 0)
                    {
                        sampleSize = input;
                        return;
                    }
                    else
                    {
                       Console.WriteLine();
                       Console.WriteLine("Sample size must be non negative.");
                    }

                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Sample size must be a integer");
                }
            }
        }

        void SpecifyThreadCount()
        {
            bool succes = false;
            int input = 0;

            while (!succes)
            {
                Console.Write("Specify thread count: ");
                if (int.TryParse(Console.ReadLine(), out input))
                {
                    
                    if (input > 0 && input <=64)
                    {
                        threadCount = input;
                        return;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Thread count must be in range <1;64>.");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Thread count must be a integer");
                }
            }
        }


        void SpecifyAlgorithm()
        {
            bool succes = false;
            while (!succes)
            {
                Console.WriteLine("==================================================================");
                Console.WriteLine("Specyfy algorithm type: \n");
                Console.WriteLine("1. CppAlgorithm \n");
                Console.WriteLine("2. AsmAlgorithm");
                Console.WriteLine("==================================================================");
                ConsoleKeyInfo input = Console.ReadKey();
                Console.Clear();
                switch (input.Key)
                {
                    case ConsoleKey.D1:
                    //Fall through numeric for 1
                    case ConsoleKey.NumPad1:
                        algorithm = new CppAlgorithm();
                        return;

                    case ConsoleKey.D2:
                    //Fall through numeric for 2
                    case ConsoleKey.NumPad2:
                        algorithm = new AsmAlgortihm();
                        return;

                    case ConsoleKey.D0:
                    //Fall through numeric for 0
                    case ConsoleKey.NumPad0:
                    //Fall through numeric for 0
                    default:
                        break;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        bool AskForPath()
        {
            string input;
            Console.Write("Specify path to file: ");
            input = Console.ReadLine();

            if (String.IsNullOrEmpty(input))
            {
                Console.WriteLine("Path cannot be empty! Please try again.");
                return false;
            }

            if (!File.Exists(input))
            {
                Console.WriteLine("File does not exists! Please try again.");
                return false;
            }

            try
            {
                Image img = Image.FromFile(input);
                Bitmap bitmapMock = new Bitmap(img);
                imageWidth =  bitmapMock.Size.Width;
                imageHeight = bitmapMock.Size.Height;
                ProjectPaths.Instance.ImageFileName = Path.GetFileNameWithoutExtension(input) + ".bmp";
                bitmapMock.Dispose();
                img.Dispose();
                img = null;
                bitmapMock = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File cannot be converted to BitMap! Please try again.");
                return false;
            }

            _pathToFile = input;
            return true;
        }

        ProgramOptions AskIfAnalize()
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine($"Path to file: {_pathToFile}");
            Console.WriteLine($"Estimated size of the file is: {CalculateImageSize()} MB - ({imageWidth}x{imageHeight})");
            Console.WriteLine("==================================================================");
            Console.WriteLine("What do you want to do with this file: \n");
            Console.WriteLine("1. Apply sharpening algorithm (with file output as result) [FAST]\n");
            Console.WriteLine("2. Analize the algorithm speed (with NO file output as result) [LONG]\n");
            Console.WriteLine("0. Choose diferent file");
            Console.WriteLine("==================================================================");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.Clear();
            switch (input.Key)
            {
                case ConsoleKey.D1:
                    //Fall through numeric for 1
                case ConsoleKey.NumPad1:
                    return ProgramOptions.ApplyAlgorithm;

                case ConsoleKey.D2:
                //Fall through numeric for 2
                case ConsoleKey.NumPad2:
                    return ProgramOptions.AnalizeSpeed;

                case ConsoleKey.D0:
                //Fall through numeric for 0
                case ConsoleKey.NumPad0:
                //Fall through numeric for 0
                default:
                    return ProgramOptions.GoBackToMenu;
            }

        }

        double CalculateImageSize()
        {
            int bytesPerPixel = 3;
            
            return Math.Round((imageWidth * imageHeight * bytesPerPixel) / 1000000.0, 2);
        }
        void DisplayStats(double time)
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine($"File name: {_pathToFile}\n");
            Console.WriteLine($"File size: {CalculateImageSize()}MB\n");
            Console.WriteLine($"File dimensions: ({imageWidth} x {imageHeight})\n");
            Console.WriteLine($"Algorithm type: {algorithm.ToString()}\n");
            Console.WriteLine($"Thread count: {threadCount}\n");
            Console.WriteLine($"Time to complete: {time}ms");
            Console.WriteLine("==================================================================");

        }

        void WaitForInput()
        {
            Console.Write("\nPress any key to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
