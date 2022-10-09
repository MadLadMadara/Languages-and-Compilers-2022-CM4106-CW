using Compiler.Tokenization;
using System;
using System.Collections.Generic;

namespace Compiler.IO
{
    /// <summary>
    /// An object for reporting errors in the compilation process
    /// </summary>
    public class ErrorReporter
    {
        /// <summary>
        /// List of error tokens that have been reported
        /// </summary>
        private List<Token> ErrorTokens;
        /// <summary>
        /// The number of errors that occourd
        /// </summary>
        public int NumberOfErrors => ErrorTokens.Count;
        /// <summary>
        /// Whether or not any errors have been encountered
        /// </summary>
        public bool HasErrors { get; private set; }
        /// <summary>
        /// Constrictor, initialise ErrorTokens to new token list and HasErrors to false.
        /// </summary>
        public ErrorReporter()
        {
            ErrorTokens = new List<Token>();
            HasErrors = false;
        }
        /// <summary>
        /// Add new error token to ErrorToken list
        /// </summary>
        /// <param name="token">Error token </param>
        public void NewError(Token token)
        {
            if (token.Type == TokenType.Error)
            {  
                HasErrors = true;
                ErrorTokens.Add(token);
            }
        }
    }
}