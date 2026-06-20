using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
class FrameData
{
    public Dictionary<ushort, byte[]> chunks = new Dictionary<ushort, byte[]>();
    public ushort total;
}


public class VideoreceiverUDP : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RawImage displayImage;
    [SerializeField] private GameObject noSignalText;   // "영상 없음" 텍스트

    [Header("Network")]
    [SerializeField] private int listenPort = 9100;
    [SerializeField] private float noSignalTimeout = 3f;  // 몇 초 후 영상없음 표시
    private UdpClient _udpClient;
    private Thread _receiveThread;
    private bool _running;

    private Dictionary<uint, FrameData> _frameBuffer = new Dictionary<uint, FrameData>();
    // 프레임 재조합 버퍼 — frame_id → (chunks dict, total)
   // private readonly Dictionary<uint, (Dictionary<ushort, byte[]> chunks, ushort total)> _frameBuffer = new();

    // 메인 스레드에 넘길 완성된 JPEG
    private byte[] _pendingJpeg;
    private readonly object _lock = new();
    private Texture2D _texture;
    private float _lastFrameTime;
    private bool _showingNoSignal = true;

    void Start()
    {
        displayImage.color = Color.black;
        noSignalText.SetActive(true);

        _lastFrameTime = Time.time;
        _udpClient = new UdpClient(listenPort);
        _texture = new Texture2D(2, 2);
        _running = true;
        _receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
        _receiveThread.Start();
       
        Debug.Log($"[VideoUDP] 수신 대기: {listenPort}");
    }

    void Update()
    {
        byte[] jpeg;
        lock (_lock)
        {
            jpeg = _pendingJpeg;
            _pendingJpeg = null;
        }

        if (jpeg == null)
        {
            if (Time.time - _lastFrameTime > noSignalTimeout && !_showingNoSignal)
                ShowNoSignal();
            return;
        }

        _lastFrameTime = Time.time;
        _showingNoSignal = false;

        // JPEG → Texture2D → RawImage
        if (_texture.LoadImage(jpeg))
        {
            displayImage.texture = _texture;
            displayImage.color = Color.white;
            noSignalText.gameObject.SetActive(false);
        }
    }
    private void ShowNoSignal()
    {
        _showingNoSignal = true;
        displayImage.texture = null;
        displayImage.color = Color.black;
        noSignalText.gameObject.SetActive(true);
    }

    private void ReceiveLoop()
    {
        var endpoint = new IPEndPoint(IPAddress.Any, listenPort);

        while (_running)
        {
            try
            {
                byte[] data = _udpClient.Receive(ref endpoint);
                if (data.Length < 8) continue;

                // 헤더 파싱 (big-endian)
                uint frameId = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
                ushort chunkId = (ushort)((data[4] << 8) | data[5]);
                ushort totalChunks = (ushort)((data[6] << 8) | data[7]);

                byte[] payload = new byte[data.Length - 8];
                Buffer.BlockCopy(data, 8, payload, 0, payload.Length);

                AssembleFrame(frameId, chunkId, totalChunks, payload);
            }
            catch (SocketException)
            {
                break; // 소켓 닫힘
            }
            catch (Exception e)
            {
                Debug.LogWarning("[VideoUDP] 수신 오류: " + e.Message);
            }
        }
    }

    private void AssembleFrame(uint frameId, ushort chunkId, ushort total, byte[] payload)
    {
        if (!_frameBuffer.ContainsKey(frameId))
        {
            _frameBuffer[frameId] = new FrameData { total = total };
        }

        /*_frameBuffer[frameId] = (new Dictionary<ushort, byte[]>(), total);

        var (chunks, _) = _frameBuffer[frameId];
        chunks[chunkId] = payload;

        if (chunks.Count < total) return;

        // 모든 청크 수신 완료 → 재조합
        var jpeg = new System.IO.MemoryStream();
        for (ushort i = 0; i < total; i++)
        {
            if (!chunks.ContainsKey(i)) return; // 누락 청크 있으면 폐기
            jpeg.Write(chunks[i], 0, chunks[i].Length);
        }

*/
        var frame = _frameBuffer[frameId];
        frame.chunks[chunkId] = payload;


        if (frame.chunks.Count < total) return;

        // 모든 청크 수신 완료 → 재조합
        var jpeg = new System.IO.MemoryStream();
        for (ushort i = 0; i < total; i++)
        {
            if (!frame.chunks.ContainsKey(i)) return; // 누락 청크 있으면 폐기
            jpeg.Write(frame.chunks[i], 0, frame.chunks[i].Length);
        }

        lock (_lock)
        {
            _pendingJpeg = jpeg.ToArray();
        }

        _frameBuffer.Remove(frameId);

        // 오래된 프레임 버퍼 정리
        if (_frameBuffer.Count > 10)
            _frameBuffer.Clear();
    }

    void OnDestroy()
    {
        _running = false;
        _udpClient?.Close();
        _receiveThread?.Join(500);
    }
}