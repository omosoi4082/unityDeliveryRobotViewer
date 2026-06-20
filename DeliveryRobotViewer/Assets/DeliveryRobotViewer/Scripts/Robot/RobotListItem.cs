using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RobotListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RobotModel _model;
    public Image bg;
    public TextMeshProUGUI id;
    public TextMeshProUGUI px;
    public TextMeshProUGUI pz;
    public TextMeshProUGUI yaw;
   
    public TextMeshProUGUI battery;
    public Image filled;
    [SerializeField] private Image onlineimg;
  

    public TextMeshProUGUI situation;
    public TextMeshProUGUI haspaylaod;
    public TextMeshProUGUI online;
    public Action OnClickde;
    public Action OnHoverEnter;
    public Action OnHoverExit;

    private bool _isAlive;
    public Color fNormal;
    public Color fWarning;
    public Color fDanger;
    public Color enter;
    public Color exit;

    [SerializeField] Color on;
    [SerializeField] Color off;

    public void OnPointerEnter(PointerEventData eventData) => OnHoverEnter?.Invoke();

    public void OnPointerClick(PointerEventData eventData) => OnClickde?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => OnHoverExit?.Invoke();

    public void OnListUpData(RobotModel model)
    {
        if (!model.isAlive ) return;
        _model = model;
        id.text = model.robotId;
        px.text = model.position.x.ToString("F1");
        pz.text = model.position.z.ToString("F1");
        Vector3 euler = model.rotation.eulerAngles;
        yaw.text = euler.ToString("F1");
        battery.text = model.battery.ToString();
        situation.text = model.situation;
        //haspaylaod.text = model.hsaPayload ? "ON" : "Dff";
        StateUpdate(model.state);
    }
    public void OnLiveness(RobotModel model)
    {
        _isAlive = model.isAlive;
        online.text = _isAlive ? "Online" : "Offline";
        onlineimg.color = _isAlive ? on : off;

    }


    private void StateUpdate(RobotState state)
    {
        switch (state)
        {
            case RobotState.Normal:
                filled.color = fNormal;
                break;
            case RobotState.Warning:
                filled.color = fWarning;
                break;
            case RobotState.Danger:
                filled.color = fDanger;
                break;
            default:
                filled.color = fNormal;
                break;
        }
    }
}
