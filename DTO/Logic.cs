using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;

namespace DTO
{
    class ReportGenerator
    {
        public void Gererate(CountryDto dto)
        {
            FileInfo file = new FileInfo(@"C:\DocumentTemplates\CountryTemplate.docx");
            string pathOutput = @"C:\GeneratedReports" + @"\" + $"ReportForCountry_{dto.Name}.docx";
            if (File.Exists(pathOutput))
            {
                File.Delete(pathOutput);
            }
            File.Copy(file.FullName, pathOutput);

            var valuesToFill = new Content(
                new FieldContent("Name", dto.Name),
                new FieldContent("Alpha3Code", dto.Alpha3Code),
                new FieldContent("Population", dto.Population.ToString()),
                new FieldContent("Gini", dto.Gini.ToString())
               );

            using (var outputDocument = new TemplateProcessor(pathOutput)
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
        }
    }
    class WebServiceCountryLoader
    {
        private HttpClient _httpClient;
        private string _baseUri = "https://restcountries.eu/rest/v2/alpha/";

        public CountryDto GetCountry(CountryResearchRequest countryRequest)
        {
            string completeUrl = _baseUri + countryRequest.RequestedCountryCode;
            Uri uriBody = new Uri(completeUrl);

            string jsonResult = _httpClient.GetStringAsync(uriBody).Result;

            try
            {
                CountryDto dto = JsonConvert.DeserializeObject<CountryDto>(jsonResult);
                return dto;
            }
            catch
            {
                return new CountryDto()
                {
                    Name = "rand",
                    Alpha3Code = "rand",
                    Capital = "rand",
                    Gini = 0,
                    Population = 0,
                    QueueName = "BadData"
                };
            }
            
        }

        public WebServiceCountryLoader()
        {
            _httpClient = new HttpClient();
        }
    }
    public class Logic
    {
        public Action<CountryResearchRequest> OnReceiveInRestService =
            (CountryResearchRequest message) =>
            {
                WebServiceCountryLoader webService = new WebServiceCountryLoader();
                CountryDto dto = webService.GetCountry(message);

                MessageBus bus = new MessageBus();
                bus.PushMessageToQueue("CountryDto", dto);

            };

        public void OnReceiveInDocumentService(CountryDto message)
        {
            ReportGenerator r = new ReportGenerator();
            r.Gererate(message);
        }
    }
}
