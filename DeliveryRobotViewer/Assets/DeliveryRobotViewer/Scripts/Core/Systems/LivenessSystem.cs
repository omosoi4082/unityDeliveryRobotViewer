using UnityEngine;
using UnityEngine.PlayerLoop;

public class LivenessSystem : MonoBehaviour
{
    [SerializeField] private float timeout = 3f;

    private RobotRegisty _registy;

    public void Initialized(RobotRegisty registy)
    {
        _registy = registy;
    }

    private void Update()
    {
        if (_registy == null) return;
        float now = Time.time;
        foreach (var item in _registy.GetAll())
        {
            if (item.isAlive && now - item.lastSeenTime > timeout)
            {
                item.Disconnected();
            }
        }
    }

}
