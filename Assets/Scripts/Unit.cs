using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public Team Team { get { return team; } }
    public float Speed { get { return speed; } }
    public float Range { get { return range; } }

    [SerializeField] private Team team;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private GameObject selectedMarker;
    private bool isSelected = false;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    public void Awake()
    {
        if (selectedMarker == null)
        {
            selectedMarker = transform.GetChild(0).gameObject;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.enabled = true;
    }

    public void Update()
    {
        if (!isSelected && agent.enabled)
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 0.1f)
                DisableAgent();
    }

    public void Select()
    {
        selectedMarker.SetActive(true);
        isSelected = true;
        
        StartCoroutine(EnableAgent());
    }

    public void Deselect()
    {
        selectedMarker.SetActive(false);
        isSelected = false;
    }

    private IEnumerator EnableAgent()
    {
        //Vector3 position = transform.position;
        obstacle.enabled = false;
        yield return null;
        agent.enabled = true;
        //transform.position = position;
    }

    private void DisableAgent()
    {
        agent.enabled = false;
        obstacle.enabled = true;
    }
}
