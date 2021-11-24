using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PlantDiary21FS7024001.Pages
{
    public class UploadModel : PageModel
    {
        public UploadModel(IHostingEnvironment environment)
        {
            Environment = environment;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public IHostingEnvironment Environment { get; }

        public void OnGet()
        {
            ViewData["Result"] = "No File Submitted";
        }

        public void OnPost()
        {
            // find the path where we want to save the uploaded file.
            string fileName = Upload.FileName;
            string file = Path.Combine(Environment.ContentRootPath, "uploads", fileName);
            // take the uploaded file, write it to disk.
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                Upload.CopyTo(fileStream);
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlNode node = doc.SelectSingleNode("/plant/specimens/specimen[last()]");
            
            // validate our XML.
            ValidateXML(file);
        }

        /// <summary>
        /// Check our XML against our XSD to see if the XML is valid.
        /// </summary>
        /// <param name="file"></param>
        private void ValidateXML(string file)
        {
            // what are my validation settings?
            XmlReaderSettings settings = new XmlReaderSettings();

            // get our XSD.
            string xsdPath = Path.Combine(Environment.ContentRootPath, "uploads", "plants.xsd");
            settings.Schemas.Add(null, xsdPath);

            // we want to validate with XSD
            settings.ValidationType = ValidationType.Schema;

            // a couple of more flags.
            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;

            // which method do I call when there is an error?
            settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(this.ValidationEventHandler);

            // marry together the  XML file we are reading, with the validation settings file.
            XmlReader xmlReader = XmlReader.Create(file, settings);
            // read the file.
            try { 
                while(xmlReader.Read())
                {

                }
                ViewData["Result"] = "Validation Passed.";
            } catch (Exception e)
            {
                ViewData["Result"] = "Validation Failed.  Message: " + e.Message;
            }
            
        }

        /// <summary>
        /// This method is ONLY invoked if/when the XML is invalid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            throw new Exception("There was an error: " + args.Message);
        }
    }
}
