using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialControllerScript : MonoBehaviour
{
    public static int index; // controls what page the tutorial is on - range in [0, --]
    public int max_index; // largest page number; number of pages - 1

    public TMP_Text TitleText;
    public TMP_Text DescriptionText;

    public string title_0;
    [TextArea]
    public string description_0;

    public string title_1;
    [TextArea]
    public string description_1;

    public string title_2;
    [TextArea]
    public string description_2;

    public string title_3;
    [TextArea]
    public string description_3;

    public string title_adv;
    [TextArea]
    public string description_adv;

    private List<string> title_list;
    private List<string> description_list;

    // Start is called before the first frame update
    void Start()
    {
        title_list = new List<string>();
        title_list.Add(title_0);
        title_list.Add(title_1);
        title_list.Add(title_2);
        title_list.Add(title_3);
        title_list.Add(title_adv);

        description_list = new List<string>();
        description_list.Add(description_0);
        description_list.Add(description_1);
        description_list.Add(description_2);
        description_list.Add(description_3);
        description_list.Add(description_adv);

        TitleText.text = title_list[index];
        DescriptionText.text = description_list[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLeftButton()
    {
        if (index > 0)
        {
            index--;
        }

        // TODO: display correct text/images for the current page index
        TitleText.text = title_list[index];
        DescriptionText.text = description_list[index];

    }

    public void OnRightButton()
    {
        if (index < max_index)
        {
            index++;
        }

        // TODO: display correct text/images for the current page index
        TitleText.text = title_list[index];
        DescriptionText.text = description_list[index];

    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
