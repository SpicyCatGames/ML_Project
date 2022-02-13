using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ML_Project
{
    public static class Program
    {
        public static string WorkingDirectory = 
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));

        public static void Main()
        {
            string preprocessDir = Path.Combine(WorkingDirectory, @"Preprocess");
            string preprocessInputDir = Path.Combine(preprocessDir, @"Input");
            string preprocessOutputDir = Path.Combine(preprocessDir, @"Output");

            ImagePreProcessor.PreProcessImages(preprocessInputDir, "*.jpg");
        }


    }
}
