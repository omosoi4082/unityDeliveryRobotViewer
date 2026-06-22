using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WebSocketClient : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private int port = 8765;
    [SerializeField] private Button connet;

    private RobotDataQueue _queue;
    private WebSocket _websocket;
    private CancellationTokenSource _cts;

    // 외부에서 큐 주입 (MqttDataSource 생성자 주입과 동일한 역할)
    public void Init(RobotDataQueue queue)
    {
        _queue = queue;
    }

    void Start()
    {
        _cts = new CancellationTokenSource();
        ipInputField.text = "192.168.4.2";
        connet.onClick.AddListener(OnConnectButtonClicked);
    }
    public void OnConnectButtonClicked()
    {
        string ip = ipInputField.text.Trim();
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogWarning("[WS] IP를 입력해주세요.");
            return;
        }

        string url = $"ws://{ip}:{port}";
      
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        ConnectLoopAsync(url, _cts.Token).Forget();
    }

    private async UniTaskVoid ConnectLoopAsync(string url,CancellationToken ct)
    {
        Debug.Log("[WS] 연결 시도: " + url);
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _websocket = new WebSocket(url);

                _websocket.OnOpen += () => Debug.Log("[WS] 연결 성공");
                _websocket.OnClose += (e) => Debug.Log("[WS] 연결 종료");
                _websocket.OnError += (e) => Debug.LogWarning("[WS] 에러: " + e);
                _websocket.OnMessage += OnMessage;

                await _websocket.Connect();         // 연결 시도
                await WaitUntilDisconnected(ct);    // 연결 살아있는 동안 대기
            }
            catch (OperationCanceledException)
            {
                break; // 앱 종료 or 명시적 취소
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[WS] 재연결 대기: {ex.Message}");
                await UniTask.Delay(3000, cancellationToken: ct);
            }
        }
    }

    private async UniTask WaitUntilDisconnected(CancellationToken ct)
    {
        while (_websocket.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            await UniTask.Delay(500, cancellationToken: ct);
        }
    }

    private void OnMessage(byte[] bytes)
    {
        try
        {
            string json = Encoding.UTF8.GetString(bytes);
            RobotDTO dto = JsonUtility.FromJson<RobotDTO>(json);

            if (dto == null || string.IsNullOrEmpty(dto.robotId))
                return;

            _queue.Enqueue(dto);
          
        }
        catch (Exception e)
        {
            Debug.LogWarning("[WS] 파싱 실패: " + e.Message);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        _cts?.Cancel();
        if (_websocket != null && _websocket.State == WebSocketState.Open)
            await _websocket.Close();
    }
}