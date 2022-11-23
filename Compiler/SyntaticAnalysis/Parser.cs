using Compiler.IO;
using Compiler.Tokenization;
using Compiler.Nodes;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static Compiler.Tokenization.TokenType;
using System.Reflection.Metadata.Ecma335;
using System.Linq.Expressions;

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
        /// <returns>A ProgramNode of the program. This is the full Abstract Syntax Tree</returns>
        public ProgramNode Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            return ParseProgram();
        }

        /// <summary>
        /// Parses a program
        /// </summary>
        /// <returns>AST of a Program</returns>
        private ProgramNode ParseProgram()
        {
            Debugger.Write("Parsing program");
            ICommandNode command = ParseCommand();
            return new ProgramNode(command);
        }

        /// <summary>
        /// Parses a command
        /// </summary>
        /// <returns>AST of a command</returns>
        private ICommandNode ParseCommand()
        {
            Debugger.Write("Parsing command");
            List<ICommandNode> commands = new List<ICommandNode>();
            commands.Add(ParseSingleCommand());
            while (CurrentToken.Type == Semicolon)
            {
                Accept(Semicolon);
                commands.Add(ParseSingleCommand());
            }
            return commands.Count == 1 ? commands[0] : new SequentialCommandNode(commands);
        }

        /// <summary>
        /// Parses a single command
        /// </summary>
        /// <returns>AST of a single command</returns>
        private ICommandNode ParseSingleCommand()
        {
            Debugger.Write("Parsing Single Command");
            switch (CurrentToken.Type)
            {
                case Identifier:
                    return ParseAssignmentOrCallCommand();
                case Begin:
                    return ParseBeginCommand();
                case Let:
                    return ParseLetCommand();
                case If:
                    return ParseIfCommand();
                case QuestionMark:
                    return ParseQuickIfCommand();
                case While:
                    return ParseWhileCommand();
                case Loop:
                    return ParseLoopCommand();
                default:
                    Debugger.Write("Parsing Skip Command");
                    return new BlankCommandNode(CurrentToken.tokenStartPosition);
            }
        }

        /// <summary>
        /// Parses an assignment or call command
        /// </summary>
        /// <returns>AST of a call or assignment command</returns>
        private ICommandNode ParseAssignmentOrCallCommand() 
        {
            Debugger.Write("Parsing Assignment Command or Call Command");
            Position position = CurrentToken.tokenStartPosition;
            IdentifierNode identifier =  ParseIdentifier();
            if(CurrentToken.Type == Is)
            {
                Debugger.Write("Parsing Assignment Command");
                Accept(Is);
                return new AssignCommandNode(identifier, ParseExpression()); 
            }else if (CurrentToken.Type == LeftBracket)
            {
                Debugger.Write("Parsing Call Command");
                Accept(LeftBracket);
                IParameterNode parameter = ParseParameter();
                Accept(RightBracket);
                return new CallCommandNode(identifier, parameter); 
            }
            else
            {
                Debugger.Write($"Failed to accepted: {CurrentToken}, Expected: {Is} or {LeftBracket}");
                Reporter.NewError(CurrentToken, $"Expected {Is} or {LeftBracket}, found: {CurrentToken.Spelling}");
                return new ErrorNode(position);
            }
        }

        /// <summary>
        /// Parses a begin command
        /// </summary>
        /// <returns>AST of a command node</returns>
        private ICommandNode ParseBeginCommand()
        {
            Debugger.Write("Parsing Begin Command");
            Accept(Begin);
            ICommandNode command = ParseCommand();
            Accept(End);
            return command;
        }

        /// <summary>
        /// Parse let command
        /// </summary>
        /// <returns>AST of a let command</returns>
        private LetCommandNode ParseLetCommand()
        {
            Debugger.Write("Parsing Let Command");
            Position position = CurrentToken.tokenStartPosition;
            Accept(Let);
            IDeclarationNode declaration = ParseDeclaration();
            Accept(In);
            ICommandNode command = ParseSingleCommand();
            return new LetCommandNode(declaration, command, position); 
        }

        /// <summary>
        ///  Parse If Command
        /// </summary>
        private void ParseIfCommand()
        {
            Debugger.Write("Parsing If Command");
            Accept(If);
            ParseExpression();
            Accept(Then);
            ParseSingleCommand();
            Accept(Else);
            ParseSingleCommand();
        }

        /// <summary>
        /// Parse Quick if Command
        /// </summary>
        private void ParseQuickIfCommand()
        {
            Debugger.Write("Parsing Quick If Command");
            Accept(QuestionMark);
            ParseExpression();
            Accept(ThenDo);
            ParseSingleCommand();
        }

        /// <summary>
        /// Parse While Command
        /// </summary>
        private void ParseWhileCommand()
        {
            Debugger.Write("Parsing While Command");
            Accept(While); 
            ParseBracketExpression();
            ParseSingleCommand();
            Accept(Wend);
        }

        /// <summary>
        /// Parse Loop Command
        /// </summary>
        private void ParseLoopCommand()
        {
            Debugger.Write("Parsing Loop Command");
            Accept(Loop);
            ParseSingleCommand();
            Accept(While);
            Accept(LeftBracket);
            ParseExpression();
            Accept(RightBracket);
            ParseSingleCommand();
            Accept(Repeat);
        }

        /// <summary>
        /// Parses a declaration
        /// </summary>
        /// <returns>AST of a single deceleration or sequential</returns>
        private IDeclarationNode ParseDeclaration()
        {
            Debugger.Write("Parsing Declaration");
            List<IDeclarationNode> declarations = new List<IDeclarationNode>();
            declarations.Add(ParseSingleDeclaration());
            while (CurrentToken.Type == Semicolon)
            {
                Accept(Semicolon);
                declarations.Add(ParseSingleDeclaration());
            }
            return declarations.Count == 1 ? declarations[0] : new SequentialDeclarationNode(declarations);
        }

        /// <summary>
        /// Parses a single declaration
        /// </summary>
        /// <returns>AST of a Const or Var deceleration</returns>
        private IDeclarationNode ParseSingleDeclaration()
        {
            Debugger.Write("Parsing Single Declaration");
            switch (CurrentToken.Type)
            {
                case Const:
                    Debugger.Write("Parsing Const");
                    return ParseConstDeclaration()
                case Var:
                    Debugger.Write("Parsing Var");
                    return ParseVarDeclaration();
                default:
                    Debugger.Write($"Failed to accepted: {CurrentToken}, Expected: 'Const' or 'Var'");
                    Reporter.NewError(CurrentToken, $"Expected 'Const' or 'Var', found: '{CurrentToken.Type}'");
                    return new ErrorNode(CurrentToken.Position);
            }
        }

        /// <summary>
        /// Parse Const Deceleration 
        /// </summary>
        private IDeclarationNode ParseConstDeclaration()
        {
            Debugger.Write("Parsing Const Declaration");
            Position position = CurrentToken.tokenStartPosition;
            Accept(Const);
            IdentifierNode identifier = ParseIdentifier();
            Accept(Is);
            IExpressionNode expression = ParseExpression();
            return new ConstDeclarationNode(identifier, expression, position);
        }

        /// <summary>
        /// Parse Var Declaration
        /// </summary>
        private IDeclarationNode ParseVarDeclaration()
        {
            Debugger.Write("Parsing Var Declaration");
            Position position = CurrentToken.tokenStartPosition;
            Accept(Var);
            IdentifierNode identifier = ParseIdentifier();
            Accept(Is);
            TypeDenoterNode type = ParseTypeDenoter();
            return new VarDeclarationNode(identifier, type, position);
        }

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns>AST of an expression</returns>
        private IExpressionNode ParseExpression()
        {
            Debugger.Write("Parsing Expression");
            IExpressionNode left  = ParsePrimaryExpression();
            while (CurrentToken.Type == TokenType.Operator)
            {
                OperatorNode op =  ParseOperator();
                IExpressionNode right = ParsePrimaryExpression();
                left = new BinaryExpressionNode(left, op, right);
            }
            return left; 
        }

        /// <summary>
        /// Parses a primary expression
        /// </summary>
        /// <returns>AST of a Primary expression</returns>
        private IExpressionNode ParsePrimaryExpression()
        {
            Debugger.Write("Parsing Primary Expression");
            switch (CurrentToken.Type)
            {
                case IntLiteral:
                    return ParseIntExpression();
                case CharLiteral:
                    return ParseCharExpression();
                case Identifier:
                    return ParseIdExpression(); 
                case Operator:
                    return ParseUnaryExpression();
                case LeftBracket:
                    return ParseBracketExpression();
                default:
                    Debugger.Write($"Failed to accepted: {CurrentToken}, Expected: 'IntLiteral', 'CharLiteral', 'Identifier', 'Operator' or 'LeftBracket'");
                    Reporter.NewError(CurrentToken, $"Expected IntLiteral', 'CharLiteral', 'Identifier', 'Operator' or 'LeftBracket'. found: '{CurrentToken.Type}'");
                    return new ErrorNode(CurrentToken.tokenStartPosition);
            }
        }

        /// <summary>
        /// Parses a unary expression
        /// </summary>
        /// <returns>AST of a Unary expression</returns>
        private UnaryExpressionNode ParseUnaryExpression()
        {
            Debugger.Write("Parsing Unary Expression");
            return new UnaryExpressionNode(ParseOperator(), ParsePrimaryExpression()); 
        }

        /// <summary>
        /// Parses a Bracket Expression
        /// </summary>
        /// <returns>AST of an expression</returns>
        private IExpressionNode ParseBracketExpression()
        {
            Debugger.Write("Parsing Bracket Expression");
            Accept(LeftBracket);
            IExpressionNode expression =  ParseExpression();
            Accept(RightBracket);
            return expression; 
        }

        /// <summary>
        /// Parse Identifier Expression
        /// </summary>
        /// <returns>AST of an Identifier expression or call expression</returns>
        private IExpressionNode ParseIdExpression() 
        {
            Debugger.Write("Parsing Call Expression or Identifier Expression");
            IdentifierNode Identifier =  ParseIdentifier();
            IExpressionNode IdExprssion = new IdExpressionNode(Identifier);
            if (CurrentToken.Type == LeftBracket)
            {
                Accept(LeftBracket);
                IdExprssion = new CallExpressionNode(Identifier, ParseParameter());
                Accept(RightBracket);
            }
            return IdExprssion;
        }

        /// <summary>
        /// Parsing Character Expression
        /// </summary>
        private CharacterExpressionNode ParseCharExpression()
        {
            Debugger.Write("Parsing Character Expression");
            return new CharacterExpressionNode(ParseCharacterLiteral());
        }

        /// <summary>
        /// Parse Integer Expression
        /// </summary>
        /// <returns>AST of an integer expression</returns>
        private IntegerExpressionNode ParseIntExpression()
        {
            Debugger.Write("Parsing Integer Expression");
            return new IntegerExpressionNode(ParseIntegerLiteral());
        }

        /// <summary>
        /// Parses a parameter
        /// </summary>
        /// <returns>AST of a parameter</returns>
        private IParameterNode ParseParameter()
        {
            Debugger.Write("Parsing Parameter");
            switch (CurrentToken.Type)
            {
                case Identifier:
                case IntLiteral:
                case CharLiteral:
                case Operator:
                case LeftBracket:
                    return ParseValueParameter();
                case Var:
                    return ParseVarParameter();
                case RightBracket:
                    Debugger.Write("Parsing Blank Parameter");
                    return new BlankParameterNode(CurrentToken.tokenStartPosition);
                default:
                    Debugger.Write($"Failed to accepted: {CurrentToken}, Expected: 'IntLiteral', 'CharLiteral', 'Identifier', 'Operator', 'LeftBracket', RightBracket or'Var'");
                    Reporter.NewError(CurrentToken, $"Expected IntLiteral', 'CharLiteral', 'Identifier', 'Operator' or 'LeftBracket'. found: '{CurrentToken.Type}'");
                    return new ErrorNode(CurrentToken.tokenStartPosition);
            }
        }

        /// <summary>
        ///  Parses a value parameter
        /// </summary>
        /// <returns>AST of a parameters value</returns>
        private ExpressionParameterNode ParseValueParameter()
        {
            Debugger.Write("Parsing Value Parameter");
            return new ExpressionParameterNode(ParseExpression());
        }

        /// <summary>
        /// Parses a variable parameter
        /// </summary>
        /// <returns>AST of a parameter identifier </returns>
        private VarParameterNode ParseVarParameter()
        {
            Debugger.Write("Parsing Variable Parameter");
            Position startPosition = CurrentToken.tokenStartPosition;
            Accept(Var);
            return new VarParameterNode(ParseIdentifier(), startPosition);
        }


        /// <summary>
        /// Parse TypeDenoter
        /// </summary>
        /// <returns>AST of an TypeDenoterNode</returns>
        private TypeDenoterNode ParseTypeDenoter()
        {
            Debugger.Write("Parsing Type Denoter");
            return new TypeDenoterNode(ParseIdentifier());
        }

        /// <summary>
        /// Parses an identifier
        /// </summary>
        /// <returns>AST of an identifier</returns>
        private IdentifierNode ParseIdentifier()
        {
            Debugger.Write("Parsing Identifier");
            Token T= CurrentToken;
            Accept(Identifier);
            return new IdentifierNode(T);
        }

        /// <summary>
        /// Parses Integer Literal 
        /// </summary>
        /// <returns>AST of the IntegerLiteralNode</returns>
        private IntegerLiteralNode ParseIntegerLiteral()
        {
            Debugger.Write("Parsing Integer Literal");
            Token T = CurrentToken;
            Accept(IntLiteral);
            return new IntegerLiteralNode(T);
        }

        /// <summary>
        /// Parses Character Literal 
        /// </summary>
        /// <returns>AST of the CharacterLiteralNode</returns>
        private CharacterLiteralNode ParseCharacterLiteral()
        {
            Debugger.Write("Parsing Character Literal");
            Token T = CurrentToken;
            Accept(CharLiteral);
            return new CharacterLiteralNode(T);
        }

        /// <summary>
        /// Parses Operator 
        /// </summary>
        /// <returns>AST of the OperatorNode</returns>
        private OperatorNode ParseOperator()
        {
            Debugger.Write("Parsing Operator");
            Token T = CurrentToken;
            Accept(Operator);
            return new OperatorNode(T);
        }
    }
}
