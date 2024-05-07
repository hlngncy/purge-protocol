using System.Numerics;
using Fusion;


public struct InputData : INetworkInput
{
    public float horizontalMovement;
    public bool horizontalMovementButton;
    public float verticalMovement;
    public float horizontalRotation;
    public float verticalRotation;
    public NetworkButtons networkButtons;
    public bool isFiring;
}
