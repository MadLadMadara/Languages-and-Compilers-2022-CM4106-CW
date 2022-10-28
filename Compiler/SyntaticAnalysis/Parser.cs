﻿using Compiler.IO;
using Compiler.Tokenization;
using System.Collections.Generic;
using static Compiler.Tokenization.TokenType;

namespace Compiler.SyntacticAnalysis
{
    /// <summary>
    /// A recursive descent parser
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// The error reporter
        /// </summary>
        public ErrorReporter Reporter { get; }

        /// <summary>
        /// The tokens to be parsed
        /// </summary>
        private List<Token> tokens;

        /// <summary>
        /// The index of the current token in tokens
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current token
        /// </summary>
        private Token CurrentToken { get { return tokens[currentIndex]; } }

        /// <summary>
        /// Advances the current token to the next one to be parsed
        /// </summary>
        private void MoveNext()
        {
            if (currentIndex < tokens.Count - 1)
                currentIndex += 1;
        }

        /// <summary>
        /// Creates a new parser
        /// </summary>
        /// <param name="reporter">The error reporter to use</param>
        public Parser(ErrorReporter reporter)
        {
            Reporter = reporter;
        }

        /// <summary>
        /// Checks the current token is the expected kind and moves to the next token
        /// </summary>
        /// <param name="expectedType">The expected token type</param>
        private void Accept(TokenType expectedType)
        {
            if (CurrentToken.Type == expectedType)
            {
                Debugger.Write($"Accepted {CurrentToken}");
                MoveNext();
            }
        }

        /// <summary>
        /// Parses a program
        /// </summary>
        /// <param name="tokens">The tokens to parse</param>
        public void Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            ParseProgram();
        }



        /// <summary>
        /// Parses a program
        /// </summary>
        private void ParseProgram()
        {
            Debugger.Write("Parsing program");
            ParseSingleCommand();
        }



        /// <summary>
        /// Parses a command
        /// </summary>
        private void ParseCommand()
        {
            Debugger.Write("Parsing command");
            ParseSingleCommand();
            while (CurrentToken.Type == Semicolon)
            {
                Accept(Semicolon);
                ParseSingleCommand();
            }
        }

        /// <summary>
        /// Parses a single command
        /// </summary>
        private void ParseSingleCommand()
        {
            Debugger.Write("Parsing Single Command");
            switch (CurrentToken.Type)
            {
                // There are missing cases here - you'll need to fill them all in
                case Identifier:
                    ParseAssignmentOrCallCommand();
                    break;
                case Begin:
                    ParseBeginCommand();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Parses an assignment or call command
        /// </summary>
        private void ParseAssignmentOrCallCommand()
        {
            Debugger.Write("Parsing Assignment Command or Call Command");
            // This is a lie - this is only dealing with the assignment. Change it to deal with function calls too.

            Debugger.Write("Parsing Assignment Command");
            ParseIdentifier();
            Accept(Becomes); 
            ParseExpression();
        }

        /// <summary>
        /// Parses a begin command
        /// </summary>
        private void ParseBeginCommand()
        {
            Debugger.Write("Parsing Begin Command");
            Accept(Begin);
            ParseCommand();
            Accept(End);
        }
        /// <summary>
        /// Parse Var Declaration
        /// </summary>
        private void ParseVarDeclaration()
        {
            Debugger.Write("Parsing Var Declaration");
            Accept(Var);
            ParseIdentifier();
            Accept(Is);
            ParseTypeDenoter();
        }

        /// <summary>
        /// Parses an expression
        /// </summary>
        private void ParseExpression()
        {
            Debugger.Write("Parsing Expression");
            ParsePrimaryExpression();
            while (CurrentToken.Type == TokenType.Operator)
            {
                ParseOperator();
                ParsePrimaryExpression();
            }

        }
        /// <summary>
        /// Parses an expression
        /// </summary>
        private void ParsePrimaryExpression()
        {
            Debugger.Write("Parsing Primary Expression");
            switch (CurrentToken.Type)
            {
                case IntLiteral:
                    ParseIntExpression();
                    break;
                case CharLiteral:
                    ParseCharExpression();
                    break;
                case Identifier:
                    ParseIdExpression();
                    break;
                case Operator:
                    ParseUnaryExpression();
                    break;
                case LeftBracket:
                    ParseBracketExpression();
                    break;
            }
        }
        /// <summary>
        /// Parses a unary expresion
        /// </summary>
        private void ParseUnaryExpression()
        {
            Debugger.Write("Parsing Unary Expression");
            ParseOperator();
            ParsePrimaryExpression();
        }

        private void ParseBracketExpression()
        {
            Debugger.Write("Parsing Bracket Expression");
            Accept(LeftBracket);
            ParseExpression();
            Accept(RightBracket);
        }
        private void ParseIdExpression() // TODO: Change from lab
        {
            Debugger.Write("Parsing Call Expression or Identifier Expression");
            ParseIdentifier();
            if(CurrentToken.Type == LeftBracket)
            {
                Accept(LeftBracket);
                ParseParameter();
                Accept(RightBracket);
            }
        }

        /// <summary>
        /// Parsing Character Expression
        /// </summary>
        private void ParseCharExpression()
        {
            Debugger.Write("Parsing Character Expression");
            ParseIntegerLiteral();
        }
        /// <summary>
        /// Parse Integer Expression 
        /// </summary>
        private void ParseIntExpression()
        {
            Debugger.Write("Parsing Integer Expression");
            ParseIntegerLiteral();
        }
        /// <summary>
        /// Parse TypeDenoter
        /// </summary>
        private void ParseTypeDenoter()
        {
            Debugger.Write("Parsing Type Denoter");
            ParseIdentifier();
        }

        /// <summary>
        /// Parses an identifier
        /// </summary>
        private void ParseIdentifier()
        {
            Debugger.Write("Parsing Identifier");
            Accept(Identifier);
        }

        /// <summary>
        /// Parses Integer Literal 
        /// </summary>
        private void ParseIntegerLiteral()
        {
            Debugger.Write("Parsing Integer Literal  ");
            Accept(IntLiteral);
        }
        /// <summary>
        /// Parses Character Literal 
        /// </summary>
        private void ParseCharacterLiteral()
        {
            Debugger.Write("Parsing Character Literal");
            Accept(IntLiteral);
        }
        /// <summary>
        /// Parses Operator 
        /// </summary>
        private void ParseOperator()
        {
            Debugger.Write("Parsing Operator");
            Accept(Operator);
        }
    }
}