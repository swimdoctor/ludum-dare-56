using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

    [SerializeField] private UIDocument titleScreen;

    [SerializeField] private string openingLevelName;

    private VisualElement root;

    private Button playButton;

    private Button quitButton;

    private void Start()
    {
        //Get our root visual element
        root = GetComponent<UIDocument>().rootVisualElement;

        //From the root, query for our buttons
        playButton = root.Q<Button>("play-button");
        quitButton = root.Q<Button>("quit-button");


        playButton.clicked += PlayButtonClicked;
        quitButton.clicked += QuitButtonClicked;
    }

    private void OnDestroy()
    {
        playButton.clicked -= PlayButtonClicked;
        quitButton.clicked -= QuitButtonClicked;
    }

    private void PlayButtonClicked()
    {
        Debug.Log("Opening level");
        SceneManager.LoadScene(openingLevelName);
    }

    private void QuitButtonClicked()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

}
