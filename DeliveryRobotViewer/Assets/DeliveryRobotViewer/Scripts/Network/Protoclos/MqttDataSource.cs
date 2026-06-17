using Cysharp.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MqttDataSource : IRobotDataSource
{
    private readonly RobotDataQueue _queue;
    private readonly MqttTopicConfig _topicConfig;
    private IMqttClient mqttClient;

    public MqttDataSource(RobotDataQueue queue, MqttTopicConfig topicConfig)
    {
        _queue = queue;
        _topicConfig = topicConfig; 
    }

    public async UniTask StartAsync(CancellationToken ct)
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await mqttClient.ConnectAsync(options, ct);//Connect 시도
                foreach (var item in _topicConfig.topics)
                {
                    await mqttClient.SubscribeAsync(item);//"robots/telemetry"
                }
               
                await WaitUntilDisconnected(ct); //연결 살아있으면 계속대기

            }
            catch (OperationCanceledException)
            {
                break;// 제대로 끊어짐“서비스 종료 요청”
            }
            catch (Exception ex)
            {
                //진짜 오류 상황
                await UniTask.Delay(3000, cancellationToken: ct);//3초 대기 다시 Connect 시도
            }
        }
    }

    private async UniTask WaitUntilDisconnected(CancellationToken ct)
    {
        //연결 살아있는동안 0.5씩 체크
        while (mqttClient.IsConnected && !ct.IsCancellationRequested)
        {
            await UniTask.Delay(500, cancellationToken: ct);
        }
    }

    private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var json = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        Debug.Log(json);
        var dto = JsonUtility.FromJson<RobotDTO>(json);
        _queue.Enqueue(dto);
        return Task.CompletedTask;
    }

    public async UniTask StopAsync()
    {
        if (mqttClient != null && mqttClient.IsConnected)
        {
            await mqttClient.DisconnectAsync();
        }
    }


}
