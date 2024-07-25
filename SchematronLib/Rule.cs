using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        public string Context
        {
            get { return context; }
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

        public Rule(string context)
        {
            this.context = context;
        }
    }
}
