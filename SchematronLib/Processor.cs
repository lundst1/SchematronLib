using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SchematronLib
{
    public class Processor
    {
        //Private variable for XML file.
        private Document document;
        //Private variable for the Schematron file.
        private SchematronFile schematronFile;
        /// <summary>
        /// Public property for variable xmlFile.
        /// Read access.
        /// </summary>
        public Document Document
        {
            get { return document; }
        }
        /// <summary>
        /// Public property for variable schematronFile.
        /// Read access.
        /// </summary>
        public SchematronFile SchemaTronFile
        {
            get { return schematronFile; }
        }
        /// <summary>
        /// Constructor for processor class.
        /// </summary>
        /// <param name="xmlFile">Path to the XML file.</param>
        /// <param name="schematronFile">Path to the Schematron file.</param>
        public Processor(string document, string schematronFile)
        {
            this.document = new Document(document);
            this.schematronFile = new SchematronFile(schematronFile);
        }
        public Processor(Document document, SchematronFile schematronFile)
        {
            this.document = document;
            this.schematronFile = schematronFile;
        }
        public Processor(XDocument document, XDocument schematronFile)
        {
            this.document = new Document(document);
            this.schematronFile = new SchematronFile(schematronFile);
        }
        public void Process()
        {
            XDocument elements = document.Elements;
            List<Rule> rules = schematronFile.RuleList;

            document.Valid = true;

            foreach (Rule rule in rules)
            {
                string context = rule.Context;
                Console.WriteLine(context);
                List<RuleContent> asserts = rule.Asserts;
                List<RuleContent> reports = rule.Reports;

                IEnumerable<object> results = (IEnumerable<object>)System.Xml.XPath.Extensions.XPathEvaluate(elements, context);
            
                foreach (XElement element in results) 
                {
                    Console.WriteLine(element.Name);
                    foreach (RuleContent assert in asserts)
                    {
                        bool assertValid = assert.Test(element);

                        if (assertValid)
                        {
                            Console.WriteLine("Assert successfull");
                        }
                        else
                        {
                            Console.WriteLine(assert.Message);
                            document.Valid = false;
                        }

                    }
                    foreach (RuleContent report in reports)
                    {
                        bool reportResult = report.Test(element);

                        if (!reportResult)
                        {
                            Console.WriteLine(report.Message);
                        }
                    }
                }
            } 
        }
    }
}
