using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class RobotMoveItem : MonoBehaviour
{
    public GameObject mark;
    public Action OnClickde;
    public Action OnHoverEnter;
    public Action OnHoverExit;

  /*  public CinemachineCamera cam;*/
    public NavMeshAgent navMeshAgent;

    public MeshRenderer renderer;
    public Material onlive;
    public Material offlive;

    private bool _isInitialized;
    private bool _isAlive;
  
    private void Start()
    {
       
       // cam.Priority = 0;
        renderer.material = onlive;
        mark.SetActive(false);
    }
    public void OnMoveUpData(RobotModel model)
    {
        if (!model.isAlive) return;

        if (!_isInitialized)
        {
            transform.position = model.position;
            transform.rotation = model.rotation;

            if(navMeshAgent!=null&&navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.Warp(model.position);
            }
            _isInitialized = true;
            return;
        }

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(model.position, out hit, 1f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
        transform.rotation = model.rotation;
    }
    public void OnLiveness(RobotModel model)
    {
        _isAlive = model.isAlive;
        renderer.material = _isAlive ? onlive : offlive;
    }

    private void OnMouseEnter() => OnHoverEnter?.Invoke();


    private void OnMouseExit() => OnHoverExit?.Invoke();

    private void OnMouseDown() => OnClickde?.Invoke();



}
