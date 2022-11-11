using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardManagerTool : EditorWindow
{
    int pixelSpace = 5;

    // Deck currently viewed.
    int[] currentDeck;

    // Deck copied and ready to relpace another.
    int[] copiedDeck;
    
    // Display deck information.
    bool showDeck = false;

    // is the deck currently copied ?
    bool isDeckCopied = false;

    // Can Deck Be modified.
    bool modifyDeck = false;

    string currentSaveFileName = "";
    string[] saveFileNames;
    bool saveFileNamesFetched = false;

    string newSaveFileName = "";
    [MenuItem("Tools/Card Mananger Tool")]
    public static void ShowWindow() {
        GetWindow<CardManagerTool>("Card Manager Tool");

        string[] deckSavedNames = SavedDeckManager.GetAllDecksNameSaved();
        SavedDeckManager.CreateAbsolutePathDirectory();
    }

    public void OnGUI() {
        if(!saveFileNamesFetched) {
            saveFileNames = SavedDeckManager.GetAllDecksNameSaved();

            bool defaultDeckNameFound = false;
            for(int i = 0; i < saveFileNames.Length - 1; i++) {
                if(!defaultDeckNameFound && saveFileNames[i] != SavedDeckManager.defaultDeckName) {
                    continue;
                }
                else if(!defaultDeckNameFound && saveFileNames[i] == SavedDeckManager.defaultDeckName) {
                    defaultDeckNameFound = true;
                    saveFileNames[i] = saveFileNames[i + 1];
                }
                else {
                    saveFileNames[i] = saveFileNames[i + 1];
                }
            }
            saveFileNames[^1] = SavedDeckManager.defaultDeckName;


            saveFileNamesFetched = true;
        }

        for(int i = 0; i < saveFileNames.Length - 1; i++) {
            if(GUILayout.Button(saveFileNames[i], GUILayout.Width(300))) {
                showDeck = true;
                currentDeck = SavedDeckManager.LoadDeck(saveFileNames[i]);
                currentSaveFileName = saveFileNames[i];

                modifyDeck = false;
            }
        }
        GUILayout.Label("—————————");
        if(GUILayout.Button(saveFileNames[^1], GUILayout.Width(300))) {
            showDeck = true;
            currentDeck = SavedDeckManager.LoadDeck(saveFileNames[^1]);
            currentSaveFileName = saveFileNames[^1];
            modifyDeck = false;
        }
        GUILayout.Label("—————————");

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("New Deck", GUILayout.Width(100))) {
            if(newSaveFileName.Length != 0 && !SavedDeckManager.DoesDeckExist(newSaveFileName)) {
                int[] newDeck = SavedDeckManager.LoadDeck(SavedDeckManager.defaultDeckName);
                SavedDeckManager.SaveDeck(newSaveFileName, newDeck);
                saveFileNamesFetched = false;

                showDeck = true;
                currentDeck = SavedDeckManager.LoadDeck(newSaveFileName);
                currentSaveFileName = newSaveFileName;

                newSaveFileName = "";
            }
        }
        GUILayout.Space(pixelSpace);
        newSaveFileName = EditorGUILayout.TextField(newSaveFileName, GUILayout.Width(192));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if(showDeck) {
            if(GUILayout.Button("Copy", GUILayout.Width(100))) {
                copiedDeck = currentDeck;
                isDeckCopied = true;
            }
            if(isDeckCopied && GUILayout.Button("Past", GUILayout.Width(100))) {
                currentDeck = copiedDeck;
            }
        }
        EditorGUILayout.EndHorizontal();

        if(!showDeck) {
            GUILayout.Label("Deck Selected : NONE", EditorStyles.boldLabel);
        }
        else if(showDeck) {
            GUILayout.Label("Deck : " + currentSaveFileName, EditorStyles.boldLabel);
        }

        if(showDeck) {
            if(modifyDeck) {
                for(int i = 0; i < currentDeck.Length; i++) {
                    EditorGUILayout.BeginHorizontal();
                    currentDeck[i] = EditorGUILayout.IntField(currentDeck[i], GUILayout.Width(50));
                    GUILayout.Space(pixelSpace);
                    GUILayout.Label(((CardManager.CardType)i).ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            else {
                for(int i = 0; i < currentDeck.Length; i++) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(currentDeck[i].ToString(), GUILayout.Width(50));
                    GUILayout.Space(pixelSpace);
                    GUILayout.Label(((CardManager.CardType)i).ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        if(showDeck) {
            EditorGUILayout.BeginHorizontal();
            if(modifyDeck && GUILayout.Button("Save", GUILayout.Width(100))) {
                SavedDeckManager.SaveDeck(currentSaveFileName, currentDeck);
                modifyDeck = false;
            }
            else if(!modifyDeck && GUILayout.Button("Modify", GUILayout.Width(100))) {
                modifyDeck = true;
            }
            if(modifyDeck && GUILayout.Button("Undo", GUILayout.Width(100))) {
                currentDeck = SavedDeckManager.LoadDeck(currentSaveFileName);
                modifyDeck = false;
            }
            if(!modifyDeck && GUILayout.Button("Delete", GUILayout.Width(100))) {
                SavedDeckManager.DeleteDeck(currentSaveFileName);
                showDeck = false;
                saveFileNamesFetched = false;
                currentDeck = new int[0];
                currentSaveFileName = "";
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
