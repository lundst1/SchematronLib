using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SchematronLib
{
    /// <summary>
    /// Class that handles Schematron rule contents.
    /// </summary>
    public class RuleContent
    {
        private Utilities utils = new Utilities();
        public string TestString { get; set; }
        public string Message { get; set; }
    
        public bool Test(XNode node)
        {
            bool result = true;

            var value = System.Xml.XPath.Extensions.XPathEvaluate(node, TestString);

            

            if (utils.XpathResultIsEmpty(value))
            {
                result = false; 
            }

            return result;

        }
    }
}
