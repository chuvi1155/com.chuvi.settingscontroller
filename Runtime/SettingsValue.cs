using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyNameText;

    //[SerializeField] GameObject InputFfour;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Toggle valueToggle;
    [SerializeField] TMP_InputField valueInputField;
#if CHUVI_EXTENSIONS
    ISettingsData data;
    System.Type type = null;
    public ISettingsData Data
    {
        get => data;
        set
        {
            data = value;
            Init();
        }
    }
    private void Start()
    {
        dropdown.onValueChanged.AddListener((val) =>
        {
            object res = System.Enum.Parse(type, dropdown.options[val].text);
            Data.SetData(res);
        });
        valueToggle.onValueChanged.AddListener((val) =>
        {
            Data.SetData(val);
        });
        valueInputField.onValueChanged.AddListener((text) =>
        {
            Data.SetData(text);
        });
    }
    void Init()
    {
        keyNameText.text = data.Key;
        type = data.GetDataType();

        if (type == typeof(bool))
        {
            valueInputField.gameObject.SetActive(false);
            valueToggle.gameObject.SetActive(true);
            dropdown.gameObject.SetActive(false);

            valueToggle.SetIsOnWithoutNotify(data.GetData<bool>());
        }
        else if (type == typeof(string))
        {
            valueInputField.gameObject.SetActive(true);
            valueToggle.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(false);

            valueInputField.SetTextWithoutNotify(data.GetData<string>());
        }
        else if (type.IsEnum)
        {
            valueInputField.gameObject.SetActive(false);
            valueToggle.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(true);

            string[] names = System.Enum.GetNames(type);
            var options = dropdown.options;
            options.Clear();
            foreach (var name in names)
            {
                options.Add(new TMP_Dropdown.OptionData(name));
            }
            dropdown.SetValueWithoutNotify(System.Array.FindIndex(names, name => name == data.GetData<string>()));
        }
        else
        {
            valueInputField.gameObject.SetActive(true);
            valueToggle.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(false);

            valueInputField.SetTextWithoutNotify(data.GetData<string>());
        }
    } 
#endif
}
