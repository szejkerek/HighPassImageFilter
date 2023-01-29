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
// Interface for measuring time

using JA_Projekt.DLL;

namespace JA_Projekt.Utility.Benchmark
{
    internal class TimeMesurement
    {
        Algorithm usedAlgorith;
        int usedThreads;
        int usedImgSize;
        List<double> timings = new List<double>();
        double _avg = 0;
        double _stdDev = 0;

        public double Avg { get => _avg; }
        public double StdDev { get => _stdDev; }

        public TimeMesurement(Algorithm usedAlgorith,
                              int usedThreads,
                              int usedImgSize)
        {
            this.usedAlgorith = usedAlgorith;
            this.usedThreads = usedThreads;
            this.usedImgSize = usedImgSize;
        }

        public void AddTime(double time)
        {
            timings.Add(time);
        }

        public void CalculateValues()
        {
            RemoveFaultyMesurements((int)(timings.Count * 0.025));
            CalculateAvg();
            CalculateStdDv();
        }

        private void RemoveFaultyMesurements(int count)
        {
            if (count >= timings.Count)
                return;

            timings.RemoveRange(0, count);
        }

        private void CalculateAvg()
        {
            _avg = timings.Average();
        }

        private void CalculateStdDv()
        {
            _stdDev = Math.Sqrt(timings.Average(v => Math.Pow(v - _avg, 2)));
        }

        public string TimeMesurementInfo()
        {
            return $"=======================================\nUsedAlgorithm:\t{usedAlgorith.ToString()}\nSample size:\t{timings.Count} - (reduced to minimalize error)\n=======================================";
        }

        public override string ToString()
        {
            return $"[{usedThreads} threads]\tTime: {Math.Round(_avg, 2)}ms\t+-{Math.Round(_stdDev, 2)}";
        }
    }
}
