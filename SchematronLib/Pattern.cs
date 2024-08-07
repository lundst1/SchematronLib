using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Pattern
    {
        //Private variable for the id of the pattern.
        private string? id;
        //Private variable for if the pattern is active. Default is true and can be changed by the element phase.
        private bool active = true;
        private List<Rule> ruleList = new List<Rule>();
        /// <summary>
        /// Public property for variable id.
        /// Read access.
        /// </summary>
        public string Id
        {
            get { return id; }
        }
        /// <summary>
        /// Public property for the variable active.
        /// Both read and write access.
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }
        /// <summary>
        /// Property to access variable ruleList.
        /// Read access. 
        /// </summary>
        public List<Rule> RuleList
        {
            get { return ruleList; }
        }
        
        public Pattern(XElement pattern, Dictionary<string, string> diagnostics, XNamespace nameSpace) 
        {
            if ((string)pattern.Attribute("id") != null)
            {
                this.id = pattern.Attribute("id").Value;
            }
            
            Parse(pattern, diagnostics, nameSpace);
        }
        private void Parse(XElement pattern, Dictionary<string, string> diagnostics, XNamespace nameSpace)
        {
            IEnumerable<XElement> patternVariables = from patternVariable in pattern.Elements(nameSpace + "let") select patternVariable;
            var ruleElements = from r in pattern.Elements(nameSpace + "rule") select r;

            foreach (var r in ruleElements)
            {
                string context = r.Attribute("context").Value.ToString();

                IEnumerable<XElement> ruleVariables = from ruleVariable in r.Elements(nameSpace + "let") select ruleVariable;
                IEnumerable<XElement> asserts = from assert in r.Elements(nameSpace + "assert") select assert;
                IEnumerable<XElement> reports = from report in r.Elements(nameSpace + "report") select report;
                
                Rule rule = new Rule(context, ruleVariables, patternVariables, asserts, reports, diagnostics);
                ruleList.Add(rule);
            }
        }
    }
}

