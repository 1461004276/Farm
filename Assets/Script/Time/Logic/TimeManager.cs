using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFarm.Save;
using Script.Utilities;

public class TimeManager : Singleton<TimeManager>,ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season _gameSeason = Season.春天;//默认游戏进入为春天
    private int _monthInSeason = 3;//三个月为一个季节
    private bool _gameClockPause;//定义一个布尔值来控制游戏的暂停
    private float _timeCount;//计时器
    private float _lightTimeDifference;//灯光时间差
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    public string GUID => GetComponent<DataGUID>().guid;
    private void OnEnable()
    {
        EventSystem.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventSystem.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventSystem.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventSystem.StartNewGameEvent += OnStartNewGameEvent;
        EventSystem.EndGameEvent += OnEndGameEvent;
    }
    private void OnDisable()
    {
        EventSystem.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventSystem.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventSystem.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventSystem.StartNewGameEvent -= OnStartNewGameEvent;
        EventSystem.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        _gameClockPause = true;
    }

    private void OnStartNewGameEvent(int index)
    {
        NewGameTime();
        _gameClockPause = false;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        _gameClockPause = gameState == GameState.Pause;
    }

    private void OnAfterSceneLoadedEvent()
    {
        _gameClockPause = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        _gameClockPause = true;
    }

    private void Start()
    {
/*        EventSystem.CallGameDateSeason(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventSystem.CallGameMinuteEvent(gameMinute, gameHour,gameSeason,gameDay);
        //切换灯光
        EventSystem.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);*/
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        _gameClockPause= true;
    }
    private void Update()
    {
        if (!_gameClockPause)
        {
            //编写一个秒针计时器
            _timeCount += Time.deltaTime;
            if (_timeCount >= Prams.secondThreshold)
            {
                _timeCount -= Prams.secondThreshold;
                UpdateGameTime();
            }
        }
        if (Input.GetKey(KeyCode.T))//作弊按钮(时间加快)
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }
        if (Input.GetKeyDown(KeyCode.G))//作弊按钮(天数增加)
        {
            gameDay++;
            EventSystem.CallGameDayEvent(gameDay, _gameSeason);
            EventSystem.CallGameDateSeason(gameHour, gameDay, gameMonth, gameYear, _gameSeason);
        }
    }
    private void NewGameTime()//新开一局游戏时给游戏初始化赋值
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;//从早上七点开始游戏
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2022;
        _gameSeason = Season.春天;
    }
    private void UpdateGameTime()//更新游戏时间,秒分年月日依次递进
    {
        gameSecond++;
        if (gameSecond > Prams.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Prams.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Prams.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Prams.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                            gameMonth = 1;

                        _monthInSeason--;
                        if (_monthInSeason == 0)
                        {
                            _monthInSeason = 3;

                            int seasonNumber = (int)_gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Prams.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }

                            _gameSeason = (Season)seasonNumber;

                            if (gameYear > 9999)
                            {
                                gameYear = 2022;
                            }
                        }
                        //用来刷新地图和农作物生长
                        EventSystem.CallGameDayEvent(gameDay, _gameSeason);
                    }
                }
                //每时间执行到此位置，调用一下委托时间
                EventSystem.CallGameDateSeason(gameHour, gameDay, gameMonth, gameYear, _gameSeason); //这里需要调用一下委托事件
            }
            EventSystem.CallGameMinuteEvent(gameMinute, gameHour,_gameSeason,gameDay);//这里需要调用一下委托事件
            //切换灯光
            EventSystem.CallLightShiftChangeEvent(_gameSeason, GetCurrentLightShift(), _lightTimeDifference);
        }
    }
    /// <summary>
    /// 返回LightShift同时计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Prams.morningTime && GameTime < Prams.nightTime)
        {
            _lightTimeDifference = (float)(GameTime - Prams.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        if (GameTime < Prams.morningTime || GameTime >= Prams.nightTime)
        {
            _lightTimeDifference = Mathf.Abs((float)(GameTime - Prams.nightTime).TotalMinutes);
            Debug.Log(_lightTimeDifference);
            return LightShift.Night;
        }
        return LightShift.Morning;
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameSeason", (int)_gameSeason);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);
        return saveData;
    }

    public void RestoreData(GameSaveData saveDate)
    {
        gameYear = saveDate.timeDict["gameYear"];
        _gameSeason = (Season)saveDate.timeDict["gameSeason"];
        gameMonth = saveDate.timeDict["gameMonth"];
        gameDay = saveDate.timeDict["gameDay"];
        gameHour = saveDate.timeDict["gameHour"];
        gameMinute = saveDate.timeDict["gameMinute"];
        gameSecond = saveDate.timeDict["gameSecond"];
    }
}
