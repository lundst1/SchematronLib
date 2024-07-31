using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchematronLib
{
    /// <summary>
    /// Class that handles methods that are used in several places.
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
    }
}
