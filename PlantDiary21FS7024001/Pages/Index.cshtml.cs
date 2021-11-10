using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using plantfeed;
using specimenfeed;
using weatherfeed;

namespace PlantDiary21FS7024001.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            GenerateBrand();

            using (var webClient = new WebClient())
            {
                // read our weather API key.
                string key = System.IO.File.ReadAllText("WeatherAPIKey.txt");
                string weatherJSON = webClient.DownloadString("https://api.weatherbit.io/v2.0/current?&city=Cincinnati&country=USA&key=" + key);
                Weather weathers = Weather.FromJson(weatherJSON);
                // store the precip.
                long precip = 0;
                foreach (weatherfeed.Datum weather in weathers.Data)
                {
                    precip = weather.Precip;
                    if (precip < 1)
                    {
                        ViewData["Weather"] = "Need to Water";
                    }
                    else
                    {
                        ViewData["Weather"] = "Don't need to water.";
                    }
                }

                // raw string data of plants that like water.
                string plantsJSON = webClient.DownloadString("http://plantplaces.com/perl/mobile/viewplantsjsonarray.pl?WetTolerant=on");

                // put the thirsty plants into a collection.
                List<Plant> plants = Plant.FromJson(plantsJSON);

                // init our dictionary
                IDictionary<long, Plant> allPlants = new Dictionary<long, Plant>();

                // load the plants into the dictionary.
                foreach (Plant plant in plants)
                {
                    allPlants.Add(plant.Id, plant);
                }


                // grab our JSON text. 
                var specimenJSON = webClient.DownloadString("https://www.plantplaces.com/perl/mobile/viewspecimenlocations.pl?Lat=39.14455075&Lng=-84.5093939666667&Range=0.5&Source=location&Version=2");

                // convert raw text to objects.
                SpecimenCollection specimenCollection = SpecimenCollection.FromJson(specimenJSON);

                // let's get our specimens
                List<Specimen> specimens = specimenCollection.Specimens;

                // read the schema.
                string specimenSchema = System.IO.File.ReadAllText("SpecimenSchema.json");
                JSchema schema = JSchema.Parse(specimenSchema);

                // Parse our incoming JSON against the schema.
                JObject jsonObject = JObject.Parse(specimenJSON);

                // a list of reasons why the JSON is not valid.
                IList<string> validationEvents = new List<string>();

                if (jsonObject.IsValid(schema, out validationEvents))
                {
                    // parse out the specimens.
                    // filter the specimens to those that like water.
                    List<Specimen> waterMeSpecimens = new List<Specimen>();
                    foreach (Specimen specimen in specimens)
                    {
                        if (allPlants.ContainsKey(specimen.PlantId))
                        {
                            waterMeSpecimens.Add(specimen);
                        }
                    }

                    ViewData["Specimens"] = waterMeSpecimens;
                }
                else
                {
                    string error = "";
                    foreach (string evt in validationEvents)
                    {
                        error = error + evt;
                        ViewData["Error"] = error;
                    }
                    ViewData["Specimens"] = new List<Specimen>();
                }

            }

        }

        private void GenerateBrand()
        {
            string brandName = Request.Query["BrandName"];
            int yearStarted = 2006;
            if (brandName == null || brandName.Length == 0)
            {
                brandName = "My Plant Diary";
            }
            ViewData["brandName"] = brandName + yearStarted;
        }
    }
}
