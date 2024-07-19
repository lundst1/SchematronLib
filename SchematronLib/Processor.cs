using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchematronLib
{
    public class Processor
    {
        private XMLFile xmlFile;
        private SchematronFile schemaTronFile;
        
        public XMLFile XMLFile
        {
            get { return xmlFile; }
        }
        public SchematronFile SchemaTronFile
        {
            get { return schemaTronFile; }
        }

        public Processor(string xmlFile, string schematronFile)
        {
            this.xmlFile = new XMLFile(xmlFile);
            this.schemaTronFile = new SchematronFile(schematronFile);
        }
    }
}
