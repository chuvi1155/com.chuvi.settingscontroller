using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XMLSystem.Settings;

public class SettingsController : MonoBehaviour

#if CHUVI_EXTENSIONS
    , ISettingsController
{
    static List<RectTransform> groups = new List<RectTransform>();
    public SettingsValue settingsValuePref;
    public RectTransform parentContainer;
    public GameObject Background;
    [SerializeField] Sprite groupSprite;
    [SerializeField] Button closeBtn;
    [SerializeField] Button saveBtn;


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

    public void Init(ISettings settings)
    {
        Debug.Log("Init settings viewer");
        StopAllCoroutines();
        StartCoroutine(_Init(settings));
    }

    IEnumerator _Init(ISettings settings)
    {
        closeBtn.interactable = false;
        saveBtn.interactable = false;
        if (parentContainer.childCount > 0)
        {
            closeBtn.interactable = true;
            saveBtn.interactable = true;
            yield break;
        }
        else
        {
            foreach (var data in settings.GetData())
            {
                if (data.Group.StartsWith("ENUM:")) continue;
                RectTransform container = null;
                if (!groups.Exists(g => g.name == data.Group))
                {
                    container = CreateGroup(data.Group, parentContainer);
                    groups.Add(container);
                }
                else
                {
                    container = groups.Find(g => g.name == data.Group);
                }
                SettingsValue sval = Instantiate(settingsValuePref, container);
                sval.Data = data;
                //sval.gameObject.SetActive(false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(container);
                yield return data;
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentContainer);
            }
            yield return new WaitForFixedUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentContainer);
        }
        closeBtn.interactable = true;
        saveBtn.interactable = true;
    }

    RectTransform CreateGroup(string groupName, Transform parent)
    {
        var go = new GameObject (groupName);
        go.transform.SetParent(parent);
        go.transform.localScale = Vector3.one;
        var img = go.AddComponent<Image>();
        img.sprite = groupSprite;
        img.type = Image.Type.Sliced;
        var vert_layout = go.AddComponent<VerticalLayoutGroup>();
        vert_layout.padding = new RectOffset(5, 5, 5, 5);
        vert_layout.childControlWidth = true;
        vert_layout.childControlHeight = false;
        vert_layout.spacing = 5;

        var sizeFitter = go.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        GameObject SettingsTitle = Instantiate(Resources.Load<GameObject>("SettingTitle"), go.transform);
        SettingsTitle.transform.localScale = Vector3.one;
        SettingsTitle.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = groupName.ToUpper();
        var tg = SettingsTitle.GetComponentInChildren<Toggle>();

        tg.onValueChanged.AddListener((val) =>
        {
            var collapsedIcon = tg.transform.Find("Background/Checkmark_Collapsed").gameObject;
            collapsedIcon.SetActive(!val);
            for (int i = 1; i < SettingsTitle.transform.parent.childCount; i++)
            {
                SettingsTitle.transform.parent.GetChild(i).gameObject.SetActive(val);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(SettingsTitle.transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(SettingsTitle.transform.parent.parent as RectTransform);
        });

        return go.transform as RectTransform;
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
        groups.Clear();
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
                Init(settings);
                Background.SetActive(true);
            }
        }
#if XMLSYSTEM
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
        {
            if (settings is UserXMLSettings)
            {
                Init(settings);
                Background.SetActive(true);
            }
        }
#endif
    }
#else
{ 
#endif
}
