using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections;
using System.Text.Json.Serialization;
using System.IO;
using RentEstimator;
using System.Windows;

namespace RentCalculator
{
    internal class ReadandParseJsonFile
    {
        private readonly string _jsonFilePath;
        public ReadandParseJsonFile(string jsonFilePath)
        {
            bool isFile = File.Exists(jsonFilePath);

            if (isFile)
            {
                _jsonFilePath = jsonFilePath;
            }
            else
            {
                throw new FileNotFoundException("Missing files", jsonFilePath);
            }
        }

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public void StreamWrite(object obj)
        {
            using (var fileStream = File.Create(_jsonFilePath))
            {
                var utf8JsonWriter = new Utf8JsonWriter(fileStream);
                JsonSerializer.Serialize(utf8JsonWriter, obj, _options);
            }
            
        }

        public async Task StreamWriteAsync(object obj)
        {
            using (var fileStream = File.Create(_jsonFilePath))
            {
                await JsonSerializer.SerializeAsync(fileStream, obj, _options);
            }
            
        }

        public List<UtilitiesModel> ExtractUtilitiesData()
        {
            if (_jsonFilePath != null)
            {
                using (FileStream json = File.OpenRead(_jsonFilePath))
                {
                    var utilitiesData = JsonSerializer.Deserialize<List<UtilitiesModel>>(json, _options);
                    return utilitiesData;
                }
            }

            return null;
        }

        public Dictionary<int, int> ExtractFMRData()
        {
            bool isfileExist = File.Exists(_jsonFilePath);

            if (!isfileExist)
            {
                var Result = MessageBox.Show("The fair market rent file could not be located would you like to create it?", "Missing FMR File", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (Result == MessageBoxResult.Yes)
                {
                    //CreateZeroedFMRFile();
                    paymentStandard openSelectedMenu = new paymentStandard();
                    openSelectedMenu.Show();
                }
                else if (Result == MessageBoxResult.No)
                {
                    Environment.Exit(0);
                }
            }

            isfileExist = File.Exists(_jsonFilePath);

            if (isfileExist)
            {
                using (FileStream json = File.OpenRead(_jsonFilePath))
                {
                    var fmrData = JsonSerializer.Deserialize<Dictionary<int, int>>(json, _options);

                    return fmrData;
                }
            }

            return null;
        }

        public Dictionary<string, string> ExtractFirstPageData()
        {
            if (_jsonFilePath != null)
            {
                using (FileStream json = File.OpenRead(_jsonFilePath))
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _options);

                    return data;
                }
            }

            return null;
        }

        public Dictionary<string, List<Dictionary<string, object>>> ExtractPageData()
        {
            if (_jsonFilePath != null)
            {
                using (FileStream json = File.OpenRead(_jsonFilePath))
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(json, _options);

                    return data;
                }
            }

            return null;
        }
    }
}
