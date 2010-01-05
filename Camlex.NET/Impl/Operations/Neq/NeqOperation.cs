﻿using System.Xml.Linq;
using Camlex.NET.Impl.Factories;
using Camlex.NET.Interfaces;

namespace Camlex.NET.Impl.Operations.Neq
{
    public class NeqOperation : BinaryOperationBase
    {
        public NeqOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            var result = new XElement(Tags.Neq,
                             this.fieldRefOperand.ToCaml(),
                             this.valueOperand.ToCaml());
            return this.operationResultBuilder.Add(result).ToResult();
        }
    }
}


