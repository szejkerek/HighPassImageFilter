using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JA_Projekt.Utility;

namespace JA_Projekt.DLL
{
    internal class AsmAlgortihm : Algorithm
    {
        [DllImport("ASM.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int ExecuteInAssembly(byte[] oldPixels, byte[] newPixels, int startingIndex, int endIndex, int width);

        public override void CalculatePixel(byte[] oldPixels, byte[] newPixels, int startingIndex, int endIndex, int width)
        {
            ExecuteInAssembly(oldPixels, newPixels, startingIndex, endIndex, width);
        }

        public override string ToString()
        {
            return "AsmAlgortihm";
        }
    }
}
