namespace Compiler.Tokenization
{
    /// <summary>
    /// A token in the source language
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The type of the token
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The actual text associated with the token
        /// </summary>
        public string Spelling { get; }
        /// <summary>
        /// The starting position of the token in the source code
        /// </summary>
        public Position tokenStartPosition { get; }

        /// <summary>
        /// Creates a token in the source language
        /// </summary>
        /// <param name="type">The type of the token</param>
        /// <param name="spelling">The spelling of the token</param>
        /// /// <param name="position">The starting position of the token</param>
        public Token(TokenType type, string spelling, Position position)
        {
            Spelling = spelling;
            Type = type;
            tokenStartPosition = position; 
        }

        /// <inheritDoc />
        public override string ToString()
        {
            return $"type={Type}, spelling=\"{Spelling}\", {tokenStartPosition.ToString()}";
        }
    }
}
