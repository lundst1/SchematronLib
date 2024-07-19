using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SchematronLib
{
    public class Rule
    {
        //Private string variable for  element context.
        private string context;
        //Private list variable for asserts.
        private List<RuleDefinition> asserts = new List<RuleDefinition>();
        //Private list variable for reports.
        private List<RuleDefinition> reports = new List<RuleDefinition>();
        public string Context
        {
            get { return context; }
        }
        /// <summary>
        /// Public property for variable asserts.
        /// Both read and write access.
        /// </summary>
        public List<RuleDefinition> Asserts
        {
            get { return asserts; }
            set { asserts = value; }
        }
        /// <summary>
        /// Public property for variable reports.
        /// Both read and write access.
        /// </summary>
        public List<RuleDefinition> Reports
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
