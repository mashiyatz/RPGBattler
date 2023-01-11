using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleLog : MonoBehaviour
{
    public TextMeshProUGUI battleLog;

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // consider creating a new logger...
        if (type==LogType.Log) battleLog.text = logString;
    }
}
