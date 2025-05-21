using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [Range(0,360)]
    [SerializeField] public float fov = 90f;
    [SerializeField] public float viewDistance;
    [SerializeField] private int rayCount = 2;
    [SerializeField] private LayerMask environmentLayerMask;
    [SerializeField] private LayerMask playerLayerMask;

    private Mesh mesh;
    private LayerMask combinedLayerMask;
    private Vector3 origin;
    private float startingAngle = 0;

    [HideInInspector]
    public float percentRaysOnPlayer;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        combinedLayerMask = environmentLayerMask | playerLayerMask;
    }

    private void LateUpdate()
    {
        DrawViewCone();   
    }

    private void DrawViewCone()
    {
        float raysOnPlayer = 0;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIdx = 1;
        int triangleIdx = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, VectorFromAngle(angle), viewDistance, combinedLayerMask);
            if (raycastHit2D.collider == null)
            {
                vertex = origin + VectorFromAngle(angle) * viewDistance;
            }
            else if (raycastHit2D.collider.gameObject.CompareTag("Player"))
            {
                vertex = origin + VectorFromAngle(angle) * viewDistance;
                Debug.Log("Can see player!");
                raysOnPlayer++;
            }
            else
            {
                vertex = raycastHit2D.point;
            }
            vertices[vertexIdx] = vertex;

            if (i > 0)
            {
                triangles[triangleIdx + 0] = 0;
                triangles[triangleIdx + 1] = vertexIdx - 1;
                triangles[triangleIdx + 2] = vertexIdx;

                triangleIdx += 3;
            }
            vertexIdx++;
            angle += angleIncrease;
        }
        percentRaysOnPlayer = raysOnPlayer / rayCount;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public static Vector3 VectorFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float AngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetViewDirection(Vector3 direction)
    {
        startingAngle = AngleFromVector(direction) - fov / 2f;
    }
}
