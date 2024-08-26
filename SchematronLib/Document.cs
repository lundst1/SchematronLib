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
        /// <summary>
        /// Public property for if XML file is valid according to some schema.
        /// Both read and write access. 
        /// </summary>
        public bool? Valid
        {
            get { return valid; }
            set { valid = value; }
        }
        /// <summary>
        /// Constructor for class Document.
        /// </summary>
        /// <param name="filename">The path of the file.</param>
        public Document(string filename) : base(filename) { }
        /// <summary>
        /// Constructor for class Document.
        /// </summary>
        /// <param name="document">An instance of class XDocument.</param>
        public Document(XDocument document) : base(document) { }
        
        public List<string> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

    }
}
