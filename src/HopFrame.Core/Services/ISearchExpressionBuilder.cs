using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Core.Config;

namespace HopFrame.Core.Services;

public interface ISearchExpressionBuilder {
    Expression? BuildSearchExpression(TableConfig table, string searchTerm, ParameterExpression parameter);
}