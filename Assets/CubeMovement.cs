using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class CubeMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    private float speedBoostUntil;
    public float CurrentSpeed => moveSpeed * (Time.time < speedBoostUntil ? 2f : 1f);
    public void ApplyDoubleSpeed() => speedBoostUntil = Time.time + 10f;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        float x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
        float z = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
        Vector3 direction = Vector3.ClampMagnitude(new Vector3(x, 0f, z), 1f);
        transform.position += direction * CurrentSpeed * Time.deltaTime;
    }
}
