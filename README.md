### A program to filter an image using a library written in C++ or Assembly with time comparisons
## Bartłomiej Gordon INF sem 5

Using this program, we can filter any Bitmap image using a high-pass filter that removes the average. Implementation of the main algorithm in Assembly guaranteed almost 2x speed-up of execution.

## Sample output image:

![Lion](https://user-images.githubusercontent.com/69083596/218283011-ad3b0cc3-9e92-4fe3-a340-8aa070178aed.png)

## Algorithm implementation in C++:

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

## Algorithm implementation in Assembly:

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

```Assembly
mov eax, 9								
mul ebx									

pxor xmm2, xmm2							
psadbw xmm1, xmm2						
movd ebx, xmm1							

sub eax, ebx												
```

```Assembly								
mov     ebx, 255						
cmp     eax, ebx							
cmovg   eax, ebx						
test    eax, eax						
mov     ebx, 0							
cmovl   eax, ebx						

mov byte ptr[R12], al	
```

```Assembly								
mov byte ptr[R12], al	
```

## The time comparison of the averaged results of both libraries with the error bars calculated from the standard deviation:

![wykres](https://user-images.githubusercontent.com/69083596/218283013-534c58a6-caf1-48a5-9d45-abea25cb8601.png)
