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
// C++ DLL importer 

using System.Runtime.InteropServices;

namespace JA_Projekt.DLL
{
    internal class CppAlgorithm : Algorithm
    {
        [DllImport("CPP.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int ExecuteInCpp(byte[] oldPixels, byte[] newPixels, int startingIndex, int endIndex, int width);

        public override void CalculatePixel(byte[] oldPixels, byte[] newPixels, int startingIndex, int endIndex, int width)
        {
           ExecuteInCpp(oldPixels, newPixels, startingIndex, endIndex, width);
        }

        public override string ToString()
        {
            return "CppAlgorithm";
        }
    }
}
