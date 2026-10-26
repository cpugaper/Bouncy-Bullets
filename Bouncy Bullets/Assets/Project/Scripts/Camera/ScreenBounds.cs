using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenBounds : MonoBehaviour
{
    public static ScreenBounds Instance { get; private set; }

    [Header("Arena")]
    [SerializeField, Min(1f)] private float arenaScale = 1.3f;
    [SerializeField, Min(0f)] private float playerEdgeMargin = 1.5f;

    public float MinX { get; private set; }
    public float MaxX { get; private set; }
    public float MinZ { get; private set; }
    public float MaxZ { get; private set; }

    public float PlayerMinX => MinX + playerEdgeMargin;
    public float PlayerMaxX => MaxX - playerEdgeMargin;
    public float PlayerMinZ => MinZ + playerEdgeMargin;
    public float PlayerMaxZ => MaxZ - playerEdgeMargin;

    private Camera cam;
    private Vector2 HalfView => new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
            return; 
        }
        Instance = this;
        cam = GetComponent<Camera>();
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        Vector2 halfArena = HalfView * arenaScale;
        Vector3 c = cam.transform.position;

        MinX = c.x - halfArena.x;
        MaxX = c.x + halfArena.x;
        MinZ = c.z - halfArena.y;
        MaxZ = c.z + halfArena.y;
    }

    public Rect GetVisibleArea()
    {
        Vector3 p = cam.transform.position;
        return new Rect(p.x - HalfView.x, p.z - HalfView.y, HalfView.x * 2f, HalfView.y * 2f);
    }

    public Vector3 GetCameraPositionFor(Vector3 playerPos, float cameraY)
    {
        float x = MapAxis(playerPos.x, PlayerMinX, PlayerMaxX, MinX + HalfView.x, MaxX - HalfView.x);
        float z = MapAxis(playerPos.z, PlayerMinZ, PlayerMaxZ, MinZ + HalfView.y, MaxZ - HalfView.y);
        return new Vector3(x, cameraY, z);
    }

    // Keeps player and camera in sync: both are at the same percentage of their own range.
    private static float MapAxis(float playerCoord, float playerMin, float playerMax, float camMin, float camMax)
    {
        if (playerMax <= playerMin || camMin >= camMax) return (camMin + camMax) * 0.5f;
        float t = Mathf.InverseLerp(playerMin, playerMax, playerCoord);
        return Mathf.Lerp(camMin, camMax, t);
    }

    public bool KeepInsideView(ref Vector3 position, ref Vector3 velocity)
    {
        Rect view = GetVisibleArea();
        bool moved = false;

        if (position.x > view.xMax) { position.x = view.xMax; velocity.x = -Mathf.Abs(velocity.x); moved = true; }
        else if (position.x < view.xMin) { position.x = view.xMin; velocity.x = Mathf.Abs(velocity.x); moved = true; }

        if (position.z > view.yMax) { position.z = view.yMax; velocity.z = -Mathf.Abs(velocity.z); moved = true; }
        else if (position.z < view.yMin) { position.z = view.yMin; velocity.z = Mathf.Abs(velocity.z); moved = true; }

        return moved;
    }
}
