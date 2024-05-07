using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MainMenuManager : Singleton<MainMenuManager>
{
   [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }
   private LobbyManager LobbyManager = new LobbyManager();
   [SerializeField] private AgentSelectionPopup agentSelectionPopup;
   [SerializeField] private AgentShowroom _agentShowroom;
   [SerializeField] private PlayerPreferences _playerPreferences;
   [SerializeField] private CharacterCustomization _agentCustomization;
   [SerializeField] private LoadingManager _loadingManager;
   public void JoinLobby(SessionInfo sessionInfo)
   {
      LobbyManager.JoinLobby(sessionInfo);
      _loadingManager.OnJoinLobby();
   }

   public void CreateLobby(LobbyInfo lobbyInfo)
   {
      LobbyManager.CreateLobby(lobbyInfo);
      _loadingManager.OnJoinLobby();
   } 
   public void FindOpenSessions()
   {
      NetworkRunnerController.OnJoinLobby();
   }

   public void InitializeListenersForAgentSelection()
   {
      agentSelectionPopup._selectedAgentID.OnValueChanged.AddListener(_agentShowroom.OnAgentSelected);
      agentSelectionPopup._selectedAgentID.OnValueChanged.AddListener(_playerPreferences.SaveAgentPreference);
   }

   public void ListedLobbies(List<SessionInfo> sessionList)
   {
      LobbyManager.AddSessions(sessionList);
   }
}
