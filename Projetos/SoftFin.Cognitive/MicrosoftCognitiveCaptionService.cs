using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Text;
using System.Collections.Generic;

namespace SoftFin.Cognitive
{

    public class MicrosoftCognitiveCaptionService 
    {
        /// <summary>
        /// Microsoft Computer Vision API key.
        /// </summary>
        private static readonly string ApiKey = "f601e6078e7645689e9634c21c39e48f";

        /// <summary>
        /// The set of visual features we want from the Vision API. 
        /// </summary>
        private static readonly VisualFeature[] VisualFeatures = { VisualFeature.Description };

        /// <summary>
        /// Gets the caption of an image URL.
        /// <remarks>
        /// This method calls <see cref="IVisionServiceClient.AnalyzeImageAsync(string, string[])"/> and
        /// returns the first caption from the returned <see cref="AnalysisResult.Description"/>
        /// </remarks>
        /// </summary>
        /// <param name="url">The URL to an image.</param>
        /// <returns>Description if caption found, null otherwise.</returns>
        public async Task<string> GetCaptionAsync(string url)
        {
            var client = new VisionServiceClient(ApiKey);
            var result = await client.AnalyzeImageAsync(url, VisualFeatures);
            return ProcessAnalysisResult(result);
        }

        /// <summary>
        /// Gets the caption of the image from an image stream.
        /// <remarks>
        /// This method calls <see cref="IVisionServiceClient.AnalyzeImageAsync(Stream, string[])"/> and
        /// returns the first caption from the returned <see cref="AnalysisResult.Description"/>
        /// </remarks>
        /// </summary>
        /// <param name="stream">The stream to an image.</param>
        /// <returns>Description if caption found, null otherwise.</returns>
        public async Task<string> GetCaptionAsync(Stream stream)
        {
            var client = new VisionServiceClient(ApiKey);
            var result = await client.AnalyzeImageAsync(stream, VisualFeatures);
            return ProcessAnalysisResult(result);
        }

        /// <summary>
        /// Processes the analysis result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The caption if found, error message otherwise.</returns>
        private static string ProcessAnalysisResult(AnalysisResult result)
        {
            string message = result.Description.Captions.FirstOrDefault().Text;

            return string.IsNullOrEmpty(message) ?
                        "Couldn't find a caption for this one" :
                        "I think it's " + message;
        }




        public async Task<string> GetTextAsync(string url)
        {
            var client = new VisionServiceClient(ApiKey);
            var result = await client.RecognizeTextAsync(url);
            return result.ToString();
        }


        public async Task<string> GetTextAsync(Stream stream)
        {
            var client = new VisionServiceClient(ApiKey);
            var result = await client.RecognizeTextAsync(stream);
            return result.ToString();
        }

        public async Task<List<string>> GetTextAsyncToList(Stream stream)
        {
            var client = new VisionServiceClient(ApiKey);
            var result = await client.RecognizeTextAsync(stream,"pt",true);
            return LogOcrResultsToList(result);
    }

        protected List<string> LogOcrResultsToList(OcrResults results)
        {
            List<string> stringBuilder = new List<string>();
            if (results != null && results.Regions != null)
            {

                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Add(word.Text);
                        }
                   }
                }
            }
            return stringBuilder;
        }


        protected string LogOcrResults(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (results != null && results.Regions != null)
            {
                stringBuilder.Append(" ");
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }

    }
}
