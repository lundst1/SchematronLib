using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        private Dictionary<string, string> diagnostics = new Dictionary<string, string>();
        private Dictionary<string, Phase> phases = new Dictionary<string, Phase>();

        public List<Pattern> PatternList
        {
            get { return patternList; }
        }
        public Dictionary<string, Phase> Phases
        {
            get { return phases; }
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
            IEnumerable<XElement> diagnosticElements = from diagnosticElement in Elements.Root.Elements(NameSpace + "diagnostics").Elements(NameSpace + "diagnostic") select diagnosticElement;
            IEnumerable<XElement> phases = from phase in Elements.Root.Elements(NameSpace + "phase") select phase;

            foreach (XElement diagnosticElement in diagnosticElements)
            {
                string id = diagnosticElement.Attribute("id").Value;
                string message = diagnosticElement.Value;

                diagnostics[id] = message;
            }
            
            foreach (XElement pattern in patterns) 
            {
                Pattern p = new Pattern(pattern, diagnostics, NameSpace);
                patternList.Add(p);
            }
            
            foreach (XElement phase in phases)
            {
                Phase p = new Phase(phase, NameSpace);
                Phases[p.Id] = p;
            }

            
        }

    }
}
