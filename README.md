### A program to filter an image using a library written in C++ or Assembly with time comparisons
## Bartłomiej Gordon INF sem 5

Using this program, we can filter any Bitmap image using a high-pass filter that removes the average.

##Sample output image:

![Lion](https://user-images.githubusercontent.com/69083596/218283011-ad3b0cc3-9e92-4fe3-a340-8aa070178aed.png)

##**Algorithm implementation in C++:**

Main function that loops through all pixels given for this thread applying mask and saving values in new pixels array.
```cpp
float mask[9] = {-1 ,-1 ,-1 ,
                 -1 , 9 ,-1 ,
                 -1 ,-1 ,-1  };

for (int i = startingIndex; i < endIndex; i ++)
{
    newPixels[i] = CutRange(CalculatePixelValue(i, oldPixels, width, PIXEL_STRIDE, mask));
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

##**Algorithm implementation in Assembly:**

```Assembly
.code
ExecuteInAssembly proc

mov ebx, dword ptr[rbp + 48]			;Move width to ebx
mov r10, rbx							;Move stride to r10

xor r11, r11							;Clear R11 register
sub r11, r10							;Assign negative value of width do R11

mov r12, rdx							;Move NewPixelPointer to ebx

mov rdi, r8								;Establish counter from starting index to rdi
add rcx, r8								;Move pointer to starting position
add R12, r8								;Move pointer to starting position

programLoop:
cmp rdi, r9								;Compare current index with end index - if equal, end loop
je endLoop															

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

mov byte ptr[R12], al					;Place clamped value in extact place in table

inc rdi									;Increment current index (loop)
inc rcx									;Increment index (oldPixelsPointer)
inc R12									;Increment index (newPixelsPointer)
jmp programLoop
endLoop:
ret
ExecuteInAssembly endp
end
```

##The time comparison of the averaged results of both libraries with the error bars calculated from the standard deviation:

![wykres](https://user-images.githubusercontent.com/69083596/218283013-534c58a6-caf1-48a5-9d45-abea25cb8601.png)
