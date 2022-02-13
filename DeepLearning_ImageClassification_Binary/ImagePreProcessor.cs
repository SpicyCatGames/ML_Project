using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Text;
using CsvHelper;

namespace ML_Project
{
    public static class ImagePreProcessor
    {
        public static List<ExtractedData> PreProcessImages(string inputDir, string fileExtension)
        {
            List<ExtractedData> colorProfiles = new List<ExtractedData>();
            string[] files = Directory.GetFiles(inputDir, fileExtension);
            foreach (var item in files)
            {
                // extract all the pixels from this image
                List<Rgba32> pixels = ImagePixels(inputDir, item);
                // green 81° to 140°
                // yellow green 61° to 80°
                // yellow 51° to 60°
                ExtractedData colorProfile = new ExtractedData();
                foreach (var pixel in pixels)
                {
                    var hslColor = ColorUtil.FromRGB(pixel);
                    if (DiscardRGBPixel(pixel))
                    {
                        continue;
                    }
                    else if(hslColor.H.IsBetween(51f, 60f))
                    {
                        // yellow
                        colorProfile.Yellow += 1;
                    }
                    else if (hslColor.H.IsBetween(60f, 80f))
                    {
                        colorProfile.YellowGreen += 1;
                    }
                    else if(hslColor.H.IsBetween(80f, 140f))
                    {
                        colorProfile.Green += 1;
                    }
                }
                colorProfiles.Add(colorProfile);
                Console.WriteLine($"{item.Substring(item.LastIndexOf('\\')+1)}, {colorProfile.Yellow},{colorProfile.YellowGreen},{colorProfile.Green}");
            }
            return colorProfiles;
        }

        private static List<Rgba32> ImagePixels(string preprocessDir, string item)
        {
            List<Rgba32> pixels = new List<Rgba32>();
            using (Image<Rgba32> image = Image.Load<Rgba32>(Path.Combine(preprocessDir, item)))
            {
                // Image<Rgba32> copy = image.Clone<Rgba32>(x => x.Resize(400, 300));
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // http://mkweb.bcgsc.ca/color-summarizer/?analyze
                        Rgba32 rgba32 = default;
                        image[x, y].ToRgba32(ref rgba32);
                        pixels.Add(rgba32);
                        // Console.WriteLine(rgba32.ToString());
                    }
                }
                return pixels;
            }
        }

        public struct ExtractedData
        {
            public int Yellow;
            public int Green;
            public int YellowGreen;
            public int Ripened;
        }
        private static bool ApproximatelyEqual(this float a, float b, float epsilon = float.Epsilon)
        {
            return Math.Abs(a - b) <= epsilon;
        }
        private static bool IsBetween(this float x, float StartInclusive, float endExclusive)
        {
            return x >= StartInclusive && x < endExclusive;
        }
        private static bool DiscardRGBPixel(Rgba32 rgba32)
        {
            byte maxAllColorThreshold = 240;
            byte minAllColorThreshold = 25;
            float minSaturation = 0.05f;
            if (rgba32.A < 2)
            {
                return true;
            }
            if(rgba32.R > maxAllColorThreshold && rgba32.G > maxAllColorThreshold && rgba32.B > maxAllColorThreshold)
            {
                return true;
            }
            if (rgba32.R < minAllColorThreshold && rgba32.G < minAllColorThreshold && rgba32.B < minAllColorThreshold)
            {
                return true;
            }

            HSLAColor hSLAColor = ColorUtil.FromRGB(rgba32);
            if (hSLAColor.S <= minSaturation)
            {
                return true;
            }

            return false;
        }
    }
}
