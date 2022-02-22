using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Project
{
    public class ExtractedData
    {
        public float Yellow;
        public float Green;
        public float YellowGreen;
        public int Ripened;

        public void TurnIntoPercents()
        {
            float totalPixels = Yellow + YellowGreen + Green;

            this.Yellow = (this.Yellow / totalPixels) * 100f;
            this.YellowGreen = (this.YellowGreen / totalPixels) * 100f;
            this.Green = (this.Green / totalPixels) * 100f;
        }
    }
}
