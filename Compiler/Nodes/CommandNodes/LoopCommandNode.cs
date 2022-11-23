using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Nodes
{
    /// <summary>
    /// A node corresponding to a loop command
    /// </summary>
    public class LoopCommandNode: ICommandNode
    {
        /// <summary>
        /// The command before the loop
        /// </summary>
        public ICommandNode Command { get; }

        /// <summary>
        /// The condition associated with the loop
        /// </summary>
        public IExpressionNode Expression { get; }

        /// <summary>
        /// The command inside the loop
        /// </summary>
        public ICommandNode LoopCommand { get; }

        /// <summary>
        /// The position in the code where the content associated with the node begins
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// Creates a new loop node
        /// </summary>
        /// <param name="command">The command before the loop</param>
        /// <param name="expression">The condition associated with the loop</param>
        /// <param name="loopCommand">The command inside the loop</param>
        /// <param name="position">The position in the code where the content associated with the node begins</param>
        public LoopCommandNode(ICommandNode command, IExpressionNode expression, ICommandNode loopCommand, Position position)
        {
            Command = command;
            Expression = expression;
            LoopCommand = loopCommand;
            Position = position;
        }
    }
}
