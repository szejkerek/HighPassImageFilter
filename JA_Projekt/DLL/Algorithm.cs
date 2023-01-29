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
// C++ DLL with algorithm 

namespace JA_Projekt.DLL
{
    internal abstract class Algorithm
    {
        public abstract void CalculatePixel(byte[] oldPixels, byte[] newPixels, int startingIndex, int endIndex, int width);

        public abstract override string ToString();
    }
}
