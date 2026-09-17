using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMainUiManagerV0 : MonoBehaviour
{
    DevLogV1 _devLog;

    public IEnumerator Initialize()
    {
        _devLog = GetComponent<DevLogV1>();
        _devLog.Initialize();
        yield return null;
    }
}
