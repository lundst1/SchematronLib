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
            List<Rule> rules = schematronFile.RuleList;

            document.Valid = true;

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

                            document.Messages.Add(assert.Message);
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
        private string HandleValueOf(string message, XElement node)
        {
            string newMessage = new string(message);

            foreach (Match match in Regex.Matches(message, "[<][^>]+[>]")) // TODO: Det här regexet kommer plocka upp felaktiga taggar ex <abc>. Åtgärda
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
