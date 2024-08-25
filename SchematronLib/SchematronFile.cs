using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SchematronLib
{
    /// <summary>
    /// Class for the schematron file.
    /// Parses the file and loads rules into memory.
    /// </summary>
    public class SchematronFile : XMLFile
    {
        private IEnumerable<XElement> patterns;
        //Private variable for list of patterns
        private List<Pattern> patternList = new List<Pattern>();
        private Dictionary<string, string> diagnostics = new Dictionary<string, string>();
        private Dictionary<string, Phase> phases = new Dictionary<string, Phase>();
        private Dictionary<string, XElement> abstractPatternDict = new Dictionary<string, XElement>();

        public List<Pattern> PatternList
        {
            get { return patternList; }
        }
        public Dictionary<string, Phase> Phases
        {
            get { return phases; }
        }
        public SchematronFile(string filename) : base(filename) //TODO: Någonstans härifrån borde det ske någon validering av schematronschemat. 
        {
            Parse();
        }
        public SchematronFile(XDocument document) : base(document)
        {
            Parse();
        }

        private void Parse()
        {
            patterns = from pattern in Elements.Root.Elements(NameSpace + "pattern").Where(e => e.Attribute("is-a") == null && e.Attribute("abstract") == null) select pattern; //Todo: Lägg till where sats som avgränsar till icke-abstrakta patterns.

            IncludeOtherSchemas();
            GetAbstractPatterns();
            ResolveAbstractPatterns();
            ParsePhases();
            ParseDiagnostics();

            foreach (XElement pattern in patterns) 
            {
                Pattern p = new Pattern(pattern, diagnostics, NameSpace);
                patternList.Add(p);
            }
        }
        /// <summary>
        /// Method that queries schema for abstract patterns
        /// Writes abstract patterns to dictionary so that they can be resolved to actual patterns before processing.
        /// </summary>
        private void GetAbstractPatterns()
        {
            IEnumerable<XElement> abstractPatterns = from pattern in Elements.Root.Elements(NameSpace + "pattern").Where(e => e.Attribute("abstract")?.Value == "true") select pattern;

            foreach (XElement abstractPattern in abstractPatterns)
            {
                string id = abstractPattern.Attribute("id").Value;
                abstractPatternDict[id] = abstractPattern;
            }
        }
        /// <summary>
        /// Method that resolves abstract patterns to a non abstract pattern.
        /// Checks if an abstract pattern is being called and resolves variable names.
        /// </summary>
        private void ResolveAbstractPatterns()
        {
            IEnumerable<XElement> usingAbstractPatterns = from pattern in Elements.Root.Elements(NameSpace + "pattern").Where(e => e.Attribute("is-a") != null) select pattern;

            foreach (XElement p in usingAbstractPatterns)
            {
                string id = p.Attribute("is-a").Value;

                if (abstractPatternDict.ContainsKey(id))
                {
                    XElement abstractPattern = abstractPatternDict[id];
                    IEnumerable<XElement> paramElems = from param in p.Elements(NameSpace + "param") select param;
                    XElement newPattern = ReplaceWithParams(abstractPattern, paramElems);
                    newPattern.Attribute("abstract").Remove();
                    patterns = patterns.Append(newPattern);
                }
            }
        }
        /// <summary>
        /// Method to replace abstract pattern variables with real values.
        /// </summary>
        /// <param name="abstractPattern">The abstract pattern</param>
        /// <param name="paramElems">The pattern that uses the abstract pattern.</param>
        /// <returns>Returns a non abstract pattern.</returns>
        private XElement ReplaceWithParams(XElement abstractPattern, IEnumerable<XElement> paramElems)
        {
            XElement pattern = abstractPattern;
            IEnumerable<XElement> descendants = pattern.Descendants();

            foreach (XElement param in paramElems)
            {
                string name = param.Attribute("name").Value;
                string value = param.Attribute("value").Value;

                string regex =  @"\/?\$" + Regex.Escape(name) + @"\/?";

                foreach (XElement elem in descendants)
                {
                    foreach(var attr in elem.Attributes())
                    {
                        attr.Value = Regex.Replace(attr.Value, regex, value);
                    }

                    if (!elem.HasElements)
                    {
                        elem.Value = Regex.Replace(elem.Value, regex, value);
                    }
                }
            }

            return pattern;
        }
        /// <summary>
        /// Method that reads queries schema for phases and saves them to a dictionary, so that they can easily be accessed in parsing.
        /// </summary>
        private void ParsePhases()
        {
            IEnumerable<XElement> phases = from phase in Elements.Root.Elements(NameSpace + "phase") select phase;
            
            foreach (XElement phase in phases)
            {
                Phase p = new Phase(phase, NameSpace);
                Phases[p.Id] = p;
            }
        }
        /// <summary>
        /// Method that reads diagnostic messages. Writes them to dictionary, so they can easily be accessed in parsing.
        /// </summary>
        private void ParseDiagnostics()
        {
            IEnumerable<XElement> diagnosticElements = from diagnosticElement in Elements.Root.Elements(NameSpace + "diagnostics").Elements(NameSpace + "diagnostic") select diagnosticElement;
            ParseDiagnostics(diagnosticElements);
        }
        private void ParseDiagnostics(IEnumerable<XElement> diagnosticElements)
        {
            foreach (XElement diagnosticElement in diagnosticElements)
            {
                string id = diagnosticElement.Attribute("id").Value;
                string message = diagnosticElement.Value;

                diagnostics[id] = message;
            }
        }
        private void IncludeOtherSchemas()
        {
            IEnumerable<XElement> includedSchemas = from includedSchema in Elements.Root.Elements(NameSpace + "include") select includedSchema;

            foreach (XElement includedSchemaElement in includedSchemas)
            {
                string href = includedSchemaElement.Attribute("href").Value;
                XDocument includedSchema = XDocument.Load(href);
                string root = includedSchema.Root.Name.LocalName;

                if (root == "schema")
                {
                    AddSchema(includedSchema);
                }
                if (root == "diagnostics")
                {
                    AddDiagnostics(includedSchema);
                }
                if (root == "pattern")
                {
                    AddPattern(includedSchema);
                }
            }
        }
        private void AddSchema(XDocument schema)
        {
            XElement diagnosticElements = schema.Root.Element(NameSpace + "diagnostics");
            IEnumerable<XElement> patternElements = from patternElement in schema.Root.Elements(NameSpace + "pattern") select patternElement;

            if (diagnosticElements != null)
            {
                AddDiagnostics(diagnosticElements);
            }
            
            AddPattern(patternElements);
        }
        private void AddDiagnostics(XElement diagnostics)
        {
            AddDiagnostics(new XDocument(diagnostics));
        }
        private void AddDiagnostics(XDocument diagnostics)
        {
            IEnumerable<XElement> diagnosticElems = from diagnosticElem in diagnostics.Root?.Elements(NameSpace + "diagnostic") select diagnosticElem;
            ParseDiagnostics(diagnosticElems);
        }
        private void AddPattern(IEnumerable<XElement> patterns)
        {
            foreach(XElement pattern in patterns)
            {
                AddPattern(new XDocument(pattern));
            }
        }
        private void AddPattern(XDocument pattern)
        {
            IEnumerable<XElement> patternElems = from p in pattern.Elements() select p;
            
            if (patternElems != null)
            {
                patterns = patterns.Concat(patternElems);
            }
        }
    }
}
