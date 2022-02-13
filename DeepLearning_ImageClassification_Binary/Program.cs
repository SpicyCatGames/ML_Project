using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static ML_Project.ImagePreProcessor;

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
            List<ExtractedData> colorProfiles = ImagePreProcessor.PreProcessImages(preprocessInputDir, "*.jpg");

            //Rgba32 rgba32 = new Rgba32(128, 157, 48);
            //HSLAColor hSLAColor = ColorUtil.FromRGB(rgba32);
            //Console.WriteLine(hSLAColor.H);
        }


    }
}
