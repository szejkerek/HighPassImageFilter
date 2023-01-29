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
// Benchmarking tool

using JA_Projekt.DLL;
using JA_Projekt.Main;
using System.Diagnostics;

namespace JA_Projekt.Utility.Benchmark
{
    internal class StatisticsManager
    {
        Stopwatch? stopwatch;
        Algorithm asm = new AsmAlgortihm();
        Algorithm cpp = new CppAlgorithm();
        int[] threadNumber = { 1, 2, 4, 8, 16, 32, 64 };
        int _sampleCount = 0;

        private static StatisticsManager? instance = null;
        public static StatisticsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StatisticsManager();
                }
                return instance;
            }
        }

        public void StartTimer()
        {
            stopwatch = Stopwatch.StartNew();
        }

        public double StopTimer()
        {
            stopwatch.Stop();
            return (double)stopwatch.Elapsed.TotalMilliseconds;
        }

        private TimeMesurement MeasureProcessing(CustomBitmap bmp, Algorithm algType, int threadsCount)
        {
            TimeMesurement tm = new TimeMesurement(algType, threadsCount, bmp.Size);

            for (int i = 0; i < _sampleCount; i++)
            {
                ThreadsManager threadsManager = new ThreadsManager(algType, bmp, threadsCount);
                tm.AddTime(threadsManager.ProcessImage());
            }

            tm.CalculateValues();

            return tm;
        }

        private List<TimeMesurement> TestThreads(CustomBitmap bmp, Algorithm algType)
        {
            List<TimeMesurement> tm = new List<TimeMesurement>();
            foreach (int threads in threadNumber)
            {
                tm.Add(MeasureProcessing(bmp, algType, threads));
            }
            return tm;
        }

        private void DisplayTimings(List<TimeMesurement> timeMesurements)
        {
            Console.WriteLine(timeMesurements[0].TimeMesurementInfo());
            foreach (var item in timeMesurements)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();

        }

        public void StartBenchmark(CustomBitmap bmp, int sampleCount)
        {
            _sampleCount = sampleCount;
            DisplayTimings(TestThreads(bmp, cpp));
            DisplayTimings(TestThreads(bmp, asm));
        }

    }
}
