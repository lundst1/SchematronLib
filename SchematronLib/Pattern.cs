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
        //Private variable for asserts.
        private IEnumerable<XElement> asserts;
        //Private variable for reports.
        private IEnumerable<XElement> reports;
        /// <summary>
        /// Public property for the if of the pattern.
        /// Read access.
        /// </summary>
        public string Id
        {
            get { return id; }
        }
        /// <summary>
        /// Public property for if the pattern is active.
        /// Both read and write access.
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }
        /// <summary>
        /// Public property for the list of rules.
        /// Read access. 
        /// </summary>
        public List<Rule> RuleList
        {
            get { return ruleList; }
        }
        /// <summary>
        /// The constructor for the class Pattern.
        /// Calls the method for parsing the content of the element pattern.
        /// </summary>
        /// <param name="pattern">XElement representation of pattern.</param>
        /// <param name="diagnostics">Diagnostic elements with messages for asserts and reports.</param>
        /// <param name="nameSpace">The namespace of the Schematron.</param>
        public Pattern(XElement pattern, Dictionary<string, string> diagnostics, XNamespace nameSpace) 
        {
            if ((string)pattern.Attribute("id") != null)
            {
                this.id = pattern.Attribute("id").Value;
            }
            
            Parse(pattern, diagnostics, nameSpace);
        }
        /// <summary>
        /// Parses the content of element pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="diagnostics"></param>
        /// <param name="nameSpace"></param>
        private void Parse(XElement pattern, Dictionary<string, string> diagnostics, XNamespace nameSpace)
        {
            IEnumerable<XElement> patternVariables = from patternVariable in pattern.Elements(nameSpace + "let") select patternVariable;
            var ruleElements = from r in pattern.Elements(nameSpace + "rule") where (string) r.Attribute("abstract") == null select r;
            
            foreach (var r in ruleElements)
            {
                string context = r.Attribute("context").Value.ToString();

                asserts = from assert in r.Elements(nameSpace + "assert") select assert;
                reports = from report in r.Elements(nameSpace + "report") select report;

                IEnumerable<XElement> ruleVariables = from ruleVariable in r.Elements(nameSpace + "let") select ruleVariable;
                IEnumerable<XElement> extends = from extendsElem in r.Elements(nameSpace + "extends").Where(e => e.Attribute("rule") != null) select extendsElem;
                IEnumerable<XElement> extendsHref = from extendsElem in r.Elements(nameSpace + "extends").Where(e => e.Attribute("href") != null) select extendsElem;

                if (extends.Any())
                {
                    HandleAbstractRules(extends, pattern, nameSpace);   
                }
                if (extendsHref.Any())
                {
                    HandleAbstractRules(extendsHref, nameSpace);
                }

                Rule rule = new Rule(context, ruleVariables, patternVariables, asserts, reports, diagnostics);
                ruleList.Add(rule);
            }
        }

        /// <summary>
        /// Method for handling abstract rules.
        /// Queries the pattern after pattern after abstract rules with ids that match the extend elements id.
        /// </summary>
        /// <param name="extends">Elements with name extends that are found in an element rule.</param>
        /// <param name="pattern">XElement representation of the node pattern.</param>
        /// <param name="nameSpace">The namespace of the Schematron file</param>
        /// <returns>Returns a tuple containing extended asserts and extended reports.</returns>
        private void HandleAbstractRules(IEnumerable<XElement> extends, XElement pattern, XNamespace nameSpace)
        {
            IEnumerable<XElement> extendedAsserts = null;
            IEnumerable<XElement> extendedReports = null;

            foreach (XElement extendsElem in extends)
            {
                string id = extendsElem.Attribute("rule").Value;

                IEnumerable<XElement> extendedRules = from extendedRule in pattern.Elements(nameSpace + "rule") where extendedRule.Attribute("abstract")?.Value == "true" && extendedRule.Attribute("id")?.Value == id select extendedRule;

                foreach (XElement extendedRule in extendedRules)
                {
                    extendedAsserts = from assert in extendedRule.Elements(nameSpace + "assert") select assert;
                    extendedReports = from report in extendedRule.Elements(nameSpace + "report") select report;
                }
            }

            asserts = asserts.Concat(extendedAsserts);
            reports = reports.Concat(extendedReports);
        }
        private void HandleAbstractRules(IEnumerable<XElement> extends, XNamespace nameSpace)
        {
            IEnumerable<XElement> extendedAsserts = null;
            IEnumerable<XElement> extendedReports = null;

            foreach(XElement extendsElem in extends)
            {
                string href = extendsElem.Attribute("href").Value;
                XDocument extendedDoc = XDocument.Load(href);

                extendedAsserts = from extendedAssert in extendedDoc.Root.Elements(nameSpace + "assert") select extendedAssert;
                extendedReports = from extendedReport in extendedDoc.Root.Elements(nameSpace + "report") select extendedReport;
            }

            asserts = asserts.Concat(extendedAsserts);
            reports = reports.Concat(extendedReports);
        }
    }
}

