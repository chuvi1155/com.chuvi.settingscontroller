using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLSystem.Settings;

public class SettingsController : MonoBehaviour

#if CHUVI_EXTENSIONS
    , ISettingsController
{
    public SettingsValue settingsValuePref;
    public RectTransform parentContainer;
    public GameObject Background;
    ISettings settings;
    public ISettings Settings
    {
        get => settings;
        set
        {
            settings = value;
            if (settings == null)
            {
                Clear();
            }
        }
    }

    public void InitUniversal(ISettings settings)
    {
        if (parentContainer.childCount > 0)
        {
            return;
        }
        else
        {
            foreach (var item in settings.GetData())
            {
                SettingsValue sval = Instantiate(settingsValuePref, parentContainer);
                sval.Data = item;
            }
        }
    }


    /// <summary>
    /// Если поле settings равен null
    /// </summary>
    public void Clear()
    {
        while (parentContainer.childCount > 0)
        {
            DestroyImmediate(parentContainer.GetChild(0).gameObject);
        }
    }

    public void Save()
    {
        settings?.Save();
        Debug.Log(settings == null ? "Settings is null" : "Settings is not null");
        Background.SetActive(false);
        Clear();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.V))
        {
            if (settings is UserINISetting)
            {
                InitUniversal(settings);
                Background.SetActive(true);
            }
        }
#if XMLSYSTEM
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
        {
            if (settings is UserXMLSettings)
            {
                InitUniversal(settings);
                Background.SetActive(true);
            }
        }
#endif
    }
#else
{ 
#endif
}
