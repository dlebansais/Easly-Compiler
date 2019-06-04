namespace EaslyCompiler
{
    /// <summary>
    /// Features that support the event type.
    /// </summary>
    public interface IFeatureWithEvents
    {
        /// <summary>
        /// The resolved event type.
        /// </summary>
        BaseNode.EventType ResolvedEventType { get; }

        /// <summary>
        /// Sets the <see cref="ResolvedEventType"/> property.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="isConflicting">True upon return if <paramref name="eventType"/> is conflicting with a previous call.</param>
        void SetEventType(BaseNode.EventType eventType, out bool isConflicting);
    }
}
