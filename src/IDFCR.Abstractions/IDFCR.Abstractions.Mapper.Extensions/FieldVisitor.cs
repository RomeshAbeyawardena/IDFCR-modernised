using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace IDFCR.Abstractions.Mapper.Extensions;

/// <summary>
/// Represents a class that extends the functionality of the ExpressionVisitor class to provide specific behavior for visiting member expressions. This class is designed
/// </summary>
public class FieldVisitor : ExpressionVisitor
{
    /// <summary>
    /// Gets a value indicating whether the visited member expression represents a property. This property checks if the Property field is not null, which indicates that the visited member is indeed a property. By using this property, the code can easily determine if the visited member expression corresponds to a property, allowing for specific handling or processing based on this information. The HasProperty property provides a convenient way to check for the presence of a property in the visited member expression, improving readability and maintainability when working with member expressions in the context of CLI extensions.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Property))]
    public bool HasProperty => Property is not null;

    /// <summary>
    /// Gets the MemberInfo object representing the member accessed in the visited member expression. This property holds information about the member being accessed, such as its name, type, and other metadata. The Member property is set during the visit of a member expression, allowing for retrieval of the member information for further processing or analysis. By accessing this property, the code can obtain details about the member being accessed in the expression, which can be useful for various purposes such as reflection, validation, or dynamic behavior based on the member's characteristics.
    /// </summary>
    public MemberInfo? Member { get; private set; }

    /// <summary>
    /// Gets the PropertyInfo object representing the property accessed in the visited member expression, if applicable. This property holds information about the property being accessed, such as its name, type, and other metadata. The Property property is set during the visit of a member expression if the accessed member is identified as a property, allowing for retrieval of the property information for further processing or analysis. By accessing this property, the code can obtain details about the property being accessed in the expression, which can be useful for various purposes such as reflection, validation, or dynamic behavior based on the property's characteristics. If the visited member expression does not correspond to a property, this property will be null.
    /// </summary>
    public PropertyInfo? Property { get; private set; }

    /// <summary>
    /// Defines a method to reset the state of the FieldVisitor by clearing the Member and Property properties. This method can be called to reset the visitor's state before visiting a new member expression, ensuring that any previous member information is cleared and does not interfere with the processing of subsequent member expressions. By invoking this method, the code can maintain a clean state for the visitor, allowing for accurate retrieval of member information during each visit without residual data from previous visits affecting the results.
    /// </summary>
    public void Reset()
    {
        Member = null;
        Property = null;
    }

    /// <summary>
    /// Defines a method to visit a MemberExpression node in the expression tree. This method overrides the VisitMember method from the ExpressionVisitor class to provide specific behavior for handling member expressions. When a member expression is visited, the method resets the state of the visitor, sets the Member property to the accessed member, and if the member is identified as a property, it also sets the Property property accordingly. By implementing this method, the FieldVisitor can effectively capture and store information about the member being accessed in the expression, allowing for further processing or analysis based on that information. The method returns the original node after processing, allowing for continued traversal of the expression tree as needed.
    /// </summary>
    /// <param name="node">The MemberExpression node to visit.</param>
    /// <returns>The original MemberExpression node after processing.</returns>
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
