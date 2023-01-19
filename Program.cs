using System.Runtime.InteropServices;

[DllImport(@"D:\Projekty\JA_Projekt\x64\Debug\ASM.dll")]
static extern int MyProc1(int a, int b);

[DllImport(@"D:\Projekty\JA_Projekt\x64\Debug\CPP.dll")]
static extern int MyProc2(int a, int b);

Console.WriteLine(MyProc1(8, 3));
Console.WriteLine(MyProc2(8, 3));