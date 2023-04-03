using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LocalisationController : MonoBehaviour
{
    public List<LanguagePack> languagesList;

    public Text mainMenuPlayButton,mainMenuSettingsButton,mainMenuAboutButton;

    [Space]

    public Text reloadSceneButtonText,exitToMainMenuText,menuButtonText,hideButtonText,shopButtonText;

    [Header("About game panel")]

    public Text AuthorsText,originalAuthorText,digitalDeveloperText,exitButtonText;


    public void Awake(){
        if(SceneManager.GetActiveScene().name == "GameScene"){
            InitGameSceneUILocalisation();
        }
        if(SceneManager.GetActiveScene().name == "MainMenu"){
            InitMainMenuLocalisation();            
        }
    }    

    private void InitGameSceneUILocalisation(){
        if(Application.systemLanguage==SystemLanguage.English){
            reloadSceneButtonText.text = languagesList[0].getValueForButton("GAME_MENU_RELOAD_SCENE_BUTTON");
            exitToMainMenuText.text = languagesList[0].getValueForButton("GAME_MENU_EXIT_BUTTON");
            menuButtonText.text = languagesList[0].getValueForButton("GAME_MENU_MENU_BUTTON");
            hideButtonText.text = languagesList[0].getValueForButton("GAME_MENU_HIDE_SHOW_BUTTON");
            shopButtonText.text = languagesList[0].getValueForButton("GAME_MENU_SHOP_BUTTON");
        }
        if(Application.systemLanguage==SystemLanguage.Russian){
            reloadSceneButtonText.text = languagesList[1].getValueForButton("GAME_MENU_RELOAD_SCENE_BUTTON");
            exitToMainMenuText.text = languagesList[1].getValueForButton("GAME_MENU_EXIT_BUTTON");
            menuButtonText.text = languagesList[1].getValueForButton("GAME_MENU_MENU_BUTTON");
            hideButtonText.text = languagesList[1].getValueForButton("GAME_MENU_HIDE_SHOW_BUTTON");
            shopButtonText.text = languagesList[1].getValueForButton("GAME_MENU_SHOP_BUTTON");
        }

    }

    private void InitMainMenuLocalisation(){
        int currentLocalisationIndex=0;
        if(Application.systemLanguage==SystemLanguage.English){
            currentLocalisationIndex = 0;
        }
        if(Application.systemLanguage==SystemLanguage.Russian){
            currentLocalisationIndex  = 1;
        }
        mainMenuPlayButton.text = languagesList[currentLocalisationIndex].getValueForButton("MAIN_MENU_PLAY_BUTTON");
        mainMenuSettingsButton.text = languagesList[currentLocalisationIndex].getValueForButton("MAIN_MENU_SETTINGS_BUTTON");
        mainMenuAboutButton.text = languagesList[currentLocalisationIndex].getValueForButton("MAIN_MENU_ABOUT_BUTTON");

        AuthorsText.text = languagesList[currentLocalisationIndex].getValueForButton("ABOUT_PANEL_AUTHORS_TITLE");
        originalAuthorText.text = languagesList[currentLocalisationIndex].getValueForButton("ABOUT_PANEL_ORIGINAL_AUTHOR");
        digitalDeveloperText.text = languagesList[currentLocalisationIndex].getValueForButton("ABOUT_PANEL_DIGITAL_DEVELOPER");
        exitButtonText.text = languagesList[currentLocalisationIndex].getValueForButton("ABOUT_PANEL_BACK_BUTTON_TEXT");
    }
}
