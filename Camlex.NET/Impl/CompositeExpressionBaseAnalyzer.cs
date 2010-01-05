﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Camlex.NET.Interfaces;
using Microsoft.SharePoint;

namespace Camlex.NET.Impl
{
    // Base class for AndAlso and OrElse analyzers
    public abstract class CompositeExpressionBaseAnalyzer : BaseAnalyzer
    {
        protected IAnalyzerFactory analyzerFactory;

        protected CompositeExpressionBaseAnalyzer(IOperationResultBuilder operationResultBuilder,
            IAnalyzerFactory analyzerFactory) :
            base(operationResultBuilder)
        {
            this.analyzerFactory = analyzerFactory;
        }

        public override bool IsValid(LambdaExpression expr)
        {
            // expression should be binary expresion
            if (!(expr.Body is BinaryExpression))
            {
                return false;
            }
            var body = expr.Body as BinaryExpression;

            // left operand should be binary expression
            if (!(body.Left is BinaryExpression))
            {
                return false;
            }

            // right operand should be binary expression
            if (!(body.Right is BinaryExpression))
            {
                return false;
            }

            var lambdaParam = expr.Parameters[0];
            // check left operand
            if (!this.isExpressionValid(body.Left as BinaryExpression, lambdaParam))
            {
                return false;
            }

            // check right operand
            if (!this.isExpressionValid(body.Right as BinaryExpression, lambdaParam))
            {
                return false;
            }
            return true;
        }

        private bool isExpressionValid(BinaryExpression subExpr, ParameterExpression lambdaParam)
        {
            // make Expression<Func<SPItem, bool>> lambda expression from BinaryExpression
            var lambda = this.createLambdaFromExpression(subExpr, lambdaParam);
            var subExpressionAnalyzer = this.analyzerFactory.Create(lambda);
            return subExpressionAnalyzer.IsValid(lambda);
        }

        // For composite expressions like x => (string)x["Email"] == "test@example.com" && (int)x["Count1"] == 1
        // it creates 2 lambdas: x => (string)x["Email"] == "test@example.com" ; x => (int)x["Count1"] == 1
        private Expression<Func<SPItem, bool>> createLambdaFromExpression(BinaryExpression subExpr,
            ParameterExpression lambdaParam)
        {
            return Expression.Lambda<Func<SPItem, bool>>(subExpr, lambdaParam);
        }

        private IOperation createOperationFromExpression(BinaryExpression subExpr, ParameterExpression lambdaParam)
        {
            // make Expression<Func<SPItem, bool>> lambda expression from BinaryExpression
            var lambda = this.createLambdaFromExpression(subExpr, lambdaParam);
            var subExpressionAnalyzer = this.analyzerFactory.Create(lambda);
            return subExpressionAnalyzer.GetOperation(lambda);
        }

        protected IOperation getLeftOperation(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as BinaryExpression;
            var lambdaParam = expr.Parameters[0];
            var operation = this.createOperationFromExpression(body.Left as BinaryExpression, lambdaParam);
            return operation;
        }

        protected IOperation getRightOperation(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as BinaryExpression;
            var lambdaParam = expr.Parameters[0];
            var operation = this.createOperationFromExpression(body.Right as BinaryExpression, lambdaParam);
            return operation;
        }
    }
}
