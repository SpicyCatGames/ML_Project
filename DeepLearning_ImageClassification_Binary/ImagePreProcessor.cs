using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ML_Project
{
    public static class ImagePreProcessor
    {
        public static List<ExtractedData> PreProcessImages(string inputDir, string fileExtension, string outputDir = "")
        {
            List<ExtractedData> dataColl = new List<ExtractedData>();
            string[] files = Directory.GetFiles(inputDir, fileExtension);
            foreach (var item in files)
            {
                ExtractedData extractedData = new ExtractedData();
                using (Image<Rgba32> image = Image.Load<Rgba32>(Path.Combine(inputDir, item)))
                {
                    // green 81° to 140°
                    // yellow green 61° to 80°
                    // yellow 51° to 60°
                    bool saveOutputAsFile = false;
                    if(outputDir != "") saveOutputAsFile = true;
                    Image<Rgba32> copy = image.Clone<Rgba32>();
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            // http://mkweb.bcgsc.ca/color-summarizer/?analyze
                            Rgba32 rgba32 = default;
                            image[x, y].ToRgba32(ref rgba32);
                            // Console.WriteLine(rgba32.ToString());
                            var hslColor = ColorUtil.FromRGB(rgba32);
                            if (DiscardRGBPixel(rgba32))
                            {
                                if (saveOutputAsFile) copy[x, y] = new Rgba32(0, 0, 0, 255);
                                continue;
                            }
                            else if (hslColor.H.IsBetween(51f, 60f))
                            {
                                // yellow
                                if (saveOutputAsFile) copy[x, y] = new Rgba32(255, 255, 0, 255);
                                extractedData.Yellow += 1;
                            }
                            else if (hslColor.H.IsBetween(60f, 80f))
                            {
                                if (saveOutputAsFile) copy[x, y] = new Rgba32(154, 205, 50, 255);
                                extractedData.YellowGreen += 1;
                            }
                            else if (hslColor.H.IsBetween(80f, 140f))
                            {
                                if (saveOutputAsFile) copy[x, y] = new Rgba32(0, 255, 0, 255);
                                extractedData.Green += 1;
                            }
                            else
                            {
                                copy[x, y] = new Rgba32(85, 85, 85, 255);
                            }

                            if (item.Contains("unripe"))
                            {
                                extractedData.Ripened = false;
                            }
                            else if(item.Contains("ripe"))
                            {
                                extractedData.Ripened = true;
                            }
                        }
                    }
                    if (saveOutputAsFile)
                        copy.Save(Path.Combine(outputDir, item.Substring(item.LastIndexOf('\\') + 1)));
                }
                extractedData.TurnIntoPercents();
                dataColl.Add(extractedData);
                Console.WriteLine($"{item.Substring(item.LastIndexOf('\\')+1)}, " +
                    $"Y:{extractedData.Yellow}, " +
                    $"YG:{extractedData.YellowGreen}, " +
                    $"G:{extractedData.Green} " +
                    (extractedData.Ripened ? "Ripe" : "Unripe"));
            }
            return dataColl;
        }

        private static bool ApproximatelyEqual(this float a, float b, float epsilon = float.Epsilon)
        {
            return Math.Abs(a - b) <= epsilon;
        }
        private static bool IsBetween(this float x, float startInclusive, float endExclusive)
        {
            return x >= startInclusive && x < endExclusive;
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
