using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchematronLib
{
    /// <summary>
    /// Class that utility methods.
    /// </summary>
    internal class Utilities
    {
        /// <summary>
        /// Method for checking if xpathevaluations are empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns true if the xpathevaluation is empty.</returns>
        public bool XpathResultIsEmpty(object value)
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
        /// <summary>
        /// Method to return content of an element, when the content contains XML-like formatting.
        /// </summary>
        /// <param name="element">The element with the content.</param>
        /// <returns>Returns the content of the element as text regardless of content.</returns>
        public string GetTextContentWithElements(XElement element)
        {
            return string.Join("", element.Nodes().Select(node =>
            {
                if (node is XText textNode)
                {
                    return textNode.Value;
                }
                else if (node is XElement elementNode)
                { 
                    var elementWithoutNamespace = new XElement(elementNode.Name.LocalName,
                                                               elementNode.Attributes().Where(a => !a.IsNamespaceDeclaration),
                                                               elementNode.Nodes());
                    return elementWithoutNamespace.ToString(SaveOptions.DisableFormatting);
                }
                else
                {
                    return string.Empty;
                }
            }));
        }
    }
}
