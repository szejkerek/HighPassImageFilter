// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <cstdint>

constexpr int PIXEL_STRIDE = 3;

inline int CalculatePixelValue(int index, uint8_t* oldPixels, int width, int depth, float* mask)
{
    return  (oldPixels[index - width - depth]   * mask[0]) +
            (oldPixels[index - width]           * mask[1]) +
            (oldPixels[index - width + depth]   * mask[2]) +
            (oldPixels[index - depth]           * mask[3]) +
            (oldPixels[index]                   * mask[4]) +
            (oldPixels[index + depth]           * mask[5]) +
            (oldPixels[index + width - depth]   * mask[6]) +
            (oldPixels[index + width]           * mask[7]) +
            (oldPixels[index + width + depth]   * mask[8]);
}

inline uint8_t CutRange(int value)
{
    int newValue = value;

    if (value > 255)
    {
        newValue = 255;
    }
    else if (value < 0)
    {
        newValue = 0;
    }

    return static_cast<uint8_t>(newValue);;
}

extern "C" __declspec(dllexport) void ExecuteInCpp(uint8_t * oldPixels, uint8_t * newPixels, int startingIndex, int endIndex, int width)
{
    float mask[9] = {-1 ,-1 ,-1 ,
                     -1 , 9 ,-1 ,
                     -1 ,-1 ,-1  };

    for (int i = startingIndex; i < endIndex; i ++)
    {
        newPixels[i] = CutRange(CalculatePixelValue(i, oldPixels, width, PIXEL_STRIDE, mask));
    }
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

