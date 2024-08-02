using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Rule
    {
        //Private string variable for  element context.
        private string context; 
        //Private list variable for asserts.
        private List<RuleContent> asserts = new List<RuleContent>();
        //Private list variable for reports.
        private List<RuleContent> reports = new List<RuleContent>();
        //Private variable for storing datatype for handling Schematron variables
        private Variables variables = new Variables();
        private Utilities util = new Utilities();
        public string Context
        {
            get { return context; }
        }
        public Variables Variables
        {
            get { return variables; }
            set { variables = value; }
        }
        /// <summary>
        /// Public property for variable asserts.
        /// Both read and write access.
        /// </summary>
        public List<RuleContent> Asserts
        {
            get { return asserts; }
            set { asserts = value; }
        }
        /// <summary>
        /// Public property for variable reports.
        /// Both read and write access.
        /// </summary>
        public List<RuleContent> Reports
        {
            get { return reports; }
            set { reports = value; }
        }
        /// <summary>
        /// Constructor for class Rule.
        /// Calls methods to build content based on children of element rule. 
        /// Rule variables are read first so the in practice can overwrite  pattern variaables.
        /// </summary>
        /// <param name="context">The context of the rule. This is an Xpath statement.</param>
        /// <param name="ruleVariables">Content of elements let when they are children to rule.</param>
        /// <param name="patternVariables">Content of elements let when they are children to pattern.</param>>
        /// <param name="asserts">Content of elements assert.</param>
        /// <param name="reports">Content of elements report.</param>
        public Rule(string context, IEnumerable<XElement> ruleVariables, IEnumerable<XElement> patternVariables, IEnumerable<XElement> asserts, IEnumerable<XElement> reports)
        {
            this.context = context;
            ReadVariables(ruleVariables);
            ReadVariables(patternVariables);
            ReadAsserts(asserts);
            ReadReports(reports);
        }
        private void ReadVariables(IEnumerable<XElement> variables)
        {
            foreach (XElement variable in variables)
            {
                if (variable != null)
                {
                    Variables.Add(variable.Attribute("name").Value, variable.Attribute("value").Value);
                }
            }
        }
        private void ReadAsserts(IEnumerable<XElement> asserts)
        {
            foreach (XElement assert in asserts)
            {
                if (assert != null)
                {
                    string testString = assert.Attribute("test").Value;
                    string message = util.GetTextContentWithElements(assert);

                    testString = ReplaceVariables(testString);
                    message = ReplaceVariables(message);

                    Asserts.Add(new RuleContent { TestString = testString, Message = message });
                }
            }
        }
        private void ReadReports(IEnumerable<XElement> reports)
        {
            foreach (XElement report in reports)
            {
                if (report != null)
                {
                    string testString = report.Attribute("test").Value;
                    string message = util.GetTextContentWithElements(report);

                    testString = ReplaceVariables(testString);
                    message = ReplaceVariables(message);

                    Reports.Add(new RuleContent { TestString = testString, Message = message });
                }
            }
        }
        private string ReplaceVariables(string stringValue)
        {
            string newStringValue = new string(stringValue);

            foreach(Match match in Regex.Matches(stringValue, @"(\$)(\w+)"))
            {
                string variableName = match.Groups[2].Value;
                string variableValue = Variables.Get(variableName);

                newStringValue = Regex.Replace(newStringValue, @"\$" + variableName, variableValue);
            }

            return (newStringValue);
        }
        
    }
}
