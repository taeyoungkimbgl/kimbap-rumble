using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevUiManagerV1 : Singleton<DevUiManagerV1>
{
    protected override bool IsDontDestroyGameObject => true;
    DevButtonManagerV1 _devButtonManager;
    DevLogV1 _devLog;
    DevStateLog _devStateLog;
    DevInputFieldManagerV1 _devInputFieldManager;
    DevDropdownManagerV1 _devDropdownManager;

    public RawImage rawImage;

    public GameObject[] Panels;
    public Slider slider;
    public enum PanelName { DebugLog = 0, StateLog = 1, InputFields = 2, Buttons = 3, Dropdowns = 4, }

    bool isActiveAllPanels;

    bool initialized;

    IEnumerator Start()
    {
        if (!initialized)
        {
            yield return StartCoroutine(Initialize());
        }
    }

    public IEnumerator Initialize()
    {
        if (initialized) yield break;

        _devButtonManager = GetComponent<DevButtonManagerV1>();
        _devLog = GetComponent<DevLogV1>();
        _devStateLog = GetComponent<DevStateLog>();
        _devInputFieldManager = GetComponent<DevInputFieldManagerV1>();
        _devDropdownManager = GetComponent<DevDropdownManagerV1>();

        yield return StartCoroutine(_devButtonManager.Initialize());


        _devButtonManager.AddListener(DevButtonTypeV1.SetActiveDebugLog, () => SetActiveLog(PanelName.DebugLog));
        _devButtonManager.AddListener(DevButtonTypeV1.SetActiveStateLog, () => SetActiveLog(PanelName.StateLog));
        _devButtonManager.AddListener(DevButtonTypeV1.SetActiveDevButtons, () => SetActiveLog(PanelName.Buttons));
        _devButtonManager.AddListener(DevButtonTypeV1.SetActiveInputFields, () => SetActiveLog(PanelName.InputFields));
        _devButtonManager.AddListener(DevButtonTypeV1.SetActiveDropdowns, () => SetActiveLog(PanelName.Dropdowns));

        _devButtonManager.m_DisplayButton.AddListener(SetActiveAllPanels);

        _devLog.Initialize();
        _devStateLog.Initialize();
        yield return StartCoroutine(_devInputFieldManager.Initialize());
        yield return StartCoroutine(_devDropdownManager.Initialize());

        initialized = true;
    }

    public void SetActiveAllPanels()
    {
        isActiveAllPanels = !isActiveAllPanels;
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(isActiveAllPanels);
        }
    }

    public void SetActiveLog(PanelName panelName)
    {
        var panel = Panels[(int)panelName];

        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }
    }

    public DevButtonManagerV1 GetDevButtonManager()
    {
        return _devButtonManager;
    }

    public DevLogV1 GetDevLog()
    {
        return _devLog;
    }

    public DevStateLog GetDevStateLog()
    {
        return _devStateLog;
    }

    public DevInputFieldManagerV1 GetDevInputFieldManager()
    {
        return _devInputFieldManager;
    }

    public DevDropdownManagerV1 GetDevDropdownManager()
    {
        return _devDropdownManager;
    }
}
