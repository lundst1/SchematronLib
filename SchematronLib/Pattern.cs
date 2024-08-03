using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Pattern
    {
        private List<Rule> ruleList = new List<Rule>();
        /// <summary>
        /// Property to access variable ruleList.
        /// Read access. 
        /// </summary>
        public List<Rule> RuleList
        {
            get { return ruleList; }
        }
        public Pattern(XElement pattern, XNamespace nameSpace) 
        {
            Parse(pattern, nameSpace);
        }
        private void Parse(XElement pattern, XNamespace nameSpace)
        {
            IEnumerable<XElement> patternVariables = from patternVariable in pattern.Elements(nameSpace + "let") select patternVariable;
            var ruleElements = from r in pattern.Elements(nameSpace + "rule") select r;

            foreach (var r in ruleElements)
            {
                string context = r.Attribute("context").Value.ToString();

                IEnumerable<XElement> ruleVariables = from ruleVariable in r.Elements(nameSpace + "let") select ruleVariable;
                IEnumerable<XElement> asserts = from assert in r.Elements(nameSpace + "assert") select assert;
                IEnumerable<XElement> reports = from report in r.Elements(nameSpace + "report") select report;
                
                Rule rule = new Rule(context, ruleVariables, patternVariables, asserts, reports);
                ruleList.Add(rule);
            }
        }
    }
}

