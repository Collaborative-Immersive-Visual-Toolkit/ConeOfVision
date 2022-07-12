using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{

    [SerializeField]
    private string _gameversion = "0.0.1";

    public string Gameversion { get { return _gameversion; } }

    [SerializeField]

    private string _nickname = "YourNameHere";

    public string Nickname {
        get {

            int value = UnityEngine.Random.Range(0, 9999);

            return _nickname + value.ToString();

        }

        set { _nickname = value; }
    }


    [SerializeField]

    private string _userID = "0";

    public string UserID {
        get {
            return _userID;
        }
        set {

            if (value == "Partecipant_0") _userID = "0";
            else if (value == "Partecipant_1") _userID = "2671308206268206";
            else if (value == "Partecipant_2") _userID = "2911531572263440";

        }
    }


    [SerializeField]
    private string _roomName = "CollaborationRoom";

    public string RoomName {
        get { return _roomName; }
        set { _roomName = value; }
    }


    [SerializeField]
    public byte InstantiateVrAvatarEventCode = 1;

    [SerializeField]
    public byte InstantiateObserverEventCode = 2;

    [SerializeField]
    public byte NextDataDisplay = 3;

    [SerializeField]
    public byte AddLongTap = 10;

    [SerializeField]
    public byte LaserPointerChange = 11;

    [SerializeField]
    public byte Reorient = 31;

    [SerializeField]
    public byte VisualConeChange = 12;

    [SerializeField]
    public byte StartRecordAudio = 36;

    [SerializeField]
    public byte StopRecordAudio = 37;

    [SerializeField]
    public byte EndOfTrialEvent = 15;

    [SerializeField]
    public byte HideUnhideLayer = 4;

    [SerializeField]
    public byte SpawnPlaceholder = 5;

    [SerializeField]
    public byte DeletePlaceHolders = 6;

    [SerializeField]
    public byte VisualizationChange = 33;

    [SerializeField]
    public byte UiHelperSwitch = 34;

    [SerializeField]
    public byte ConeOn = 38;

    [SerializeField]
    public byte OwnConeOn = 39;

    [SerializeField]
    public byte ConeOff = 40;

    [SerializeField]
    public bool _observer = true;

    [SerializeField]
    public bool _devmode = true;

    //todocondition

  
    public bool Observer
    {
        get {


#if UNITY_EDITOR

            return _observer;

#elif UNITY_ANDROID

        return false;

#else
               
        return _observer; 
#endif



        }
        set {

            _observer = value;



        }
    }

    public string _dataFolder = "Data";

    [SerializeField]
    public string DataFolder {

        get {

                if (_dataFolder == "Data")
                {
                    
                    _dataFolder = Application.dataPath.Replace("/Assets", "/Data") + System.DateTime.Now.ToString("_MMM_ddd_HH_mm");
                }

                return _dataFolder;
            
            }

    }
}


public enum RoomType { 

    CollaborativeRoom =0 ,
    PersonalRoom =1
}

