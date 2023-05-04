using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Cinemachine;

#region ReadOnly Attribute
/*
    Thanks to It3ration for this argueably garbage solution used for making a public variable read only.
    https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
*/

public class ReadOnlyAttribute : PropertyAttribute {  }

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endregion

[RequireComponent(typeof (CinemachineVirtualCamera))]
public class ShakeManager : MonoBehaviour {
    
    [Header("Debug")]
    [SerializeField] bool logWarnings = true; 
    [ReadOnly] public float currentPlayingKey = -1f;

    Dictionary<int, float[]> shakePriorities = new Dictionary<int, float[]>();
    Coroutine currentPlayingShake = null;
    CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake() {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    IEnumerator playShake(float amplitude, float frequency, float duration, int key) {
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

        shakePriorities.Remove(key);
        currentPlayingKey = -1f;
        handleShakes();
    }

    void handleShakes() {
        if (shakePriorities.Count == 0) return;

        int largestKey = shakePriorities.Keys.Max();
        if (currentPlayingKey == largestKey) return;

        float _amplitude = shakePriorities[largestKey][0];
        float _frequency = shakePriorities[largestKey][1];
        float _timeOfCreation = shakePriorities[largestKey][2];
        float _duration = shakePriorities[largestKey][3];

        float _durationComparedToCreationTime = _duration - (Time.time - _timeOfCreation);
        if (Time.time - _timeOfCreation >= _duration) {
            /* Key is invalidated due to it being over its max duration. Reruns function to check for new screenshake. */
            shakePriorities.Remove(largestKey);
            handleShakes();
            return;
        };

        currentPlayingKey = largestKey;
        if (currentPlayingShake != null) StopCoroutine(currentPlayingShake);
        currentPlayingShake = StartCoroutine(playShake(_amplitude, _frequency, _durationComparedToCreationTime, largestKey));
    }

    public void addShakeWithPriority(float amplitude, float frequency, float duration, int priority) {
        if (priority < 0) {
            Debug.LogError("Failed at creating shake with priority. Priority must be a positive integer.");
            return;
        }

        if (shakePriorities.ContainsKey(priority))
        {
            if (logWarnings) Debug.LogWarning("Shake created with duplicate priority. This is often not good and will result in unexpected overriding.\nUncheck *Log Warnings* in the inspector of this script to disable the warning.");

            shakePriorities.Remove(priority);
            currentPlayingKey = -1f;
            if (currentPlayingKey == priority)
                StopCoroutine(currentPlayingShake);
        }
        
        shakePriorities.Add(priority, new float[] { amplitude, frequency, Time.time, duration });
        handleShakes();
    }
}