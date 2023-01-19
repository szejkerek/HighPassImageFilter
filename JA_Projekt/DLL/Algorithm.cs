using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA_Projekt.DLL
{
    internal abstract class Algorithm
    {
        public abstract void CalculatePixel(byte[] oldPixels, byte[] newPixels, int startingIndex, int endIndex, int width);

        public abstract override string ToString();
    }
}
