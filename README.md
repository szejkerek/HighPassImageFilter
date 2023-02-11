### A program to filter an image using a library written in C++ or Assembly with time comparisons
## Bartłomiej Gordon INF sem 5

Using this program, we can filter any Bitmap image using a high-pass filter that removes the average.

Sample output image:

![Lion](https://user-images.githubusercontent.com/69083596/218283011-ad3b0cc3-9e92-4fe3-a340-8aa070178aed.png)

Algorithm implementation in C++:

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

![wykres](https://user-images.githubusercontent.com/69083596/218283013-534c58a6-caf1-48a5-9d45-abea25cb8601.png)
