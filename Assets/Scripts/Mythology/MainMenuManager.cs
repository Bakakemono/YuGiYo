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

    public void GoBack() {
        create.SetActive(false);
        join.SetActive(false);
        main.SetActive(true);
    }

    public void CreateRoom() {
        main.SetActive(false);
        create.SetActive(true);
    }

    public void JoinRoom() {
        main.SetActive(false);
        join.SetActive(true);
    }

    public void ExitApp() {
        Application.Quit();
    }

    public void RegisterRoomName() {
        roomName = text.text;
    }
}
