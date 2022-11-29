using Compiler.Nodes;
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
        /// List of errors that have been reported
        /// </summary>
        private List<Error> Errors;

        /// <summary>
        /// Returns a read-only copy of the Errors List property 
        /// </summary>
        public IReadOnlyList<Object> ListOfErrors => Errors.AsReadOnly();

        /// <summary>
        /// The number of errors that occurred
        /// </summary>
        public int NumberOfErrors => Errors.Count;

        /// <summary>
        /// Whether or not any errors have been encountered
        /// </summary>
        public bool HasErrors { get; private set; }

        /// <summary>
        /// Constrictor, initialize ErrorTokens to new token list and HasErrors to false.
        /// </summary>
        public ErrorReporter()
        {
            Errors = new List<Error>();
            HasErrors = false;
        }

        /// <summary>
        /// Reports a new error to be added to the error list from a token
        /// </summary>
        /// <param name="token">Token that the error occurred on</param>
        /// <param name="message">Description of the error</param>
        public void NewError(Token token, string message)
        {
            HasErrors = true;
            Errors.Add(new Error(token.Position, message));
        }

        /// <summary>
        /// Reports a new error to be added to the error list from a AST Node
        /// </summary>
        /// <param name="node">AST node that the error occurred on</param>
        /// <param name="message">Description of the error</param>
        public void NewError(IAbstractSyntaxTreeNode node, string message)
        {
            HasErrors = true;
            Errors.Add(new Error(node.Position, message));
        }

        /// <summary>
        /// Reports a new error to be added to the error list that dose not require a position.
        /// These are for general errors. 
        /// </summary>
        /// <param name="message">Description of the error</param>
        public void NewError(string message)
        {
            HasErrors = true;
            Errors.Add(new Error(new Position(-1, -1), message));
        }

        /// <summary>
        /// Creates a formated output of the list of errors that have been add via the NewError method
        /// </summary>
        /// <returns>Formatted string of errors</returns>
        public override string ToString()
        {
            string errorLog = $"ERROR LOG...\n\nHas errors occurred:{HasErrors}\nNumber of errors:{NumberOfErrors}\n\n";
            foreach (Error e in Errors)
            {
                errorLog += $"{e.ToString()}\n\n";
            }
            return errorLog;
        }
    }
}