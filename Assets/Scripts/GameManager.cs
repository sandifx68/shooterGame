using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Din cate am inteles baiatu asta organizeaza playerii
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    void Awake ()
    {
        if(instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        } else
        {
            instance = this;
        }
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if(sceneCamera == null)
            return;
        sceneCamera.SetActive(isActive);
    }

    #region Player tracking
    
    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    //Dictionar cu Tkey= un string si Tvalue = player, adica tu ii bagi cv pe teava si dupa
    //asociaza acel ceva cu un alt lucru, ambele chestii sunt de tip Tkey
    
    public static void RegisterPlayer (string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID; // uite aici face numele la player
        //cred ca m-am prins                // sa fie ID-ul editat (cu prefixul)
        //cred doar
        //Deci in dictionar se pune totul legat de player intr-o varibila(clasa?)
        //chonky si daca dai remove la network id-ul player-ului asa dai remove 
        //si la player cred
    }
    public static void UnRegisterPlayer (string _playerID)
    {
        players.Remove(_playerID);
        //in consecinta da remove si la player in sine cred,
        //pe langa id-ul lui cu prefix
    }

    public static Player GetPlayer (string _playerID)
    {
        return players[_playerID];//Din dic. de playeri ia playerID
    }

    // void OnGUI()
    // {
    //     GUILayout.BeginArea(new Rect(200,200,200,500));
    //     GUILayout.BeginVertical();

    //     foreach(string _playerID in players.Keys)
    //     {
    //         GUILayout.Label(_playerID + " - " + players[_playerID].transform.name);
    //     }

    //     GUILayout.EndVertical();
    //     GUILayout.EndVertical();
    // }
    #endregion
}
