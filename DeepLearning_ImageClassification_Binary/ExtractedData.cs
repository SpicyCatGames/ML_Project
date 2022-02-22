using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Project
{
    public class ExtractedData
    {
        [LoadColumn(0)]
        public float Yellow;
        [LoadColumn(1)]
        public float Green;
        [LoadColumn(2)]
        public float YellowGreen;
        [LoadColumn(3), ColumnName("Label")]
        public bool Ripened;

        public void TurnIntoPercents()
        {
            float totalPixels = Yellow + YellowGreen + Green;

            this.Yellow = (this.Yellow / totalPixels) * 100f;
            this.YellowGreen = (this.YellowGreen / totalPixels) * 100f;
            this.Green = (this.Green / totalPixels) * 100f;
        }
    }

    public class RipenessPrediction : ExtractedData
    {

        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
