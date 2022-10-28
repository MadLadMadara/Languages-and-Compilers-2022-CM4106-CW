using Compiler.IO;
using Compiler.Tokenization;
using System.Collections.Generic;
using System.Reflection.Metadata;
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
            else
            {
                Debugger.Write($"Failed to accepted: {CurrentToken}, Expected: {expectedType}");
                Reporter.NewError(CurrentToken, $"Expected '{expectedType}', found: '{CurrentToken.Type}'");
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
        private void ParseAssignmentOrCallCommand() // TODO: Change made from lab with the error handling
        {
            Debugger.Write("Parsing Assignment Command or Call Command");
            ParseIdentifier();
            if(CurrentToken.Type == Is)
            {
                Debugger.Write("Parsing Assignment Command");
                Accept(Is);
                ParseExpression();
            }else if (CurrentToken.Type == LeftBracket)
            {
                Debugger.Write("Parsing Call Command");
                Accept(LeftBracket);
                ParseParameter();
                Accept(RightBracket);
            }
            else
            {
                Debugger.Write($"Failed to accepted: {CurrentToken}, Expected: {Is} or {LeftBracket}");
                Reporter.NewError(CurrentToken, $"Expected {Is} or {LeftBracket}, found: {CurrentToken.Spelling}");
            }
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
        /// Parse Let command
        /// </summary>
        private void ParseLetCommand()
        {
            Accept(Let);
            ParseDeclaration();
            Accept(In);
            ParseSingleCommand();
        }

        /// <summary>
        /// Parses a declaration
        /// </summary>
        private void ParseDeclaration()
        {
            Debugger.Write("Parsing Declaration");
            ParseSingleDeclaration();
            while (CurrentToken.Type == Semicolon)
            {
                Accept(Semicolon);
                ParseSingleDeclaration();
            }
        }

        /// <summary>
        /// Parses a single declaration
        /// </summary>
        private void ParseSingleDeclaration()
        {
            Debugger.Write("Parsing Single Declaration");
            switch (CurrentToken.Type)
            {
                case Const:
                    ParseConstDeclaration();
                    break;
                case Var:
                    ParseVarDeclaration();
                    break;
                default:
                    // TODO: error handeling here
                    break;
            }
        }

        /// <summary>
        /// Parse Const Deceleration 
        /// </summary>
        private void ParseConstDeclaration()
        {
            Debugger.Write("Parsing Const Declaration");
            Accept(Const);
            ParseIdentifier();
            Accept(Is);
            ParseExpression();
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
        /// Parses a unary expression
        /// </summary>
        private void ParseUnaryExpression()
        {
            Debugger.Write("Parsing Unary Expression");
            ParseOperator();
            ParsePrimaryExpression();
        }

        /// <summary>
        /// Parses a Bracket Expression
        /// </summary>
        private void ParseBracketExpression()
        {
            Debugger.Write("Parsing Bracket Expression");
            Accept(LeftBracket);
            ParseExpression();
            Accept(RightBracket);
        }

        /// <summary>
        /// Parse Identifier Expression
        /// </summary>
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
        /// Parses a parameter
        /// </summary>
        private void ParseParameter()
        {
            Debugger.Write("Parsing Parameter");
            switch (CurrentToken.Type)
            {
                case RightBracket:
                    // Empty parameter list
                    break;
                case Var:
                    ParseVarParameter();
                    break;
                default:
                    ParseValueParameter();
                    break;
            }
        }

        /// <summary>
        /// Parses a value parameter
        /// </summary>
        private void ParseValueParameter()
        {
            Debugger.Write("Parsing Value Parameter");
            ParseExpression();
        }

        /// <summary>
        /// Parses a variable parameter
        /// </summary>
        private void ParseVarParameter()
        {
            Debugger.Write("Parsing Variable Parameter");
            Accept(Var);
            ParseIdentifier();
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
