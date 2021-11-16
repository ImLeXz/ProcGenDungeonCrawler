using UnityEngine;

namespace ATDungeon.Player.Movement
{
    public interface IPlayerMovement
    {
        PlayerMoveData moveData { get; }
        Collider collider { get; }
        GameObject groundObject { get; set; }
        Vector3 forward { get; }
        Vector3 right { get; }
        Vector3 up { get; }
        Vector3 baseVelocity { get; }

    }
}
