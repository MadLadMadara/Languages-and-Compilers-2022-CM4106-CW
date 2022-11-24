using System;

namespace Compiler
{
    /// <summary>
    /// A position in a file
    /// </summary>
    public class Position
    {   
        /// <summary>
        /// A constant to use as the position of system defined items
        /// </summary>
        public static Position BuiltIn { get; } = new Position(-1, -1);
        /// <summary>
        /// line which the token is on in source code
        /// </summary>
        public int LineNumber { get; private set; }
        /// <summary>
        /// position in line that the token is found in source code
        /// </summary>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Initialises class with line position and line number for token
        /// </summary>
        /// <param name="lineNumber"> line which the token is on in source code</param>
        /// <param name="linePosition">position in line that the token is found in source code</param>
        public Position(int lineNumber, int linePosition)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }
        /// <inheritDoc />
        public override string ToString()
        {
            return (this == BuiltIn) ? "System defined": $"LineNumber={LineNumber}, LinePosition=\"{LinePosition}\", ";
        }

 
    }
}