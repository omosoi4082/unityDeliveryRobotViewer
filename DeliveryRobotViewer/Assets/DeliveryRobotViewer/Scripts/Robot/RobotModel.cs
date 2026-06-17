using System;
using UnityEngine;
/// <summary>
/// 도메인 객체
/// </summary>
public class RobotModel
{
    public string robotId { get; }// 프로퍼티읽기 전용   public string robotId >외부에서 수정 가능 

    public float battery { get; private set; }// 프로퍼티읽기 전용 클래스 내부에서만 수정 가능
    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; }  //회전 값 생성
    public bool hsaPayload{ get; private set; }
    public bool isAlive{ get; private set; }
    public float lastSeenTime { get; private set; }
    public RobotState state { get;  set; }


    public Action<RobotModel> OnChange;
    public RobotModel(string robotId )
    {
        this.robotId = robotId;
    }

    public void UpdateRobotInfo(float dtoBattery, Vector3 dtoPosition, Quaternion quaternion,bool paylaod)
    {
        battery = dtoBattery;
        position = dtoPosition;
        rotation = quaternion;
        hsaPayload = paylaod;
        lastSeenTime=Time.time;
        isAlive = true;
    }

    public void SetState(RobotState robot)
    {
        if (state == robot) return; 
        state = robot;

        OnChange?.Invoke(this);
    }
    public void Disconnected()
    {
        if (!isAlive) return;
        isAlive=false;
        OnChange?.Invoke(this);
    }
}
