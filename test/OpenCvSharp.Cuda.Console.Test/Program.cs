using OpenCvSharp;
using OpenCvSharp.Text;
using System;
using System.Diagnostics;
using static OpenCvSharp.FileStorage;

if (OperatingSystem.IsLinux()){
    Console.WriteLine("=== Starting Linux Diagnostics ===");

    try
    {
        // 1. Run the Linux 'ldd' command to see if the .so file is missing any C++ dependencies
        Console.WriteLine("Checking dependencies for libOpenCvSharpExtern.so...");
        var p = Process.Start(new ProcessStartInfo("ldd", "libOpenCvSharpExtern.so")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });
        Console.WriteLine(p.StandardOutput.ReadToEnd());
        Console.WriteLine(p.StandardError.ReadToEnd());
        p.WaitForExit();

        Console.WriteLine("=== Attempting to load OpenCV ===");

        // 2. Try to run OpenCV
        Console.WriteLine(Cv2.GetBuildInformation());

        Console.WriteLine("=== SUCCESS ===");
    }
    catch (Exception ex)
    {
        // 3. If it crashes, stop the SIGABRT and print the actual .NET error!
        Console.WriteLine("=== MANAGED CRASH CAUGHT ===");
        Console.WriteLine(ex.ToString());
    }
}
else
{
    Console.WriteLine(Cv2.GetBuildInformation());

   

}

