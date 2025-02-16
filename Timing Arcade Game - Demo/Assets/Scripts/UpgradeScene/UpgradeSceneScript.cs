using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeSceneScript : MonoBehaviour
{

    public TMP_Text DescriptionTitle;
    public TMP_Text DescriptionText;

    public Button Attack_1;
    public Button Attack_2;

    // Start is called before the first frame update
    void Start()
    {
        UpgradeButtonScript Attack_1_script = Attack_1.GetComponent<UpgradeButtonScript>();
        Attack_1.onClick.AddListener( delegate{ UpdateDescription(Attack_1_script.button_name, Attack_1_script.description); });
        UpgradeButtonScript Attack_2_script = Attack_2.GetComponent<UpgradeButtonScript>();
        Attack_2.onClick.AddListener(delegate { UpdateDescription(Attack_2_script.button_name, Attack_2_script.description); });
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: create description which appears when upgrade is clicked
            // - disappears when clicked again
            // - disappears when another button is clicked
        // TODO: create description + buy button for each upgrade
        // TODO: lock further upgrades until previous one has been bought
        // TODO: costs coins to buy an upgrade - can't buy if not enough coins
        // TODO: buyable icons look different from non-buyable ones
            // note: try just turning off interactability
        // TODO: bought ones look different from locked and unbought ones
    }

    public void UpdateDescription(string name, string description)
    {
        if (name == null)
        {
            Debug.LogError("UpdateDescription is called with a null button name");
        }
        else if (DescriptionTitle.text.Equals(name))
        {
            DescriptionTitle.GetComponent<TextMeshProUGUI>().enabled = false;
            DescriptionText.GetComponent<TextMeshProUGUI>().enabled = false;
            DescriptionTitle.text = "";
        }
        else
        {
            DescriptionTitle.GetComponent<TextMeshProUGUI>().enabled = true;
            DescriptionText.GetComponent<TextMeshProUGUI>().enabled = true;
            DescriptionTitle.text = name;
            DescriptionText.text = description;
        }
    }

    // Switches scene to battle scene- attached to Retry button
    public void LoadBattleScene()
    {
        SceneManager.LoadScene("Battle");
    }

    // Switches scene to main menu- attached to Main Menu button
    public void LoadMainMenuScene()
    {

        SceneManager.LoadScene("MainMenu");
    }

    /*
    private void OnDestroy()
    {
        // TODO: remove all listeners from all buttons
    }
    */
}
