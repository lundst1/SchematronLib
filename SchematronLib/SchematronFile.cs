using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace SchematronLib
{
    /// <summary>
    /// Class for the schematron file.
    /// Parses the file and loads rules into memory.
    /// </summary>
    public class SchematronFile : XMLFile
    {
        //Private variable for list of rules
        private List<Rule> ruleList = new List<Rule>();
        /// <summary>
        /// Property to access variable ruleList.
        /// Read access. 
        /// </summary>
        public List<Rule> RuleList
        {
            get { return ruleList; }
        }
        public SchematronFile(string filename) : base(filename) 
        {
            Parse();
        }
        public SchematronFile(XDocument document) : base(document)
        {
            Parse();
        }

        private void Parse()
        {
            var ruleElements = from r in Elements.Root.Element(NameSpace + "pattern").Elements(NameSpace + "rule") select r;
            
            foreach (var r in ruleElements) 
            {
                string context = r.Attribute("context").Value.ToString();
                
                IEnumerable<XElement> variables = from variable in r.Elements(NameSpace + "let") select variable;
                IEnumerable<XElement> asserts = from assert in r.Elements(NameSpace + "assert") select assert;
                IEnumerable<XElement> reports = from report in r.Elements(NameSpace + "report") select report;
                
                Rule rule = new Rule(context, variables, asserts, reports);
                ruleList.Add(rule);
            }
        }

    }
}
