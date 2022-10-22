﻿using Compiler.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                TokenSpelling.Clear();
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

            // Remember the starting position of the token
            Position tokenStartPosition = Reader.CurrentPosition;

            // Scan the token and work out its type
            TokenType tokenType = ScanToken();

            // Create the token
            Token token = new Token(tokenType, TokenSpelling.ToString(), tokenStartPosition);
            Debugger.Write($"Scanned {token}");

            // Report an error if necessary
            if (tokenType == TokenType.Error)
            {
                Reporter.NewError(token);
            }
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
        private TokenType ScanToken()
        {
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
                return T;
            } else if (Reader.Current == '{') 
            {
                // consume as char chars literal
                TakeIt();
                if (!IsGraphic(Reader.Current)) return TokenType.Error;
                TakeIt();
                if(Reader.Current != '}') return TokenType.Error;
                TakeIt();
                return TokenType.CharLiteral;

            } else if (IsPunctuation(Reader.Current))
            {
                // Consume as punctuation
                TokenType T = TokenType.Error;
                switch (Reader.Current)
                {
                    case '(':
                        TakeIt();
                        T = TokenType.LeftBracket;
                        break;
                    case ')':
                        TakeIt();
                        T = TokenType.RightBracket;
                        break;
                    case '~':
                        TakeIt();
                        T = TokenType.Is;
                        break;
                    case ';':
                        TakeIt();
                        T = TokenType.Semicolon;
                        break;
                    case ':':
                        TakeIt();
                        T = TokenType.Colon;
                        break;
                    case '?':
                        TakeIt();
                        T = TokenType.QuestionMark;
                        break;
                    default:
                        TakeIt();
                        break;
                }
                return T;
            }else if (IsOperator(Reader.Current))
            {
                TokenType T = TokenType.Operator; // set token type to operator
                if (Reader.Current == '=') // check if operator is a special case
                {
                    TakeIt(); 
                    if(Reader.Current == '>') // if not then consume as Operator
                    {
                        // Consume as punctuation special case ThenDo 
                        TakeIt();
                        T = TokenType.ThenDo;  
                    }
                }
                else
                {
                    // Consume as operator 
                    TakeIt();

                }
                return T;
            } 
            else if (Reader.Current == default(char))
            {
                // Read the end of the file
                TakeIt();
                return TokenType.EndOfText;
            }
            else
            {
                // Encountered a character we weren't expecting
                TakeIt();
                return TokenType.Error;
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
