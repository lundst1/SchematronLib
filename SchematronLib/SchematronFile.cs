using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace SchematronLib
{
    public class SchematronFile : XMLFile
    {
        private List<Rule> ruleList = new List<Rule>();
        public List<Rule> RuleList
        {
            get { return ruleList; }
        }
        public SchematronFile(string filename) : base(filename) 
        {
            Parse();
        }

        private void Parse()
        {
            var ruleElements = from r in Elements.Element(NameSpace + "pattern").Elements(NameSpace + "rule") select r;
            
            foreach (var r in ruleElements) 
            {
                string context = r.Attribute("context").Value.ToString();
                Rule rule = new Rule(context);

                IEnumerable<XElement> asserts = from assert in r.Elements(NameSpace + "assert") select assert;
                IEnumerable<XElement> reports = from report in r.Elements(NameSpace + "report") select report;

                foreach (XElement assert in asserts)
                {
                    rule.Asserts.Add(new RuleDefinition { Test = assert.Attribute("test").Value, Message=assert.Value }) ;
                }

                foreach (XElement report in reports)
                {
                    rule.Reports.Add(new RuleDefinition { Test = report.Attribute("test").Value, Message = report.Value });
                }

                ruleList.Add(rule);
            }
        }

    }
}
