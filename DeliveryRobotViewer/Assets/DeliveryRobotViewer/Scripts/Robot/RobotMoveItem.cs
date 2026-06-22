using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class RobotMoveItem : MonoBehaviour
{
    public GameObject mark;
    public TMP_Text id;
    public Action OnClickde;
    public Action OnHoverEnter;
    public Action OnHoverExit;
    public MeshRenderer renderer;
    public Material onlive;
    public Material offlive;
    
    private bool _isAlive;

    [Header("Lerp Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private bool _hasTarget;

    

    private void Start()
    {
       
        renderer.material = onlive;
        mark.SetActive(false);
    }

    private void Update()
    {
        if (!_hasTarget) return;
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
        transform.rotation = _targetRotation;
    }

    public void OnMoveUpData(RobotModel model)
    {
        if (!model.isAlive) return;

        _targetPosition = model.position;
        _targetRotation = model.rotation;
        _hasTarget = true;
       
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