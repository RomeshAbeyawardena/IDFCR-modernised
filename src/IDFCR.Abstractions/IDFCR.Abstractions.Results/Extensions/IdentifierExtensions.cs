
using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Results.Extensions;

/// <summary>
/// Defines extension methods for working with identifiable entities and determining their state based on their identifiers.
/// </summary>
public static class IdentifierExtensions
{
    /// <summary>
    /// Determines whether the specified entity state is considered new.
    /// </summary>
    /// <param name="entityState">The entity state to check.</param>
    /// <returns><c>true</c> if the entity state is new; otherwise, <c>false</c>.</returns>
    public static bool IsNew(this EntityState entityState)
    {
        return entityState == EntityState.New;
    }

    /// <summary>
    /// Determines whether the specified entity state is considered an update.
    /// </summary>
    /// <param name="entityState">The entity state to check.</param>
    /// <returns><c>true</c> if the entity state is an update; otherwise, <c>false</c>.</returns>
    public static bool IsUpdate(this EntityState entityState)
    {
        return entityState == EntityState.Update;
    }

    /// <summary>
    /// Gets the considered entity state of an identifiable entity based on its Id and returns the result along with the Id.
    /// </summary>
    /// <param name="identifiable">The identifiable entity to check.</param>
    /// <param name="output">The output tuple containing the Id and the original result of the state check.</param>
    /// <returns>The considered state of the entity.</returns>
    public static EntityState GetEntityState(this IIdentifiable? identifiable,
        out (Guid? Id, IUnitResult Result) output)
    {
        var stateResult = identifiable.IsUpdateState(out var id);
        output = (id, stateResult);

        if (!stateResult.IsSuccess)
        {
            return EntityState.Invalid;
        }

        return stateResult.Result ? EntityState.Update : EntityState.New;
    }

    /// <summary>
    /// Determines whether the identifiable entity is considered to be in an update state based on its Id.
    /// </summary>
    /// <param name="identifiable">The identifiable entity to check.</param>
    /// <param name="id">The Id of the identifiable entity.</param>
    /// <returns>A result indicating whether the entity is considered to be in an update state.</returns>
    public static IUnitResult<bool> IsUpdateState(this IIdentifiable? identifiable, out Guid? id)
    {
        id = null;

        //check for new state
        if (identifiable is null
            || identifiable.Id is null
            || string.IsNullOrWhiteSpace(identifiable.Id.ToString()))
        {
            return UnitResult.FromResult(false);
        }

        //check for valid update state
        if (identifiable.Id.IsOfGuid(out id))
        {
            return UnitResult.FromResult(true);
        }

        return UnitResult.Failed<bool>(new ArgumentException("Invalid Id."));
    }
}
