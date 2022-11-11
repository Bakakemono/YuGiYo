using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedDeckManager {
    static string resourceDir = "resources";
    static string savedDatasDir = "SavedDatas";
    static string decksDir = "Decks";

    public static string defaultDeckName = "DefaultCardDeck";

    static string gameDir = "Mythology";

    static string jsonExtension = ".json";
    static string metaExtansion = ".meta";

    public static int[] LoadDeck(string _saveFileName) {
        string saveFilePath = 
            System.IO.Path.Combine(
                Application.dataPath,
                resourceDir,
                savedDatasDir,
                decksDir,
                _saveFileName + jsonExtension
                );

        string jsonCardNumber;
        if(!System.IO.File.Exists(saveFilePath)) {
            Debug.LogError("Card save file doesn't exist");
            return new int[0];
        }
        jsonCardNumber = System.IO.File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<CardsNumber>(jsonCardNumber).cardsNumber;
    }

    public static void SaveDeck(string _saveFileName, int[] _cardNumbers) {
        string saveFilePath = 
            System.IO.Path.Combine(
                Application.dataPath,
                resourceDir,
                savedDatasDir,
                decksDir,
                _saveFileName + jsonExtension
                );

        if(!System.IO.File.Exists(saveFilePath)) {
            System.IO.FileStream fileStream = System.IO.File.Create(saveFilePath);
            fileStream.Close();
        }

        string persistentDeckPath = System.IO.Path.Combine(
            Application.persistentDataPath,
            gameDir,
            decksDir,
            _saveFileName + jsonExtension
            );

        if(!System.IO.File.Exists(saveFilePath)) {
            System.IO.FileStream fileStream = System.IO.File.Create(saveFilePath);
            fileStream.Close();
        }

        CardsNumber tmpCardsNumber;
        tmpCardsNumber.cardsNumber = _cardNumbers;
        string jsonCardNumber = JsonUtility.ToJson(tmpCardsNumber);

        System.IO.File.WriteAllText(saveFilePath, jsonCardNumber);
    }

    public static void DeleteDeck(string _deckName) {
        string deckPath = 
            System.IO.Path.Combine(
                Application.dataPath,
                resourceDir,
                savedDatasDir,
                decksDir,
                _deckName + jsonExtension
                );

        if(_deckName == defaultDeckName || !System.IO.File.Exists(deckPath)) {
            return;
        }

        System.IO.File.Delete(deckPath);
        System.IO.File.Delete(deckPath + metaExtansion);
    }

    public static string[] GetAllDecksNameSaved() {
        string[] nameWithExtension = 
            System.IO.Directory.GetFiles(
                System.IO.Path.Combine(
                    Application.dataPath,
                    resourceDir,
                    savedDatasDir,
                    decksDir
                    )
                );
        List<string> nameWithoutExtention = new List<string>();

        for(int i = 0; i < nameWithExtension.Length; i++) {
            if(nameWithExtension[i].Contains(".meta")) {
                continue;
            }
            nameWithoutExtention.Add(System.IO.Path.GetFileNameWithoutExtension(nameWithExtension[i]));
        }

        return nameWithoutExtention.ToArray();
    }


    public static bool DoesDeckExist(string _deckName) {
        string deckPath =
            System.IO.Path.Combine(
                Application.dataPath,
                resourceDir,
                savedDatasDir,
                decksDir,
                _deckName + jsonExtension
                );

        return System.IO.File.Exists(deckPath);
    }
    



    // Create a directory at the given path if it doesn't already exist.
    public static void CreateAbsolutePathDirectory() {
        string[] paths = {
            Application.persistentDataPath,
            gameDir,
            decksDir
        };

        for(int i = 0; i < paths.Length; i++) {
            string dirPath = "";
            for(int j = 0; j < i; j++) {
                dirPath = System.IO.Path.Combine(dirPath, paths[j]);
            }
            dirPath = System.IO.Path.Combine(dirPath, paths[i]);
            if(!System.IO.Directory.Exists(dirPath)) {
                System.IO.Directory.CreateDirectory(dirPath);
            }
        }
    }
}
