using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JA_Projekt.Utility;

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
