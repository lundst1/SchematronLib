using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Processor
    {
        //Private variable for XML file.
        private XMLFile xmlFile;
        //Private variable for the Schematron file.
        private SchematronFile schemaTronFile;
        /// <summary>
        /// Public property for variable xmlFile.
        /// Read access.
        /// </summary>
        public XMLFile XMLFile
        {
            get { return xmlFile; }
        }
        /// <summary>
        /// Public property for variable schematronFile.
        /// Read access.
        /// </summary>
        public SchematronFile SchemaTronFile
        {
            get { return schemaTronFile; }
        }
        /// <summary>
        /// Constructor for processor class.
        /// </summary>
        /// <param name="xmlFile">Path to the XML file.</param>
        /// <param name="schematronFile">Path to the Schematron file.</param>
        public Processor(string xmlFile, string schematronFile)
        {
            this.xmlFile = new XMLFile(xmlFile);
            this.schemaTronFile = new SchematronFile(schematronFile);
        }
        public void Process()
        {
            XDocument elements = xmlFile.Elements;
            List<Rule> rules = schemaTronFile.RuleList;

            xmlFile.Valid = true;

            foreach (var el in elements.Descendants())
            {
                string name = el.Name.ToString();

                foreach (Rule rule in rules)
                {
                    
                    string context = rule.Context;
                    List<RuleContent> asserts = rule.Asserts;
                    List<RuleContent> reports = rule.Reports;

                    if (name == context)
                    {
                        foreach(RuleContent assert in asserts)
                        {
                            bool assertValid = assert.Test(el);
                            
                            if (assertValid)
                            {
                                Console.WriteLine("Assert successfull");
                            }
                            else
                            {
                                Console.WriteLine(assert.Message);
                                xmlFile.Valid = false;
                            }

                        }
                        foreach (RuleContent report in reports)
                        {
                            bool reportResult = report.Test(el);

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
}
