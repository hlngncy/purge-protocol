using System.Collections.Generic;
using System.IO;
using Fusion;
using UnityEngine;
using Random = System.Random;

//TODO create a system that creates unique id for lobbyid.
public class LobbyManager 
{
    private int _lobbyID;
    private static Random random = new Random();
    private static HashSet<int> usedIDs = new HashSet<int>();
    
    public static int GenerateUniqueID()
    {
        int uniqueNumber;
        
        do
        {
            uniqueNumber = random.Next(100000, 1000000); // Generates a 6-digit number
        } while (usedIDs.Contains(uniqueNumber));
        
        usedIDs.Add(uniqueNumber);
        return uniqueNumber;
    }

    public void CreateLobby(LobbyInfo lobbyInfo)
    {
        _lobbyID = GenerateUniqueID();
        MainMenuManager.Instance.NetworkRunnerController.CreateGame(_lobbyID.ToString(),"Game 1", (int)lobbyInfo.difficulty, lobbyInfo.lobbyName);
    }
    

    public void JoinLobby(SessionInfo sessionInfo)
    {
        MainMenuManager.Instance.NetworkRunnerController.JoinGame(sessionInfo);
    }

    public void RemoveLobby(int sessionID)
    {
        usedIDs.Remove(sessionID);
    }

    public void AddSessions(List<SessionInfo> sessionList)
    {
        if (sessionList.Count == 0) return;
        usedIDs.Clear();
        foreach (SessionInfo sessionInfo in sessionList)
        {
            AddLobby(sessionInfo.Name);
        }
    }
    
    public bool SearchSession(string sessionID)
    {
        return usedIDs.Contains(int.Parse(sessionID));
    }
    
    public void AddLobby(string sessionID)
    {
        usedIDs.Add(int.Parse(sessionID));
    }
}
