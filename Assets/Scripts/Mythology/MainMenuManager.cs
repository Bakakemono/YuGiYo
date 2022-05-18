using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] GameObject main;
    [SerializeField] GameObject create;
    [SerializeField] GameObject join;

    [SerializeField] TMP_InputField text;
    [SerializeField] string roomName;

    // Display main menu
    public void GoBack() {
        create.SetActive(false);
        join.SetActive(false);
        main.SetActive(true);
    }

    // Show room creation options
    public void CreateRoom() {
        main.SetActive(false);
        create.SetActive(true);
    }

    // Show room joing options
    public void JoinRoom() {
        main.SetActive(false);
        join.SetActive(true);
    }

    // Stop the game
    public void ExitApp() {
        Application.Quit();
    }

    // Register room name upon room creation or during joing a specific room
    public void RegisterRoomName() {
        roomName = text.text;
    }
}
