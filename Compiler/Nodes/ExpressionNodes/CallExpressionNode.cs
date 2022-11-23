using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Nodes
{
    public class CallExpressionNode : IExpressionNode
    {
        /// <summary>
        /// The function name
        /// </summary>
        public IdentifierNode Identifier { get; }

        /// <summary>
        /// The function call parameter
        /// </summary>
        public IParameterNode Parameter { get; }

        /// <summary>
        /// The type of the node
        /// </summary>
        public SimpleTypeDeclarationNode Type { get; set; }

        /// <summary>
        /// The position in the code where the content associated with the node begins
        /// </summary>
        public Position Position { get { return Identifier.Position; } }

        /// <summary>
        /// Creates a new call command node
        /// </summary>
        /// <param name="identifier">The function name</param>
        /// <param name="parameter">The function call parameter</param>
        public CallExpressionNode(IdentifierNode identifier, IParameterNode parameter)
        {
            Identifier = identifier;
            Parameter = parameter;
        }
    }
}
