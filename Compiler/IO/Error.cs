using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Tokenization;

namespace Compiler.IO
{
    /// <summary>
    /// Class to be used as a simple data structure for storing error messages
    /// To be used by the ErrorReporter class
    /// </summary>
    class Error
    {
        /// <summary>
        /// Token Associated with the error
        /// </summary>
        public Position Position { get; private set; }
        /// <summary>
        /// message associated with the error
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Create new Error with token and message
        /// </summary>
        /// <param name="token">Token that the error occurred on</param>
        /// <param name="message">Error message</param>
        public Error(Position position, string message)
        {
            Position = position;
            Message = message; 
        }
        /// <summary>
        /// A formated error message of this error as 
        /// Formatting is as follows "Error:{Message}, At:{Token.ToString()}"
        /// </summary>
        /// <returns>A formated error message</returns>
        public override string ToString()
        {
            return $"Error: {Message}\nOn: {Position.ToString()}.";
        }
    }
}
