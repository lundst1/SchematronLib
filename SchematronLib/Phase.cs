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
        public string Id
        {
            get { return id; }
        }
        /// <summary>
        /// Public property for the variable activePatterns.
        /// Reac access.
        /// </summary>
        public List<string> ActivePatterns 
        {
            get { return activePatterns; }
        }
        public Phase(XElement phase, XNamespace nameSpace) 
        {
            this.id = phase.Attribute("id").Value;
            Parse(phase, nameSpace);
        }
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
