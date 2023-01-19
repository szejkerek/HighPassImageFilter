using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using JA_Projekt.Main;
using JA_Projekt.Utility;
using JA_Projekt.DLL;
using JA_Projekt.Utility.Benchmark;

internal class Program
{
    private static void Main(string[] args)
    {
        //CustomBitmap bmp = new CustomBitmap();
        Algorithm alg = new AsmAlgortihm(); //algorithm
        int threadNumber = 4;               //threads
        ProjectPaths.Instance.ImageFileName = "small.bmp";

        UserInterface.Instance.OpenMenu();

        //try
        //{
        //    bmp.LoadFromFile($"{ProjectPaths.Instance.ImportImagePath}");
        //    StatisticsManager.Instance.StartBenchmark(bmp, 201);
        //}

        //catch (Exception e)
        //{
        //    Console.WriteLine(e.ToString());
        //}
    }
}