using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Processor
    {
        //Private variable for XML file.
        private Document document;
        //Private variable for the xml file stored as an instance of XDocument.
        private XDocument elements;
        //Private variable for the Schematron file.
        private SchematronFile schematronFile;
        private Utilities utils = new Utilities();
        /// <summary>
        /// Public property the xml file.
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
        /// <param name="document">Path to the XML file.</param>
        /// <param name="schematronFile">Path to the Schematron file.</param>
        public Processor(string document, string schematronFile)
        {
            this.document = new Document(document);
            this.elements = this.document.Elements;
            this.schematronFile = new SchematronFile(schematronFile);
        }
        /// <summary>
        /// Constructor for processor class.
        /// </summary>
        /// <param name="document">XML-file to be validated as an instance class Document.</param>
        /// <param name="schematronFile">The schematron schema as an instance of class SchematronFile.</param>
        public Processor(Document document, SchematronFile schematronFile)
        {
            this.document = document;
            this.elements = this.document.Elements;
            this.schematronFile = schematronFile;
        }
        /// <summary>
        /// Constructor for processor class.
        /// </summary>
        /// <param name="document">XML-file to be validated as an instance class XDocument</param>
        /// <param name="schematronFile">The schematron schema as an instance of class XDocument.</param>
        public Processor(XDocument document, XDocument schematronFile)
        {
            this.document = new Document(document);
            this.elements = this.document.Elements;
            this.schematronFile = new SchematronFile(schematronFile);
        }
        /// <summary>
        /// Method for processing the rules and the document.
        /// Iterates through all patterns and calls method to process the rules in the pattern.
        /// </summary>
        public void Process()
        {
            document.Valid = true;
            List<Pattern> patternList = schematronFile.PatternList;

            foreach (Pattern pattern in patternList)
            {
                List<Rule> rules = pattern.RuleList;
                Process(rules);
            }
        }
        /// <summary>
        /// Method for processing the rules and the document.
        /// Iterates through all patterns who are active and calls method to process the rules in the pattern.
        /// </summary>
        /// <param name="phaseList">List of phases that are used.</param>
        public void Process(List<string> phaseList)
        {
            document.Valid = true;
            List<string> activePatterns = GetActivePatterns(phaseList);
            List<Pattern> patternList = schematronFile.PatternList;

            foreach (Pattern pattern in from i in patternList where activePatterns.Contains(i.Id) select i)
            {
                List<Rule> rules = pattern.RuleList;
                Process(rules);
            }

        }
        private void Process(List<Rule> rules)
        { 

            foreach (Rule rule in rules)
            {
                string context = rule.Context;
                List<RuleContent> asserts = rule.Asserts;
                List<RuleContent> reports = rule.Reports;

                IEnumerable<object> results = (IEnumerable<object>)System.Xml.XPath.Extensions.XPathEvaluate(elements, context);

                foreach (XElement element in results)
                {
                    foreach (RuleContent assert in asserts)
                    {
                        ProcessAsserts(assert, element);
                    }
                    foreach (RuleContent report in reports)
                    {
                        ProcessReports(report, element);
                    }
                }
            }
        }
        private void ProcessAsserts(RuleContent assert, XElement element)
        {
            Console.WriteLine(assert.TestString);
            bool assertValid = assert.Test(element);

            if (assertValid)
            {
                Console.WriteLine("Assert successfull");
            }
            else
            {
                string assertMessage = assert.Message;

                assertMessage = HandleValueOf(assertMessage, element);
                Console.WriteLine(assertMessage);

                document.Messages.Add(assertMessage);
                document.Valid = false;
            }
        }
        private void ProcessReports(RuleContent report, XElement element)
        {
            bool reportResult = report.Test(element);

            if (!reportResult)
            {
                Console.WriteLine(report.Message);

                document.Messages.Add(report.Message);
            }
        }
        private List<string> GetActivePatterns(List<string> phaseList)
        {
            List<string> activePatterns = new List<string>();
            
            foreach(string phaseId in phaseList)
            {
                Phase phase = schematronFile.Phases[phaseId];
                activePatterns.AddRange(phase.ActivePatterns);
            }

            return activePatterns;
        }
        /// <summary>
        /// Method that handles the tag value-of.
        /// Extracts and parses the tag.
        /// The contents of select is used to get some value from the XML document.
        /// </summary>
        /// <param name="message">The message that contains the tag</param>
        /// <param name="node">The XML document with the content.</param>
        /// <returns></returns>
        private string HandleValueOf(string message, XElement node)
        {
            string newMessage = new string(message);

            foreach (Match match in Regex.Matches(message, "<[^\\/>]+\\/>")) 
            {
                string m = match.Value;
                XElement valueOf = XElement.Parse(m);
                string selectValue = valueOf.Attribute("select").Value;

                if (selectValue != null)
                {
                    string result = node.Element(selectValue).Value;

                    if (result != string.Empty)
                    {
                        newMessage = Regex.Replace(newMessage, m, result);
                    }
                }
            }

            return (newMessage);
        }
    }
}
