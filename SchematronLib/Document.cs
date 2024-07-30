using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    /// <summary>
    /// Class that handles document to be validated.
    /// </summary>
    public class Document : XMLFile
    {
        //Private nullable bool for if the XML file is valid according to some schema.
        private bool? valid;

        private List<string> messages = new List<string>();
        public bool? Valid
        {
            get { return valid; }
            set { valid = value; }
        }
        public Document(string filename) : base(filename) { }  
        public Document(XDocument document) : base(document) { }
    
        public List<string> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

    }
}
