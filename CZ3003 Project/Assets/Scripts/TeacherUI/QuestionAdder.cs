using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionAdder : MonoBehaviour
{
    public static QuestionAdder instance;
    public GameObject AddQuestionPanel;
    public GameObject OptionSelectionPanel;
    public Dropdown WorldSelection;
    public static int World;
    public Dropdown DifficultySelect;
    public static string Difficulty;
    public Dropdown SectionSelect;
    public static int Section;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void WorldSelect()
    {
        Debug.Log(WorldSelection.options[WorldSelection.value].text);
        switch (WorldSelection.options[WorldSelection.value].text)
        {
            case "OODP":
                World = 1;
                break;
            case "SE":
                World = 2;
                break;
            case "SSAD":
                World = 3;
                break;
            default:
                Debug.Log("Error Occured");
                break;
        }
        Debug.Log(World);
    }
    public void DifficultySelection()
    {
        Difficulty = DifficultySelect.options[DifficultySelect.value].text;
        Debug.Log(Difficulty);
    }
    public void SectionSelection()
    {
        Section = int.Parse(SectionSelect.options[SectionSelect.value].text);
        Debug.Log(Section);
    }

    public void PressNextButton()
    {
        AddQuestionPanel.gameObject.SetActive(true);
        OptionSelectionPanel.gameObject.SetActive(false);
    }
    public void PressBackButton()
    {
        AddQuestionPanel.gameObject.SetActive(false);
        OptionSelectionPanel.gameObject.SetActive(true);
    }
}