using UnityEngine;

public static class PointerUtils
{
    public static bool ScreenToGroundPoint(Camera camera, Vector2 screenPosition, float planeY, out Vector3 worldPosition)
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

    public static bool TryGetAimInfo(Camera camera, Vector2 screenPosition, Vector3 origin, out Vector3 direction, out float distance)
    {
        direction = Vector3.zero;
        distance = 0f;

        if (!ScreenToGroundPoint(camera, screenPosition, origin.y, out Vector3 worldPos))
            return false;

        Vector3 offset = worldPos - origin;
        distance = offset.magnitude;
        if (distance < 0.0001f) return false;

        direction = offset / distance;
        return true;
    }
}