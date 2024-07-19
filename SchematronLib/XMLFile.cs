using System.Linq;
using System.Xml.Linq;

namespace SchematronLib
{
    public class XMLFile
    {
        //Private string variable for the file name.
        private string filename;
        //Private string variable for the namespace
        private XNamespace nameSpace;
        //Private variable for loading all elements into memory.
        private XElement elements;
        
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        public XNamespace NameSpace
        {
            get { return nameSpace; }
        }
        public XElement Elements
        {
            get { return elements; }
        }
        public XMLFile(string filename)
        {
            this.filename = filename;
            ReadFile();
            SetNameSpace();

        }
        private void ReadFile()
        {
            elements = XElement.Load(filename);
        }
        private void SetNameSpace()
        {
            nameSpace = elements.GetDefaultNamespace();
        }
        //Todo: Antingen returnerar du listan så får Processor iterera över den
        // Eller så publicerar du en händelse som processor lyssnar på och jämför regler med. 
    }
}
