using System;
using System.Collections.Generic;
using CsvHelper;
using System.IO;
using System.Text;
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
            List<ExtractedData> dataColl = ImagePreProcessor.PreProcessImages(preprocessInputDir, "*.jpg");

            try
            {
                string strFilePath = Path.Combine(preprocessOutputDir, @"output.csv"); ;
                string strSeperator = ",";
                StringBuilder sbOutput = new StringBuilder();

                for (int i = 0; i < dataColl.Count; i++)
                {
                    Console.WriteLine(dataColl[i]);
                    sbOutput.AppendLine(string.Join(strSeperator, dataColl[i].Yellow, dataColl[i].YellowGreen, dataColl[i].Green, dataColl[i].Ripened));


                }
                // Create and write the csv file
                File.WriteAllText(strFilePath, sbOutput.ToString());

                // To append more lines to the csv file
                // File.AppendAllText(strFilePath, sbOutput.ToString());
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }

            //Rgba32 rgba32 = new Rgba32(128, 157, 48);
            //HSLAColor hSLAColor = ColorUtil.FromRGB(rgba32);
            //Console.WriteLine(hSLAColor.H);
        }


    }
}
