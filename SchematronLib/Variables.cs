using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchematronLib
{
    /// <summary>
    /// Class that handles specific logic regarding variables. 
    /// </summary>
    public class Variables
    {
        private Dictionary<string, string> variables = new Dictionary<string, string>();
        /// <summary>
        /// Method to add new variables.
        /// Schematron does not allow overwriting variables within the same rule.
        /// Therefore the method does not add variables that exist and returns false if the schematron file attempts to overwrite a variable.
        /// </summary>
        /// <param name="name">The attribute name in the element let.</param>
        /// <param name="value">The attribute value in the element let.</param>
        /// <returns></returns>
        public bool Add(string name, string value)
        {
            bool success = false;

            if (!variables.ContainsKey(name))
            {
                variables[name] = value;
                success = true;
            } 

            return success; 
        }
        /// <summary>
        /// Method to check if the dictionary of variables is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            bool empty = true;

            if (variables.Count > 0)
            {
                empty = false;
            }

            return empty;
        }

        public string Get(string name)
        {
            return variables[name];
        }

    }
}
