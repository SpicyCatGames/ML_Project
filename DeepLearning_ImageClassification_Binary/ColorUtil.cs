using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Project
{
    public static class ColorUtil
    {
        public static HSLAColor FromRGB(Rgba32 rgba32)
        {
            Byte R = rgba32.R;
            Byte G = rgba32.G;
            Byte B = rgba32.B;

            float _R = (R / 255f);
            float _G = (G / 255f);
            float _B = (B / 255f);

            float _Min = Math.Min(Math.Min(_R, _G), _B);
            float _Max = Math.Max(Math.Max(_R, _G), _B);
            float _Delta = _Max - _Min;

            float H = 0;
            float S = 0;
            float L = (float)((_Max + _Min) / 2.0f);

            if (!(_Delta <= 0.001f && _Delta >= -0.001f)) // TODO: use an epsilon if _Delta != 0
            {
                if (L < 0.5f)
                {
                    S = (float)(_Delta / (_Max + _Min));
                }
                else
                {
                    S = (float)(_Delta / (2.0f - _Max - _Min));
                }


                if (_R == _Max)
                {
                    H = (_G - _B) / _Delta;
                }
                else if (_G == _Max)
                {
                    H = 2f + (_B - _R) / _Delta;
                }
                else if (_B == _Max)
                {
                    H = 4f + (_R - _G) / _Delta;
                }
            }

            H = H * 60f;
            if (H < 0) H += 360;

            return new HSLAColor(H, S, L, rgba32.A);
        }
    }
    
    public struct HSLAColor{
        public float H; // 0 - 360
        public float S; // 0 - 1
        public float L; // 0 - 1
        public byte A; // 0 - 255
        public HSLAColor(float H, float S, float L, byte A)
        {
            this.H = H;
            this.S = S;
            this.L = L;
            this.A = A;
        }
    }
}
