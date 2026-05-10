### A program to filter an image using a library written in C++ or Assembly with time comparisons
#### Bartłomiej Gordon INF sem 5

Using this program, we can filter any Bitmap image using a high-pass filter that removes the average. Implementation of the main algorithm in Assembly guaranteed almost 2x speed-up of execution.

#### Sample output image:

![Lion](https://user-images.githubusercontent.com/69083596/218283011-ad3b0cc3-9e92-4fe3-a340-8aa070178aed.png)

#### Algorithm implementation in C++:

```cpp
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

```

DLL entry point that loops through all pixels given for this thread applying mask and saving values in new pixels array.
```cpp
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
```

Function that apply mask value for given RGB component.
```cpp
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
```

Clamping value between 0-255
```cpp
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
```

#### Algorithm implementation in Assembly:

```Assembly
.code
ExecuteInAssembly proc

mov ebx, dword ptr[rbp + 48]			
mov r10, rbx							

xor r11, r11							
sub r11, r10							

mov r12, rdx							

mov rdi, r8								
add rcx, r8								
add R12, r8								

programLoop:
cmp rdi, r9								
je endLoop															

pinsrb xmm1, byte ptr[RCX + R11 - 3], 0 
pinsrb xmm1, byte ptr[RCX + R11]    , 1 
pinsrb xmm1, byte ptr[RCX + R11 + 3], 2 
pinsrb xmm1, byte ptr[RCX - 3]      , 3 
movzx  ebx , byte ptr[RCX] 				
pinsrb xmm1, byte ptr[RCX + 3]      , 4 
pinsrb xmm1, byte ptr[RCX + R10 - 3], 5 
pinsrb xmm1, byte ptr[RCX + R10]    , 6 
pinsrb xmm1, byte ptr[RCX + R10 + 3], 7 

mov eax, 9								
mul ebx									

pxor xmm2, xmm2							
psadbw xmm1, xmm2						
movd ebx, xmm1							

sub eax, ebx							
										

mov     ebx, 255						
cmp     eax, ebx							
cmovg   eax, ebx						
test    eax, eax						
mov     ebx, 0							
cmovl   eax, ebx						

mov byte ptr[R12], al					

inc rdi									
inc rcx									
inc R12									
jmp programLoop
endLoop:
ret
ExecuteInAssembly endp
end
```

Passing arguments from the main program and storing them in the appropriate registers.
```Assembly
mov ebx, dword ptr[rbp + 48]			
mov r10, rbx							

xor r11, r11							
sub r11, r10							

mov r12, rdx							

mov rdi, r8								
add rcx, r8								
add R12, r8	
```

Transfer all values surrounding a given pixel to the first half of the XMM1 register.
```Assembly
pinsrb xmm1, byte ptr[RCX + R11 - 3], 0 
pinsrb xmm1, byte ptr[RCX + R11]    , 1 
pinsrb xmm1, byte ptr[RCX + R11 + 3], 2 
pinsrb xmm1, byte ptr[RCX - 3]      , 3 
movzx  ebx , byte ptr[RCX] 				
pinsrb xmm1, byte ptr[RCX + 3]      , 4 
pinsrb xmm1, byte ptr[RCX + R10 - 3], 5 
pinsrb xmm1, byte ptr[RCX + R10]    , 6 
pinsrb xmm1, byte ptr[RCX + R10 + 3], 7 
```

Multiply the middle mask value by 9 and then subtract from it the sum of the rest of the pixels added together using the vector horizontal addition instruction.
```Assembly
mov eax, 9								
mul ebx									

pxor xmm2, xmm2							
psadbw xmm1, xmm2						
movd ebx, xmm1							

sub eax, ebx												
```
Clamp the obtained value in the range 0-255
```Assembly								
mov     ebx, 255						
cmp     eax, ebx							
cmovg   eax, ebx						
test    eax, eax						
mov     ebx, 0							
cmovl   eax, ebx						
```
Transfer the calculated value to the appropriate place in the new pixel array
```Assembly								
mov byte ptr[R12], al	
```

#### The time comparison of the averaged results of both libraries run on different number of threads with the error bars calculated from the standard deviation:

![wykres](https://user-images.githubusercontent.com/69083596/218283013-534c58a6-caf1-48a5-9d45-abea25cb8601.png)

## Code Highlights

### SIMD Horizontal Sum via `psadbw` in the Assembly Convolution Loop

```asm
pinsrb xmm1, byte ptr[RCX + R11 - 3], 0 ;Place maskValue on index 0 in xmm1
pinsrb xmm1, byte ptr[RCX + R11]    , 1 ;Place maskValue on index 1 in xmm1
pinsrb xmm1, byte ptr[RCX + R11 + 3], 2 ;Place maskValue on index 2 in xmm1
pinsrb xmm1, byte ptr[RCX - 3]      , 3 ;Place maskValue on index 3 in xmm1
movzx  ebx , byte ptr[RCX] 				;Place middle maskValue in ebx
pinsrb xmm1, byte ptr[RCX + 3]      , 4 ;Place maskValue on index 4 in xmm1
pinsrb xmm1, byte ptr[RCX + R10 - 3], 5 ;Place maskValue on index 5 in xmm1
pinsrb xmm1, byte ptr[RCX + R10]    , 6 ;Place maskValue on index 6 in xmm1
pinsrb xmm1, byte ptr[RCX + R10 + 3], 7 ;Place maskValue on index 7 in xmm1

mov eax, 9								;Move value 9 (from mask) to eax
mul ebx									;Multiply pixel middle value by 9

pxor xmm2, xmm2							;Zero xmm2 register
psadbw xmm1, xmm2						;Add all values pixel values to eachothers and store in in xmm1
movd ebx, xmm1							;Move stored value into ebx

sub eax, ebx							;Subtract all added values with middle pixel multiplied by mask 
										;(v1+v2+v3 .. +v8) - 9*v5

mov     ebx, 255						;Clamp the value we got between 0 - 255
cmp     eax, ebx						;	
cmovg   eax, ebx						;
test    eax, eax						;
mov     ebx, 0							;
cmovl   eax, ebx						;
```

Because all 8 neighbor coefficients in the kernel are `-1`, the 8 individual multiply-accumulate operations collapse into a single horizontal byte sum. The eight surrounding pixel values are packed into the lower half of `XMM1` using `pinsrb`, then `psadbw` against a zeroed `XMM2` computes their sum in one SIMD instruction. The center pixel is separately multiplied by 9 and the neighbor sum is subtracted. The final clamp to `[0, 255]` uses branchless `cmovg`/`cmovl` conditional moves, avoiding branch misprediction penalties on the hot path.

### Boundary-Aware Thread Row Partitioning

```csharp
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

private int CalculateStartingIndex(int taskID, int threadRow)
{
    int row = _bmp.Width * _pixelStride;
    int skipFirstRowIndex = row;
    int threadPoolFirstObject = skipFirstRowIndex + (taskID * _rowsPerThread * row);
    int currentIndex = threadPoolFirstObject + (threadRow * row) + _pixelStride;
    return currentIndex;
}
```

A 3×3 convolution kernel cannot be applied to border pixels that lack a full neighbourhood. `CalculateThreadsValues` subtracts 2 from both height (`realHeight = height - 2`) and adjusts width by two pixel strides (`realWidth = _bmp.Stride - 2*_pixelStride`) to exclude the single-pixel border on all sides. `CalculateStartingIndex` then builds the correct byte offset per thread and per row, including the `_pixelStride` column skip, ensuring no thread ever attempts to read outside the valid pixel neighbourhood. Remainder rows (when `realHeight % threadsCount != 0`) are assigned to the last thread.

### Benchmark Warm-Up Trimming and Standard Deviation

```csharp
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

private void CalculateStdDv()
{
    _stdDev = Math.Sqrt(timings.Average(v => Math.Pow(v - _avg, 2)));
}
```

The benchmarking pipeline trims the first 2.5% of recorded samples before computing statistics. This removes JIT compilation overhead and CPU cache warm-up from the first one or two runs, which would otherwise skew average timings upward for small sample sets. Standard deviation is computed as the population formula `sqrt(mean of squared deviations)`, appropriate here since the full measurement sample — not an estimate from a subset — is used.
