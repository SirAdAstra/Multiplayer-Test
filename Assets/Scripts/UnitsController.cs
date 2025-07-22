using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class UnitsController : MonoBehaviour
{
    [SerializeField] private LayerMask unitLayer, groundLayer, obstacleLayer;

    private NavMeshAgent selectedAgent;
    private LineRenderer lineRenderer;
    private LineRenderer unitLineRenderer;

    public void OnSelectAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedAgent != null)
            {
                DeselectUnit();
            }

            SelectUnit();
        }
    }

    public void OnBuildAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BuildPath();
        }
    }

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveUnit();
        }
    }

    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
    }

    public void FixedUpdate()
    {
        if (selectedAgent != null && selectedAgent.hasPath)
        {
            DrawPath(selectedAgent.path);
        }
    }

    private void SelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, unitLayer))
        {
            if (hit.collider.gameObject.TryGetComponent(out NavMeshAgent agent))
            {
                selectedAgent = agent;
                unitLineRenderer = selectedAgent.GetComponent<LineRenderer>();
                selectedAgent.GetComponent<Unit>().Select();
                DrawAttackRange();
            }
        }
    }

    private void BuildPath()
    {
        if (selectedAgent == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            selectedAgent.ResetPath();

            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(selectedAgent.transform.position, hit.point, NavMesh.AllAreas, path))
            {
                float pathLength = 0f;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    pathLength += Vector3.Distance(i == 0 ? selectedAgent.transform.position : path.corners[i - 1], path.corners[i]);
                }
                pathLength += Vector3.Distance(path.corners[path.corners.Length - 1], hit.point);

                Debug.Log($"Calculated path length: {pathLength}");
                if (pathLength > selectedAgent.GetComponent<Unit>().Speed)
                {
                    Debug.LogWarning("Path exceeds unit's range, cannot set path.");
                    lineRenderer.positionCount = 0;
                    DrawAttackRange();
                    return;
                }

                if (selectedAgent.SetPath(path))
                {
                    selectedAgent.isStopped = true;
                    DrawAttackRange();
                }
            }
        }
    }

    private void MoveUnit()
    {
        if (selectedAgent == null) return;
        if (selectedAgent.destination == null) return;

        selectedAgent.isStopped = false;
    }

    private void DeselectUnit()
    {
        if (selectedAgent.isStopped)
            selectedAgent.ResetPath();

        selectedAgent.GetComponent<Unit>().Deselect();
        selectedAgent = null;

        lineRenderer.positionCount = 0;
        unitLineRenderer.positionCount = 0;
    }

    private void DrawPath(NavMeshPath path)
    {
        if (path == null) return;

        lineRenderer.positionCount = path.corners.Length + 1;

        for (int i = 0; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }

        lineRenderer.SetPosition(path.corners.Length, selectedAgent.destination);
    }

    private void DrawAttackRange()
    {
        if (selectedAgent == null) return;

        unitLineRenderer.positionCount = 20;
        Vector3 center = selectedAgent.hasPath ? selectedAgent.destination : selectedAgent.transform.position;
        float angleStep = 360f / unitLineRenderer.positionCount;
        float radius = selectedAgent.GetComponent<Unit>().Range;

        for (int i = 0; i < unitLineRenderer.positionCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            Ray ray = new Ray(center, point);
            if (Physics.Raycast(ray, out RaycastHit hit, radius, obstacleLayer))
            {
                point = hit.point - center;
            }

            unitLineRenderer.SetPosition(i, center + point);
        }
    }
}
