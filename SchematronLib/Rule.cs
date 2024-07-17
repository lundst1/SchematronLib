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
        //Private variable 
        private string context;
        private string definition;
        private string message;
        private bool assert;
        private bool report;

        private Rule(string context, string definition, string message, bool assert, bool report)
        {
            this.context = context;
            this.definition = definition;
            this.message = message;
            this.assert = assert;
            this.report = report;
        }

    }
}
