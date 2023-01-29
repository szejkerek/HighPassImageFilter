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
// Thread management for algorithm

using JA_Projekt.DLL;
using JA_Projekt.Utility.Benchmark;

namespace JA_Projekt.Main
{
    internal class ThreadsManager
    {
        const int MAX_THREADS = 64;
        Algorithm _alg;
        CustomBitmap _bmp;
        List<Task> _tasks = new List<Task>();
        int _threadsCount = 1;
        int _rowsPerThread = 0;
        int _additionalLastThreadRows = 0;
        int realWidth;
        int realHeight;
        int _pixelStride;

        public ThreadsManager(Algorithm alg, CustomBitmap bmp, int threadsCount)
        {
            _alg = alg;
            _bmp = bmp;
            _pixelStride = _bmp.PixelStride;
            _threadsCount = threadsCount;
            CalculateThreadsValues();           
            AssignTasks();
        }
        public double ProcessImage()
        {
            StatisticsManager st = StatisticsManager.Instance;
            st.StartTimer();
            Parallel.ForEach(_tasks, (task) => task.Start());
            Task.WaitAll(_tasks.ToArray());
            return st.StopTimer();
        }

        private void AssignTasks()
        {           
            for (int i = 0; i < _threadsCount - 1; i++)
            {
                _tasks.Add(CreateTask(i));
            }
            _tasks.Add(CreateTask(_threadsCount - 1, _additionalLastThreadRows));
        }

        private Task CreateTask(int taskID, int additionalRows = 0)
        {
            return new Task(() =>
            {
                for (int i = 0; i < _rowsPerThread + additionalRows; i++)
                {
                    int startIndex = CalculateStartingIndex(taskID, i);
                    int endIndex = startIndex + realWidth;
                    _alg.CalculatePixel(_bmp.BufferOld, _bmp.BufferNew, startIndex, endIndex, _bmp.Width * _pixelStride);
                }
            });
        }

        private int CalculateStartingIndex(int taskID, int threadRow)
        {
            int row = _bmp.Width * _pixelStride;
            int skipFirstRowIndex = row;
            int threadPoolFirstObject = skipFirstRowIndex + (taskID * _rowsPerThread * row);
            int currentIndex = threadPoolFirstObject + (threadRow * row) + _pixelStride;
            return currentIndex;
        }

        private void CalculateThreadsValues()
        {
            int width = _bmp.Width;
            int height = _bmp.Height;

            realWidth = _bmp.Stride - 2*_pixelStride;
            realHeight = height - 2;

            if (_threadsCount <= 0)
            {
                _threadsCount = 1;
            }
            else if (_threadsCount >= MAX_THREADS)
            {
                _threadsCount = 64;
            }

            _rowsPerThread = realHeight / _threadsCount;

            if(_threadsCount * _rowsPerThread !=realHeight)
            {
                _additionalLastThreadRows = realHeight % _threadsCount;
            }
        }
    }
}
