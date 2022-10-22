using Compiler.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Compiler.Tokenization
{
    /// <summary>
    /// A tokenizer for the reader language
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// The error reporter
        /// </summary>
        public ErrorReporter Reporter { get; }

        /// <summary>
        /// The reader getting the characters from the file
        /// </summary>
        private IFileReader Reader { get; }

        /// <summary>
        /// The characters currently in the token
        /// </summary>
        private StringBuilder TokenSpelling { get; } = new StringBuilder();

        /// <summary>
        /// Create a new tokenizer
        /// </summary>
        /// <param name="reader">The reader to get characters from the file</param>
        /// <param name="reporter">The error reporter to use</param>
        public Tokenizer(IFileReader reader, ErrorReporter reporter)
        {
            Reader = reader;
            Reporter = reporter;
        }

        /// <summary>
        /// Gets all the tokens from the file
        /// </summary>
        /// <returns>A list of all the tokens in the file in the order they appear</returns>
        public List<Token> GetAllTokens()
        {
            List<Token> tokens = new List<Token>();
            Token token = GetNextToken();
            while (token.Type != TokenType.EndOfText)
            {
                tokens.Add(token);
                token = GetNextToken();
            }
            tokens.Add(token);
            Reader.Close();
            return tokens;
        }

        /// <summary>
        /// Scan the next token
        /// </summary>
        /// <returns>True if and only if there is another token in the file</returns>
        private Token GetNextToken()
        {
            // Skip forward over any white space and comments
            SkipSeparators();

            // Clear spelling 
            TokenSpelling.Clear();

            // Scan the next token and store it
            Token  token = ScanToken();

            Debugger.Write($"Scanned {token}");

            return token;
        }

        /// <summary>
        /// Skip forward until the next character is not whitespace or a comment
        /// </summary>
        private void SkipSeparators()
        {
            if (Reader.Current == '&')
            {
                Reader.SkipRestOfLine();
            }
            else
            {
                while (IsWhiteSpace(Reader.Current))
                {
                    Reader.MoveNext();
                }
            }
        }

        /// <summary>
        /// Find the next token
        /// </summary>
        /// <returns>The type of the next token</returns>
        /// <remarks>Sets tokenSpelling to be the characters in the token</remarks>
        private Token ScanToken()
        {
            // Remember the starting position of the token
            Position tokenStartPosition = Reader.CurrentPosition;

            if (Char.IsLetterOrDigit(Reader.Current))
            {
                // consume as IntLiteral as default
                TokenType T = TokenType.IntLiteral;
                do
                {
                    if (Char.IsLetter(Reader.Current)) T = TokenType.Identifier; // consume as identifier if a letter is found
                    TakeIt();
                } 
                while (Char.IsLetterOrDigit(Reader.Current));
                // check if keyword
                if (TokenTypes.IsKeyword(TokenSpelling))
                {
                    // consume as Keyword
                    T = TokenTypes.GetTokenForKeyword(TokenSpelling);
                }
                return new Token(T, TokenSpelling.ToString(), tokenStartPosition);
            } else if (Reader.Current == '{') 
            {
                // consume as char chars literal
                TokenType T = TokenType.CharLiteral;
                string errorMessage = ""; // construct error message in case there is an issue
                TakeIt();
                if (IsGraphic(Reader.Current))
                {
                    TakeIt();

                    if (Reader.Current == '}')
                    {
                        TakeIt();
                    }
                    else
                    {
                        T = TokenType.Error;
                        errorMessage = $"Syntax, Character literal expected closing tag '}}' but '{Reader.Current}' was found instead. Characters literal may only contain one character.";
                    }
                }
                else
                {
                    TakeIt();
                    T = TokenType.Error;
                    errorMessage = $"Syntax, Character literal expected a diet (0-9), Question mark '?', single black space character or a letter (a-z) in upper or lower case. This was found instead '{Reader.Current}'";
                }
                // create token 
                Token token = new Token(T, TokenSpelling.ToString(), tokenStartPosition);
                if (T == TokenType.Error) Reporter.NewError(token, errorMessage); // Report error if one has occurred
                return token;

            } else if (IsPunctuation(Reader.Current))
            {
                // Consume as punctuation
                TokenType T = TokenType.Error; // set default
                string errorMessage = "";      // set default error message
                TakeIt();
                switch (Reader.Current)
                {
                    case '(':
                        T = TokenType.LeftBracket;
                        break;
                    case ')':
                        T = TokenType.RightBracket;
                        break;
                    case '~':
                        T = TokenType.Is;
                        break;
                    case ';':
                        T = TokenType.Semicolon;
                        break;
                    case ':':
                        T = TokenType.Colon;
                        break;
                    case '?':
                        T = TokenType.QuestionMark;
                        break;
                    default:
                        // Code will never be reached but is a good fail safe in-case of future changes
                        errorMessage = $"Syntax, punctuation, Expected either '('. ')', '~', ';', ':', '?' but instead {Reader.Current} was founds."; 
                        break;
                }
                Token token = new Token(T, TokenSpelling.ToString(), tokenStartPosition);
                if (T == TokenType.Error) Reporter.NewError(token, errorMessage); // Report error if one has occurred, again this is unreachable but a failsafe in-case of future changes 
                return token;
            }
            else if (IsOperator(Reader.Current))
            {
                TokenType T = TokenType.Operator; // set token type to operator
                if (Reader.Current == '=') // check if operator is a special case
                {
                    TakeIt(); 
                    if(Reader.Current == '>') // if not then consume as Operator
                    {
                        // Consume as punctuation special case ThenDo '=>' for the quick if command 
                        TakeIt();
                        T = TokenType.ThenDo;  
                    }
                }
                else
                {
                    // Consume as operator 
                    TakeIt();

                }
                return new Token(T, TokenSpelling.ToString(), tokenStartPosition);
            } 
            else if (Reader.Current == default(char))
            {
                // Read the end of the file
                TakeIt();
                return new Token(TokenType.EndOfText, TokenSpelling.ToString(), tokenStartPosition);
            }
            else
            {
                // Encountered a character we weren't expecting
                TakeIt();
                Token token = new Token(TokenType.Error, TokenSpelling.ToString(), tokenStartPosition);
                Reporter.NewError(token, $"Syntax, unexpected and unknown character found '{Reader.Current}'."); 
                return token;
            }
        }

        /// <summary>
        /// Appends the current character to the current token then moves to the next character
        /// </summary>
        private void TakeIt()
        {
            TokenSpelling.Append(Reader.Current);
            Reader.MoveNext();
        }

        /// <summary>
        /// Checks whether a character is white space
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>True if and only if c is a whitespace character</returns>
        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        /// <summary>
        /// Checks whether a character is an operator
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>True if and only if the character is an operator in the language</returns>
        private static bool IsOperator(char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '<':
                case '>':
                case '=':
                case '!':
                    return true;
                default:
                    return false;
            }
        }
        // ADDED: IsPunctuation
        /// <summary>
        ///  checks whether a given character is punctuation
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>True if char is of token type punctuation</returns>
        private static bool IsPunctuation(char c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '~':
                case ':':
                case ';':
                case '?':
                    return true;
                default:
                    return false;
            }
        }
        // ADDED: IsGraphic
        /// <summary>
        /// Checks if character is a graphic 
        /// </summary>
        /// <param name="c">The given character to be checked</param>
        /// <returns>True if given char is a letter, digit, white space or a '?' otherwise return false</returns>
        private static bool IsGraphic(char c)
        {
            if (Char.IsLetterOrDigit(c) || IsWhiteSpace(c) || c == '?')
                return true;
            else
                return false; 
        }
    }
}
