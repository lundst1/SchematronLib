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
        public string TestString { get; set; }
        public string Message { get; set; }
    
        public bool Test(XNode node)
        {
            bool result = true;

            var value = System.Xml.XPath.Extensions.XPathEvaluate(node, TestString);

            

            if (IsEmpty(value))
            {
                result = false; 
            }

            return result;

        }

        private bool IsEmpty(object value)
        {
            if (value == null)
            {
                return true;
            }

            switch (value)
            {
                case IEnumerable<object> enumerableResult:
                    return !enumerableResult.Any();
                case string strResult:
                    return string.IsNullOrEmpty(strResult);
                case bool boolResult:
                    return !boolResult;
                case double doubleResult:
                    return double.IsNaN(doubleResult);
                default:
                    return false;
            }
        }

    }
}
