using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    public class Phase
    {
        //Private string variable for the pattern id.
        private string id;
        //Private list for active patterns. Holds ids to the pattern.
        private List<string> activePatterns = new List<string>();
        /// <summary>
        /// Public property for pattern id.
        /// Read access.
        /// </summary>
        public string Id
        {
            get { return id; }
        }
        /// <summary>
        /// Public property for ids of which patterns that are active.
        /// Reac access.
        /// </summary>
        public List<string> ActivePatterns 
        {
            get { return activePatterns; }
        }
        /// <summary>
        /// Constructor for class Phase.
        /// Writes id to global variable and calls method Parse.
        /// </summary>
        /// <param name="phase">Instance of class XElement, containing phase.</param>
        /// <param name="nameSpace">The namespace of the schema.</param>
        public Phase(XElement phase, XNamespace nameSpace) 
        {
            this.id = phase.Attribute("id").Value;
            Parse(phase, nameSpace);
        }
        /// <summary>
        /// Method for reading XML to data structures.
        /// Iterates over XML elements and adds active pattern ids to List activePatterns.
        /// </summary>
        /// <param name="phase">Instance of class XElement. Contains elements phase.</param>
        /// <param name="nameSpace">The namespace of the schematorn schema.</param>
        private void Parse(XElement phase, XNamespace nameSpace)
        {
            IEnumerable<XElement> apElems = from apElem in phase.Elements(nameSpace + "active") select apElem;

            foreach (XElement apElem in apElems)
            {
                activePatterns.Add(apElem.Attribute("pattern").Value);
            }
        } 
    }
}
