using System.Linq;
using System.Xml.Linq;

namespace SchematronLib
{
    public class XMLFile
    {
        //Private string variable for the file name.
        private string filename;
        //Private variable for the XML files namespace.
        private XNamespace nameSpace;
        //Private variable for loading all elements into memory.
        private XDocument elements;
        //Private bool variable for if the XML file is well formed
        private bool wellFormed = true;
       /// <summary>
       /// Property for variable filename.
       /// Both read and write access.
       /// </summary>
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        /// <summary>
        /// Property for variable namespace.
        /// Read access.
        /// </summary>
        public XNamespace NameSpace
        {
            get { return nameSpace; }
        }
        /// <summary>
        /// Property for variable elements.
        /// Read access.
        /// </summary>
        public XDocument Elements
        {
            get { return elements; }
        }
       
        public XMLFile(string filename)
        {
            this.filename = filename;
            ReadFile();
            SetNameSpace();

        }
        /// <summary>
        /// Constructor for Document.
        /// Uses XDocument variable.
        /// </summary>
        /// <param name="elements">An XML-tree saved as a instance of XDocument.</param>
        public XMLFile(XDocument elements)
        {
            this.elements = elements;
            SetNameSpace();
        }
        /// <summary>
        /// Method that reads file into memory as instance of class XDocument.
        /// </summary>
        private void ReadFile()
        {
            elements = XDocument.Load(filename);
        }
        /// <summary>
        /// Setter for target namespace.
        /// </summary>
        private void SetNameSpace()
        {
            nameSpace = elements.Root.GetDefaultNamespace();
        }
    }
}
