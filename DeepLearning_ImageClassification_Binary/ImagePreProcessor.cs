using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ML_Project
{
    public static class ImagePreProcessor
    {
        public static void PreProcessImages(string dir, string fileExtension)
        {
            string[] files = Directory.GetFiles(dir, fileExtension);
            foreach (var item in files)
            {
                // extract all the pixels from this image
                List<Rgba32> pixels = ImagePixels(dir, item);
                // green 81° to 140°
                // yellow green 61° to 80°
                // yellow 51° to 60°
            }
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
                        Console.WriteLine(rgba32.ToString());
                    }
                }
                return pixels;
            }
        }

        public struct ColorProfile
        {
            public float Yellow;
            public float Green;
            public float Black;
        }
    }
}
