using System;

using System.Collections.Generic;
using System.IO;
using Google.Apis;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Newtonsoft.Json;
using Google.Apis.Services;
using System.Linq;

namespace ConsoleApp12
{

    class Program
    {
        static void Main(string[] args)
        {
            string _exePath = @"C:\Users\User\source\repos\ConsoleApp12\bin\Debug\netcoreapp3.1";
            string credPath = _exePath + @"\info.json";

            var json = File.ReadAllText(credPath);
            var cr = JsonConvert.DeserializeObject<PersonalServiceAccountCred>(json); // "personal" service account credential

            // Create an explicit ServiceAccountCredential credential
            var xCred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.client_email)
            {
                Scopes = new[] {
                    AnalyticsService.Scope.AnalyticsManageUsersReadonly,
                    AnalyticsService.Scope.AnalyticsReadonly
                }
            }.FromPrivateKey(cr.private_key));

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",  $"{_exePath}"+ @"\info.json");


            GetTextFromImage();
            Console.WriteLine("-------------------------");
            GetFacesFromImage();
        }
        public static void GetFacesFromImage()
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<FaceAnnotation> result = client.DetectFaces(Image.FromFile("testImage.jpg"));
            foreach (FaceAnnotation face in result)
            {
                string poly = string.Join(" - ", face.BoundingPoly.Vertices.Select(v => $"({v.X}, {v.Y})"));
                Console.WriteLine($"Confidence: {(int)(face.DetectionConfidence * 100)}%; BoundingPoly: {poly}");
            }
        }
        public static void GetTextFromImage()
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(Image.FromFile("testText.jpg"));
            foreach (EntityAnnotation text in textAnnotations)
            {
                Console.WriteLine($"Description: {text.Description}");
            }
        }
    }
}
