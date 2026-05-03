namespace ProjectManatite.Core
{
    /// <summary>
    /// Implemented by anything chips can be applied to (weapons, turrets, the player).
    /// Lets <c>SlotBehaviour</c> talk to a target without knowing its concrete type.
    /// </summary>
    public interface IEquippable
    {
        /// <summary>Apply a chip's modifiers. Implementations are expected to be idempotent for a given chip instance.</summary>
        void Apply(ChipSO chip);

        /// <summary>Reverse a previously-applied chip's modifiers.</summary>
        void Unapply(ChipSO chip);
    }
}
