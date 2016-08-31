using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Happy;

public class FieldOfView : MonoBehaviour
{
    public struct ViewCastInfo
    {
        public bool HasHit;
        public Vector3 Point;
        public float Distance;
        public float Angle;

        public ViewCastInfo(bool hasHit, Vector3 point, float distance, float angle)
        {
            HasHit = hasHit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }

    public float ViewRadius = 5.0f;
    [Range(0.0f, 180.0f)]
    public float ViewAngle = 60.0f;

    public LayerMask TargetLayerMask;
    public LayerMask ObstaclesLayerMask;

    [Header("Mesh Properties")]
	public Material DefaultMeshMaterial;
	public Material AlertMeshMaterial;
    public float DrawMeshOffset = 0.5f;
    public int MeshTriangleCount = 10;
    public int EdgeResolveIterations = 6;
    public float EdgeDistanceThreshold = 2.0f;

    private Mesh _viewMesh;
    private MeshFilter _viewMeshFilter;
	private MeshRenderer _viewMeshRenderer;
	private float _drawMeshRadius;

    [Header("Read-Only")]
    [SerializeField]
    private Transform _targetFound;
    private Transform _transform;

	public CustomUnityEvent<Transform> OnTargetFoundEvent = new CustomUnityEvent<Transform>();
	public CustomUnityEvent<Transform> OnTargetLostEvent = new CustomUnityEvent<Transform>();

    void Awake()
    {
        _viewMesh = new Mesh();
        _viewMeshFilter = GetComponent<MeshFilter>();
        _viewMeshFilter.mesh = _viewMesh;

		_viewMeshRenderer = GetComponent<MeshRenderer> ();
		_viewMeshRenderer.material = DefaultMeshMaterial;

		_drawMeshRadius = ViewRadius + DrawMeshOffset;

        _transform = transform;
    }

    void Update()
    {
        #region Testing
        //		FindVisibleTarget ();
        //		Debug.Log(SeekVisibleTarget(TargetToSeek));
        #endregion

        //Supports only one object In and Out
        //TODO: List of Transforms for multiple
        if (!_targetFound)
        {
            _targetFound = FindVisibleTarget();

			if (_targetFound && OnTargetFoundEvent != null)
			{
				_viewMeshRenderer.material = AlertMeshMaterial;

				OnTargetFoundEvent.Invoke (_targetFound);

				// TODO: temporary... is there a better way? = Switched in Enemy.cs
                //FieldOfView shouldn't invoke directly any methods (it will not be reusable)
			}
        }
        else
        {
            if (!SeekVisibleTarget(_targetFound) && OnTargetLostEvent != null)
            {
				_viewMeshRenderer.material = DefaultMeshMaterial;

                OnTargetLostEvent.Invoke(_targetFound);
		
                _targetFound = null;
            }
        }

        DrawFieldOfView();
    }

    void OnDrawGizmos()
    {
//        GizmosExtension.DrawCircle(transform.position, ViewRadius, Color.white, 32);
//        GizmosExtension.DrawLine(transform.position, transform.position + DirectionFromAngle(ViewAngle / 2.0f) * ViewRadius, Color.white);
//        GizmosExtension.DrawLine(transform.position, transform.position + DirectionFromAngle(-ViewAngle / 2.0f) * ViewRadius, Color.white);

        if (_targetFound)
            GizmosExtension.DrawLine(transform.position, _targetFound.position, Color.red);
    }

    private Vector3 DirectionFromAngle(float angle)
    {
        angle += transform.eulerAngles.z;
        float angleRad = angle * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0.0f);
    }

    public Transform FindVisibleTarget()
    {
        Transform targetFound = null;

        Collider2D targetInViewRadius = Physics2D.OverlapCircle(_transform.position, ViewRadius, TargetLayerMask.value);

        if (targetInViewRadius)
        {
            Transform target = targetInViewRadius.transform;
            Vector3 directionToTarget = (target.position - _transform.position).normalized;

            if (Vector3.Angle(_transform.right, directionToTarget) <= (ViewAngle / 2.0f))
            {
                if (!Physics2D.Linecast(_transform.position, target.position, ObstaclesLayerMask))
                    targetFound = target;
            }
        }

        return targetFound;
    }

    public bool SeekVisibleTarget(Transform targetToSeek)
    {
        return targetToSeek == FindVisibleTarget();
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCastInfo, ViewCastInfo maxViewCastInfo)
    {
        float minAngle = minViewCastInfo.Angle;
        float maxAngle = maxViewCastInfo.Angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < EdgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2.0f;
            ViewCastInfo viewCastInfo = ViewCast(angle);

            bool isEdgeDistanceThresholdExceeded = Mathf.Abs(minViewCastInfo.Distance - viewCastInfo.Distance) > EdgeDistanceThreshold;

            if (viewCastInfo.HasHit == minViewCastInfo.HasHit && !isEdgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = viewCastInfo.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = viewCastInfo.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private ViewCastInfo ViewCast(float castAngle)
    {
        Vector3 castDirection = DirectionFromAngle(castAngle);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(_transform.position, castDirection, _drawMeshRadius, ObstaclesLayerMask);

        if (raycastHit2D)
            return new ViewCastInfo(true, raycastHit2D.point, raycastHit2D.distance, castAngle);

		return new ViewCastInfo(false, transform.position + castDirection * _drawMeshRadius, _drawMeshRadius, castAngle);
    }

    private void DrawFieldOfView()
    {
        float stepAngleInterval = ViewAngle / MeshTriangleCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCastInfo = new ViewCastInfo();

        for (int i = 0; i <= MeshTriangleCount; i++)
        {
            float angle = (ViewAngle / 2.0f) - (stepAngleInterval * i);
            ViewCastInfo viewCastInfo = ViewCast(angle);

            if (i > 0)
            {
                bool isEdgeDistanceThresholdExceeded = Mathf.Abs(oldViewCastInfo.Distance - viewCastInfo.Distance) > EdgeDistanceThreshold;

                if (oldViewCastInfo.HasHit != viewCastInfo.HasHit || (oldViewCastInfo.HasHit && viewCastInfo.HasHit && isEdgeDistanceThresholdExceeded))
                {
                    EdgeInfo edgeInfo = FindEdge(oldViewCastInfo, viewCastInfo);

                    if (edgeInfo.PointA != Vector3.zero)
                        viewPoints.Add(edgeInfo.PointA);

                    if (edgeInfo.PointB != Vector3.zero)
                        viewPoints.Add(edgeInfo.PointB);
                }
            }

            viewPoints.Add(viewCastInfo.Point);
            oldViewCastInfo = viewCastInfo;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < viewPoints.Count; i++)
        {
            vertices[i + 1] = _transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}
