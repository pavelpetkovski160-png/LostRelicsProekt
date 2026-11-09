using UnityEngine;
using UnityEngine.InputSystem; // Unity 6 uses the new Input System by default

[RequireComponent(typeof(Ship))]
public class PlayerShip : MonoBehaviour
{
    Ship ship;
    float throttle, turn;

    void Awake() => ship = GetComponent<Ship>();

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return;
        throttle = (k.wKey.isPressed ? 1f : 0f) - (k.sKey.isPressed ? 0.5f : 0f);
        turn = (k.aKey.isPressed ? 1f : 0f) - (k.dKey.isPressed ? 1f : 0f);
        if (k.spaceKey.isPressed) ship.Fire();
    }

    void FixedUpdate() => ship.Drive(throttle, turn);
}
