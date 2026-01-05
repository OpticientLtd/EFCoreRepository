using System;
using System.Linq;
using System.Linq.Expressions;

namespace Opticient.EFCore.Repository.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Expression{TDelegate}" /> trees,
/// particularly for building and manipulating expressions for querying data.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Returns an expression that always evaluates to true.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the expression.</typeparam>
    /// <returns>An expression that represents a constant true value.</returns>
    public static Expression<Func<T, bool>> True<T>() => f => true;

    /// <summary>
    /// Returns an expression that always evaluates to false.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the expression.</typeparam>
    /// <returns>An expression that represents a constant false value.</returns>
    public static Expression<Func<T, bool>> False<T>() => f => false;

    /// <summary>
    /// Combines two expressions using a logical OR operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the expressions.</typeparam>
    /// <param name="expr1">The first expression.</param>
    /// <param name="expr2">The second expression.</param>
    /// <returns>An expression that represents the logical OR of the two input expressions.</returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        => Expression.Lambda<Func<T, bool>>
              (Expression.OrElse(expr1.Body, Expression.Invoke(expr2, expr1.Parameters)), expr1.Parameters);

    /// <summary>
    /// Combines two expressions using a logical AND operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the expressions.</typeparam>
    /// <param name="expr1">The first expression.</param>
    /// <param name="expr2">The second expression.</param>
    /// <returns>An expression that represents the logical AND of the two input expressions.</returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        => Expression.Lambda<Func<T, bool>>
              (Expression.AndAlso(expr1.Body, Expression.Invoke(expr2, expr1.Parameters)), expr1.Parameters);

    /// <summary>
    /// Converts an expression to a human-readable string representation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the expression.</typeparam>
    /// <param name="expression">The expression to convert.</param>
    /// <returns>A string representation of the expression.</returns>
    public static string ToReadableString<T>(this Expression<Func<T, bool>> expression)
    {
        try
        {
            return ExpressionToString(expression.Body);
        }
        catch (Exception)
        {
            // Do Nothing
        }
        return string.Empty;
    }

    private static string ExpressionToString(Expression expression)
    {
        try
        {
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    return $"{ExpressionToString(binaryExpression.Left)} {GetOperator(binaryExpression.NodeType)} {ExpressionToString(binaryExpression.Right)}";

                case MemberExpression memberExpression:
                    return memberExpression?.Member?.Name ?? string.Empty;

                case ConstantExpression constantExpression:
                    return constantExpression?.Value?.ToString() ?? string.Empty;

                case MethodCallExpression methodCallExpression:
                    var arguments = string.Join(Constants.Separators.Name, methodCallExpression.Arguments.Select(ExpressionToString));
                    return $"{ExpressionToString(methodCallExpression?.Object)}.{methodCallExpression?.Method?.Name}({arguments})";

                case ParameterExpression parameterExpression:
                    return parameterExpression?.Name ?? string.Empty;

                case UnaryExpression unaryExpression:
                    return $"{GetOperator(unaryExpression.NodeType)}{ExpressionToString(unaryExpression.Operand)}";

                default:
                    return expression?.ToString();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
            // Do nothing
        }

        return string.Empty;
    }

    private static string GetOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.AndAlso => "&&",
            ExpressionType.OrElse => "||",
            ExpressionType.Equal => "==",
            ExpressionType.NotEqual => "!=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.Not => "!",
            _ => nodeType.ToString()
        };
    }
}
