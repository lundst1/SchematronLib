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
       
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        public XNamespace NameSpace
        {
            get { return nameSpace; }
        }
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
        public XMLFile(XDocument elements)
        {
            this.elements = elements;
            SetNameSpace();
        }
        private void ReadFile()
        {
            elements = XDocument.Load(filename);
        }
        private void SetNameSpace()
        {
            nameSpace = elements.Root.GetDefaultNamespace();
        }
    }
}
