using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace BuildTools.Cli.Extensions;

public class FieldVisitor : ExpressionVisitor
{
    [MemberNotNullWhen(true, nameof(Property))]
    public bool HasProperty => Property is not null;

    public MemberInfo? Member { get; private set; }
    public PropertyInfo? Property { get; private set; }
    public void Reset()
    {
        Member = null;
        Property = null;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        Reset();
        Member = node.Member;
        
        if (Member is PropertyInfo property)
        {
            Property = property;
        }

        return node;
    }
}
