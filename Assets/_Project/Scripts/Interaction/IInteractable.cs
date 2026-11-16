using ProjectAllUnits.Player;

namespace ProjectAllUnits.Interaction
{
    /// <summary>Anything a player can look at and press E on.</summary>
    public interface IInteractable
    {
        string Prompt { get; }
        void Interact(PlayerInteractor player);
    }
}
