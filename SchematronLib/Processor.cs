using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Processor
    {
        //Private variable for XML file.
        private Document document;
        //Private variable for the Schematron file.
        private SchematronFile schematronFile;
        private Utilities utils = new Utilities();
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
            //List<Rule> rules = schematronFile.RuleList;
            List<Pattern> patternList = schematronFile.PatternList;
            document.Valid = true;

            foreach(Pattern pattern in patternList)
            {
                List<Rule> rules = pattern.RuleList;

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
                        foreach (RuleContent report in reports)
                        {
                            bool reportResult = report.Test(element);

                            if (!reportResult)
                            {
                                Console.WriteLine(report.Message);

                                document.Messages.Add(report.Message);
                            }
                        }
                    }
                }
            }
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
