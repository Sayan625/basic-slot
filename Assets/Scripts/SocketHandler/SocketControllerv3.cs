using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
public class SocketControllerv3 : MonoBehaviour
{

    [SerializeField] private TextAsset mockDataFile;
    internal SocketModel socketModel = new SocketModel();
    internal Action onInit;
    internal Action onSpin;
    private Helper helper = new Helper();
 
    [SerializeField] private string authToken;

    protected string gameID = "SL-VIK";

    internal bool isLoaded = false;

    private const int maxReconnectionAttempts = 6;
    private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(2);
    string myAuth = null;

    private void Awake()
    {
        isLoaded = false;
    }

    private void Start()
    {
        //OpenWebsocket();
        // OpenSocket();
    }



    internal void InitiateSocket(string token)
    {

        Debug.Log("Initialize socket");
        Invoke("OnConnected",0.5f);


    }




    // Connected event handler implementation
    void OnConnected()
    {
        Debug.Log("Connected!");
        JObject mockData = JObject.Parse(mockDataFile.text);

        // Simulate InitData event
        string initDataJson = mockData["InitData"].ToString();
        RecievedData(initDataJson);
    }


    internal void ReceivedMessage()
    {
        Debug.Log("Received some_event with data: ");
        Invoke("SimulateSpin",1f);


    }

 
    void SimulateSpin(){
        JObject mockData = JObject.Parse(mockDataFile.text);

        // Simulate InitData event
        string initDataJson = mockData["ResultData"].ToString();
        RecievedData(initDataJson);
    }


    private void RecievedData(string modifiedResponse)
    {
        Debug.Log("in received data :" + modifiedResponse);
        JObject jsonObject = JObject.Parse(modifiedResponse);
        string messageId = jsonObject["id"].ToString();

        Debug.Log("message id " + messageId);


        var message = jsonObject["message"];
        var gameData = message["GameData"];
        if (messageId == "InitData")
        {
            socketModel.UIdata = message["UIData"].ToObject<UIData>();

            socketModel.initGameData.Lines = gameData["Lines"].ToObject<List<List<int>>>();
            socketModel.initGameData.Bets = gameData["Bets"].ToObject<List<double>>();
            socketModel.initGameData.canSwitchLines = gameData["canSwitchLines"].ToObject<bool>();
            socketModel.initGameData.LinesCount = gameData["LinesCount"].ToObject<List<int>>();
            socketModel.PlayerData = message["PlayerData"].ToObject<PlayerData>();  
            onInit?.Invoke();
        }
        else if (messageId == "ResultData")
        {
            socketModel.resultGameData.ResultReel = helper.ConvertStringListsToIntLists(gameData["ResultReel"].ToObject<List<List<string>>>());
            socketModel.resultGameData.linesToEmit = gameData["linesToEmit"].ToObject<List<int>>();
            socketModel.resultGameData.symbolsToEmit = gameData["symbolsToEmit"].ToObject<List<List<string>>>();
            socketModel.resultGameData.WinAmout = gameData["WinAmout"].ToObject<double>();
            socketModel.resultGameData.freeSpins = gameData["freeSpins"].ToObject<double>();
            socketModel.resultGameData.jackpot = gameData["jackpot"].ToObject<double>();
            socketModel.resultGameData.isBonus = gameData["isBonus"].ToObject<bool>();
            socketModel.resultGameData.BonusStopIndex = gameData["BonusStopIndex"].ToObject<double>();
            onSpin?.Invoke();

        }

        print("player data: " + JsonConvert.SerializeObject(socketModel.PlayerData));


    }

    internal double ChangeLineBet()
    {
        socketModel.currentBetIndex++;
        if (socketModel.currentBetIndex >= socketModel.initGameData.Bets.Count)
        {
            socketModel.currentBetIndex = 0;
        }

        return socketModel.initGameData.Bets[socketModel.currentBetIndex];
    }







}
