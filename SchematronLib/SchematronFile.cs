using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace SchematronLib
{
    /// <summary>
    /// Class for the schematron file.
    /// Parses the file and loads rules into memory.
    /// </summary>

    public class SchematronFile : XMLFile
    {
        //Private variable for list of patterns
        private List<Pattern> patternList = new List<Pattern>();
        
        public List<Pattern> PatternList
        {
            get { return patternList; }
        }
        public SchematronFile(string filename) : base(filename) 
        {
            Parse();
        }
        public SchematronFile(XDocument document) : base(document)
        {
            Parse();
        }

        private void Parse()
        {
            IEnumerable<XElement> patterns = from pattern in Elements.Root.Elements(NameSpace + "pattern") select pattern;

            foreach (XElement pattern in patterns) 
            {
                Pattern p = new Pattern(pattern, NameSpace);
                patternList.Add(p);
            }
        }

    }
}
