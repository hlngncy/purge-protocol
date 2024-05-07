using System;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LobbyWaitCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _lobbyDifficulty;
    [SerializeField] private GameObject _startButton;
    private int _diffID;

    public void Setup(SessionInfo sessionInfo, bool isHost)
    {
        _lobbyName.text = sessionInfo.Properties["sessionTitle"].PropertyValue.ToString();
        _diffID = Convert.ToInt16(sessionInfo.Properties["difficulty"].PropertyValue);
         SetDifficultyText();
        _startButton.SetActive(isHost);
    }

    private void SetDifficultyText()
    {
        switch (_diffID)
        {
            case 0:
                _lobbyDifficulty.text = "Easy";
                break;
            case 1:
                _lobbyDifficulty.text = "Normal";
                break;
            case 2:
                _lobbyDifficulty.text = "Hard";
                break;
            case 3:
                _lobbyDifficulty.text = "Expert";
                break;
            default:
                throw new UnityException("ANASINI SİKTİN, AMMMMCIKLADIN BRE AQ");
        }
         
    }
}
