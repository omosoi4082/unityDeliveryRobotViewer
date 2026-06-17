
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MQTT/Topic Config")]
public class MqttTopicConfig : ScriptableObject
{
    public List<string> topics;
}
