using UnityEngine;
using UnityEngine.InputSystem;

public static class PointerUtils
{
    private const float StickDeadzone = 0.2f;
    private const float GamepadCursorSpeed = 0.75f;

    private static Vector2 cursorScreenPos;
    private static bool isInitialized = false;

    public static bool GetMouseWorldPosition(Camera camera, Vector2 screenPosition, float planeY, out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;
        if (camera == null) return false;

        Ray ray = camera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            worldPosition = ray.GetPoint(rayDistance);
            return true;
        }

        return false;
    }

    public static bool TryGetAimInfo(InputSystem_Actions controls, Camera camera, Vector3 playerPos,
        float sensitivityMultiplier, bool advanceCursor, out Vector3 direction, out float distance)
    {
        direction = Vector3.zero;
        distance = 0f;

        if (controls == null || camera == null) return false;

        Vector2 screenPos;

        if (GameManager.IsUsingGamepad)
        {
            if (!isInitialized)
            {
                cursorScreenPos = new Vector2(Screen.width, Screen.height) * 0.5f;
                isInitialized = true;
            }

            if (advanceCursor)
            {
                Vector2 stick = controls.Player.Aim.ReadValue<Vector2>();
                if (stick.sqrMagnitude < StickDeadzone * StickDeadzone) stick = Vector2.zero;

                cursorScreenPos += stick * (GamepadCursorSpeed * Screen.height * sensitivityMultiplier) * Time.deltaTime;
                cursorScreenPos.x = Mathf.Clamp(cursorScreenPos.x, 0f, Screen.width);
                cursorScreenPos.y = Mathf.Clamp(cursorScreenPos.y, 0f, Screen.height);
            }

            screenPos = cursorScreenPos;
        }
        else
        {
            screenPos = controls.Player.Look.ReadValue<Vector2>();
            cursorScreenPos = screenPos;
            isInitialized = true;
        }

        if (!GetMouseWorldPosition(camera, screenPos, playerPos.y, out Vector3 worldPos))
            return false;

        Vector3 offset = worldPos - playerPos;
        distance = offset.magnitude;
        if (distance < 0.0001f) return false;

        direction = offset / distance;
        return true;
    }
}