using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using specimenfeed;

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
            string brandName = Request.Query["BrandName"];
            int yearStarted = 2006;
            if (brandName == null || brandName.Length == 0) {
                brandName = "My Plant Diary";
            }
            ViewData["brandName"] = brandName + yearStarted;

            using (var webClient = new WebClient())
            {
                // grab our JSON text. 
                var specimenJSON = webClient.DownloadString("https://www.plantplaces.com/perl/mobile/viewspecimenlocations.pl?Lat=39.14455075&Lng=-84.5093939666667&Range=0.5&Source=location&Version=2");

                // convert raw text to objects.
                SpecimenCollection specimenCollection = SpecimenCollection.FromJson(specimenJSON);

                // let's get our specimens
                List<Specimen> specimens = specimenCollection.Specimens;

                ViewData["Specimens"] = specimens;
            }

        }
    }
}
