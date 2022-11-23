using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Nodes
{
    /// <summary>
    /// A node corresponding to an quick if command
    /// </summary>
    public class QuickIfCommandNode: ICommandNode
    {
        /// <summary>
        /// The condition expression
        /// </summary>
        public IExpressionNode Expression { get; }

        /// <summary>
        /// The then branch command 
        /// </summary>
        public ICommandNode ThenCommand { get; }

        /// <summary>
        /// The position in the code where the content associated with the node begins
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// Creates a new quick if command node
        /// </summary>
        /// <param name="expression">The condition expression</param>
        /// <param name="thenCommand">The then branch command</param>
        /// <param name="position">The position in the code where the content associated with the node begins</param>
        public QuickIfCommandNode(IExpressionNode expression, ICommandNode thenCommand, Position position)
        {
            Expression = expression;
            ThenCommand = thenCommand;
            Position = position;
        }
    }
}
