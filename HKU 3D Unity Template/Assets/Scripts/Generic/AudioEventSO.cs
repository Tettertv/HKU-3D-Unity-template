using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Event")]
public class AudioEventSO : ScriptableObject
{
    public EventReference eventReference;

    [Header("Bank Loading")]
    public string bankName;
}
