using IDFCR.Abstractions.Interceptors;

namespace IDFCR.Abstractions.Persistence
{
    internal class RepositoryInterceptorContext
        : IEntityInterceptorContext
    {
        private RepositoryInterceptorContext() { }

        public static RepositoryInterceptorContext Create(EntityContextBehaviorStage Stage,
            EntityContextBehavior Behavior,
            object Model)
        {
            return new RepositoryInterceptorContext
            {
                Stage = Stage,
                Behavior = Behavior,
                Model = Model
            };
        }

        public EntityContextBehaviorStage Stage { get; init; }
        public EntityContextBehavior Behavior { get; init; }
        public object? Model { get; init; }
        /// <summary>
        /// Indicates that the underlying persistence operation (e.g. Add, Update, Delete)
        /// should be skipped by the repository where supported.
        /// 
        /// Interceptors may still run before and after the operation.
        /// 
        /// This is primarily intended for scenarios such as soft deletes,
        /// no-op updates, or environment-based write suppression.
        /// </summary>
        public bool BypassOperation { get; set; }
    }
}
