using UnityEngine;
/// <summary>
/// Unity Editor의 Scene View 카메라와 유사한 동작을 제공하는 카메라 컨트롤러
/// - 마우스 우클릭 + WASD: 1인칭 시점 이동
/// - 마우스 휠클릭 + 드래그: 팬 (평면 이동)
/// - Alt + 좌클릭: 타겟 중심 회전 (오빗)
/// - 마우스 휠: 줌 인/아웃
/// - F키: 선택된 오브젝트에 포커스
/// </summary>
public class SceneViewCameraController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastMoveMultiplier = 3f;
    [SerializeField] private float slowMoveMultiplier = 0.3f;

    [Header("회전 설정")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float orbitSensitivity = 0.3f;

    [Header("줌 설정")]
    [SerializeField] private float zoomSensitivity = 2f;
    [SerializeField] private float minZoomSpeed = 0.5f;
    [SerializeField] private float maxZoomSpeed = 50f;

    [Header("팬 설정")]
    [SerializeField] private float panSensitivity = 0.5f;

    [Header("오빗 설정")]
    [SerializeField] private Vector3 orbitTarget = Vector3.zero;
    [SerializeField] private bool autoUpdateOrbitTarget = true;

    private Vector3 lastPanPosition;
    private bool isPanning = false;
    private bool isOrbiting = false;
    private bool isLooking = false;

    void Update()
    {
        HandleMouseInput();
        HandleKeyboardMovement();
        HandleZoom();
        HandleFocusKey();
    }

    /// <summary>
    /// 마우스 입력 처리
    /// </summary>
    void HandleMouseInput()
    {
        // 우클릭: 1인칭 시점 회전
        if (Input.GetMouseButtonDown(1))
        {
            isLooking = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isLooking = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (isLooking)
        {
            Look();
        }

        // 휠클릭: 팬 (평면 이동)
        if (Input.GetMouseButtonDown(2))
        {
            isPanning = true;
            lastPanPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Pan();
        }

        // Alt + 좌클릭: 오빗 (타겟 중심 회전)
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButtonDown(0))
            {
                isOrbiting = true;
                UpdateOrbitTarget();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isOrbiting = false;
            }

            if (isOrbiting)
            {
                Orbit();
            }
        }
    }

    /// <summary>
    /// 1인칭 시점 회전 (우클릭 상태)
    /// </summary>
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up, mouseX, Space.World);
        transform.Rotate(Vector3.right, -mouseY, Space.Self);

        // 카메라 상하 각도 제한 (90도 이상 회전 방지)
        Vector3 currentRotation = transform.localEulerAngles;
        if (currentRotation.x > 180)
            currentRotation.x -= 360;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -89f, 89f);
        transform.localEulerAngles = currentRotation;
    }

    /// <summary>
    /// 키보드로 이동 (WASD + QE)
    /// </summary>
    void HandleKeyboardMovement()
    {
        if (!isLooking) return;

        float currentSpeed = moveSpeed;

        // Shift: 빠른 이동
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= fastMoveMultiplier;
        }
        // Ctrl: 느린 이동
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            currentSpeed *= slowMoveMultiplier;
        }

        Vector3 movement = Vector3.zero;

        // WASD: 전후좌우 이동
        if (Input.GetKey(KeyCode.W)) movement += transform.forward;
        if (Input.GetKey(KeyCode.S)) movement -= transform.forward;
        if (Input.GetKey(KeyCode.A)) movement -= transform.right;
        if (Input.GetKey(KeyCode.D)) movement += transform.right;

        // QE: 상하 이동
        if (Input.GetKey(KeyCode.E)) movement += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) movement += Vector3.down;

        transform.position += movement.normalized * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 팬 (휠클릭 + 드래그로 평면 이동)
    /// </summary>
    void Pan()
    {
        Vector3 mouseDelta = Input.mousePosition - lastPanPosition;
        lastPanPosition = Input.mousePosition;

        Vector3 panMovement = -transform.right * mouseDelta.x * panSensitivity * Time.deltaTime;
        panMovement -= transform.up * mouseDelta.y * panSensitivity * Time.deltaTime;

        transform.position += panMovement;

        if (autoUpdateOrbitTarget)
        {
            orbitTarget += panMovement;
        }
    }

    /// <summary>
    /// 오빗 (Alt + 좌클릭으로 타겟 중심 회전)
    /// </summary>
    void Orbit()
    {
        float mouseX = Input.GetAxis("Mouse X") * orbitSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * orbitSensitivity;

        // 타겟을 중심으로 회전
        transform.RotateAround(orbitTarget, Vector3.up, mouseX);
        transform.RotateAround(orbitTarget, transform.right, -mouseY);

        // 항상 타겟을 바라보도록
        transform.LookAt(orbitTarget);
    }

    /// <summary>
    /// 마우스 휠로 줌 인/아웃
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // 카메라와 타겟 사이 거리에 따라 줌 속도 조절
            float distanceToTarget = Vector3.Distance(transform.position, orbitTarget);
            float zoomSpeed = Mathf.Clamp(distanceToTarget * zoomSensitivity, minZoomSpeed, maxZoomSpeed);

            transform.position += transform.forward * scroll * zoomSpeed;
        }
    }

    /// <summary>
    /// F키로 선택된 오브젝트에 포커스
    /// </summary>
    void HandleFocusKey()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FocusOnSelection();
        }
    }

    /// <summary>
    /// 선택된 오브젝트나 현재 타겟에 포커스
    /// </summary>
    void FocusOnSelection()
    {
        // Unity Editor가 아닌 런타임에서는 orbitTarget에 포커스
        Vector3 targetPosition = orbitTarget;

        // 포커스할 대상의 크기 계산 (Renderer 기반)
        float focusDistance = 10f; // 기본 거리

        Collider[] colliders = Physics.OverlapSphere(targetPosition, 50f);
        if (colliders.Length > 0)
        {
            Bounds bounds = new Bounds(targetPosition, Vector3.zero);
            foreach (Collider col in colliders)
            {
                bounds.Encapsulate(col.bounds);
            }
            focusDistance = Mathf.Max(bounds.size.magnitude * 1.5f, 2f);
        }

        // 카메라를 타겟 뒤쪽으로 부드럽게 이동
        Vector3 desiredPosition = targetPosition - transform.forward * focusDistance;
        StartCoroutine(SmoothFocus(desiredPosition, targetPosition));
    }

    /// <summary>
    /// 부드러운 포커스 이동
    /// </summary>
    System.Collections.IEnumerator SmoothFocus(Vector3 targetPos, Vector3 lookAtPos)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smoothstep

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.LookAt(lookAtPos);

            yield return null;
        }

        transform.position = targetPos;
        transform.LookAt(lookAtPos);
        orbitTarget = lookAtPos;
    }

    /// <summary>
    /// 현재 카메라가 보고 있는 지점을 오빗 타겟으로 업데이트
    /// </summary>
    void UpdateOrbitTarget()
    {
        if (autoUpdateOrbitTarget)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                orbitTarget = hit.point;
            }
            else
            {
                // 히트가 없으면 카메라 앞 일정 거리를 타겟으로
                orbitTarget = transform.position + transform.forward * 10f;
            }
        }
    }

    /// <summary>
    /// Inspector에서 오빗 타겟 위치 시각화
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(orbitTarget, 0.5f);
        Gizmos.DrawLine(transform.position, orbitTarget);
    }

    /// <summary>
    /// 외부에서 오빗 타겟 설정
    /// </summary>
    public void SetOrbitTarget(Vector3 target)
    {
        orbitTarget = target;
    }

    /// <summary>
    /// 외부에서 타겟을 보도록 설정
    /// </summary>
    public void LookAtTarget(Vector3 target, float distance = 10f)
    {
        orbitTarget = target;
        Vector3 direction = (transform.position - target).normalized;
        transform.position = target + direction * distance;
        transform.LookAt(target);
    }
}
