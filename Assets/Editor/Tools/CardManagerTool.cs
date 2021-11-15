using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardManagerTool : EditorWindow
{
    int pixelSpace = 5;

    static CardManager cardManager;
    [MenuItem("Tools/Card Mananger Tool")]
    public static void ShowWindow() {
        cardManager = FindObjectOfType<CardManager>();
        GetWindow<CardManagerTool>("Card Manager Tool");
    }

    bool show = true;
    bool modify = false;

    public void OnGUI() {
        if (Application.isPlaying) {
            GUILayout.Label("DON'T WORK IN PLAY MODE !", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label("Card Number", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if(!show && GUILayout.Button("Show", GUILayout.Width(100))) {
            show = true;
        }
        else if(show) {
            if (GUILayout.Button("Hide", GUILayout.Width(100))) {
                show = false;
                modify = false;
            }
            if (modify && GUILayout.Button("Lock", GUILayout.Width(100))) {
                modify = false;
            }
            else if (!modify && GUILayout.Button("Modify", GUILayout.Width(100))) {
                modify = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (show) {
            if (modify) {
                for (int i = 0; i < cardManager.cardsNumber.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    cardManager.cardsNumber[i] = EditorGUILayout.IntField(cardManager.cardsNumber[i], GUILayout.Width(50));
                    GUILayout.Space(pixelSpace);
                    GUILayout.Label(((CardManager.CardType)i).ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            else {
                for (int i = 0; i < cardManager.cardsNumber.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(cardManager.cardsNumber[i].ToString(), GUILayout.Width(50));
                    GUILayout.Space(pixelSpace);
                    GUILayout.Label(((CardManager.CardType)i).ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        
    }
}
