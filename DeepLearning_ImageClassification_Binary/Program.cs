using System;
using System.Collections.Generic;
using CsvHelper;
using System.IO;
using System.Text;
using SixLabors.ImageSharp; // Requires nuget package imagesharp
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static ML_Project.ImagePreProcessor;
using Microsoft.ML;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.ML.Data;

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
            List<ExtractedData> dataColl = ImagePreProcessor.PreProcessImages(preprocessInputDir, "*.jpg", preprocessOutputDir);

            try
            {
                string strFilePath = Path.Combine(preprocessOutputDir, @"output.csv"); ;
                string strSeperator = ",";
                StringBuilder sbOutput = new StringBuilder();

                for (int i = 0; i < dataColl.Count; i++)
                {
                    var stringToWrite = string.Join(strSeperator, dataColl[i].Yellow, dataColl[i].YellowGreen, dataColl[i].Green, dataColl[i].Ripened);
                    //Console.WriteLine(stringToWrite);
                    sbOutput.AppendLine(stringToWrite);
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

            MLContext mlContext = new MLContext();

            TrainTestData splitDataView = LoadData(mlContext, dataColl);

            Console.WriteLine("=============== LdSVM ===============");
            var LdSVMestimator = mlContext.Transforms
                .Concatenate("Features", "Yellow", "Green", "YellowGreen")
                .Append(mlContext.BinaryClassification.Trainers.LdSvm());
            var LdSVMmodel = LdSVMestimator.Fit(splitDataView.TrainSet);
            // ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, LdSVMmodel, splitDataView.TestSet);
            // UseModelWithSingleItem(mlContext, model);

            Console.WriteLine("=============== LinearSVM ===============");
            var LDSVMmlContext = new MLContext();
            splitDataView = LoadData(LDSVMmlContext, dataColl);
            var LinearSVMestimator = LDSVMmlContext.Transforms
                .Concatenate("Features", "Yellow", "Green", "YellowGreen")
                .Append(LDSVMmlContext.BinaryClassification.Trainers.LinearSvm());
            var LinearSVMmodel = LinearSVMestimator.Fit(splitDataView.TrainSet);
            Evaluate(LDSVMmlContext, LinearSVMmodel, splitDataView.TestSet);

            Console.WriteLine("=============== AveragedPerceptron ===============");
            var APContext = new MLContext();
            splitDataView = LoadData(APContext, dataColl);
            var APMestimator = APContext.Transforms
                .Concatenate("Features", "Yellow", "Green", "YellowGreen")
                .Append(APContext.BinaryClassification.Trainers.AveragedPerceptron());
            var APMmodel = APMestimator.Fit(splitDataView.TrainSet);
            Evaluate(APContext, APMmodel, splitDataView.TestSet);

            Console.WriteLine("=============== LbfgsLogisticRegression ===============");
            var LRContext = new MLContext();
            splitDataView = LoadData(LRContext, dataColl);
            var LRestimator = LRContext.Transforms
                .Concatenate("Features", "Yellow", "Green", "YellowGreen")
                .Append(LRContext.BinaryClassification.Trainers.LbfgsLogisticRegression());
            var LRmodel = LRestimator.Fit(splitDataView.TrainSet);
            Evaluate(LRContext, LRmodel, splitDataView.TestSet);

            Console.WriteLine("=============== SdcaLogisticRegression ===============");
            var sdcaLRmlContext = new MLContext();
            splitDataView = LoadData(sdcaLRmlContext, dataColl);
            var sdcaLRestimator = sdcaLRmlContext.Transforms
                .Concatenate("Features", "Yellow", "Green", "YellowGreen")
                .Append(sdcaLRmlContext.BinaryClassification.Trainers.SdcaLogisticRegression());
            var sdcaLRmodel = sdcaLRestimator.Fit(splitDataView.TrainSet);
            Evaluate(sdcaLRmlContext, sdcaLRmodel, splitDataView.TestSet);
        }

        private static TrainTestData LoadData(MLContext mlContext, IEnumerable<ExtractedData> dataColl)
        {
            IDataView dataView = mlContext.Data.LoadFromEnumerable<ExtractedData>(dataColl);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;
        }

        private static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms
                .Concatenate("Features", "Yellow", "Green", "YellowGreen")
                .Append(mlContext.BinaryClassification.Trainers.LdSvm());

            Console.WriteLine("=============== Create and Train the Model ===============");
            var model = estimator.Fit(splitTrainSet);
            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();

            return model;
        }

        private static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
            IDataView predictions = model.Transform(splitTestSet);

            BinaryClassificationMetrics metrics = mlContext.BinaryClassification.EvaluateNonCalibrated(predictions, "Label");

            Console.WriteLine($"Accuracy: {metrics.Accuracy:0.##}{Environment.NewLine}" +
                              $"F1 Score: {metrics.F1Score:#.##}{Environment.NewLine}" +
                              $"Positive Precision: {metrics.PositivePrecision:#.##}{Environment.NewLine}" +
                              $"Negative Precision: {metrics.NegativePrecision:0.##}{Environment.NewLine}" +
                              $"Positive Recall: {metrics.PositiveRecall:#.##}{Environment.NewLine}" +
                              $"Negative Recall: {metrics.NegativeRecall:#.##}{Environment.NewLine}" +
                              $"Area Under Precision Recall Curve: {metrics.AreaUnderPrecisionRecallCurve:#.##}{Environment.NewLine}");
        }

        private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            PredictionEngine<ExtractedData, RipenessPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<ExtractedData, RipenessPrediction>(model);
            ExtractedData sampleData = new ExtractedData
            {
                Yellow = 70.57143f,
                YellowGreen = 27.714285f,
                Green = 1.7142856f, 
                Ripened = true
            };
            var resultPrediction = predictionFunction.Predict(sampleData);

            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Y:{resultPrediction.Yellow}, " +
                $"YG:{resultPrediction.YellowGreen}, " +
                $"G:{resultPrediction.Green} " +
                (resultPrediction.Ripened ? "Ripe" : "Unripe"));
            Console.WriteLine($"Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Ripe" : "Unripe")}");

            Console.WriteLine("=============== End of Predictions ===============");
            Console.WriteLine();
        }

        private static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {

        }
    }
}
