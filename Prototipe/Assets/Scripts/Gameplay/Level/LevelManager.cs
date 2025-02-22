﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public List<LevelDataSO> levels;
    public LevelDataSO actualLevelDataSO;

    public GameObject[] rocks;
    public GameObject[] WaterPaths;
    public int actualLvl;
    public static Action ActivateLvlSpawners;
    public List<Transform> activeSpawnStarts;
    public Transform[] allSpawners;
    public static Action<string> SetDayOn;
    public static Action<string> SetNightOn;
    public static Action<NPC_SO, string> ShowNPCs;
    public static Action DayEnds;
    public static event Action FinishLastLevel;
    [SerializeField] private LevelSounds levelSounds;
    static public event Action<string> ExecutelevelSounds;

    static public LevelManager instanceLevelManager;
    static public LevelManager instance { get { return instanceLevelManager; } }

    //private uint levelManagerMusicID;

    private void Start()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].actualWave = 0;
        }
        WaveSpawner.ShowNPC += InvokeNPCShow;
    }
    private void Awake()
    {
        if (instanceLevelManager != null && instanceLevelManager != this)
            Destroy(this.gameObject);
        else
            instanceLevelManager = this;
    }
    private void OnDisable()
    {
        WaveSpawner.ShowNPC -= InvokeNPCShow;
        //AkSoundEngine.StopPlayingID(levelManagerMusicID);
    }
    public void FindLvlInformation()  /// setea cantidad de waves, habilita spawners o limpia piedras
    {
        if (GameManager.instance.finishDay)
            StartNight();
        if (actualLevelDataSO.itIsDay && GameManager.instance.finishNight)
        {
            StartDay();
        }
        ActivateSpawner(actualLevelDataSO.activateSpawner);
        ClearRock(actualLevelDataSO.clearRock);
        ActivatePath(actualLevelDataSO.activateWater);
    }

    private void ActivateSpawner(int Index)
    {
        if (Index != -1)
        {
            allSpawners[Index].gameObject.SetActive(true);
            activeSpawnStarts.Add(allSpawners[Index]);
        }
    }

    private void ClearRock(int Index)
    {
        if (Index != -1)
        {
            rocks[Index - 1].SetActive(false);
        }
        //levelManagerMusicID = AkSoundEngine.PostEvent("level_wallcollapse", this.gameObject);
        ExecutelevelSounds?.Invoke("level_wallcollapse");
        //levelSounds.ExecuteSound("level_wallcollapse");

    }
    private void ActivatePath(int Index)
    {
        if (Index != -1)
        {
            WaterPaths[Index - 1].SetActive(true);
        }
        //levelManagerMusicID = AkSoundEngine.PostEvent("level_newriver_play", this.gameObject);
        ExecutelevelSounds?.Invoke("level_newriver_play");
        //levelSounds.ExecuteSound("level_newriver_play");
    }

    public int GetQuantityOfEnemieTypesInThisWave()
    {
        return actualLevelDataSO.waves[actualLevelDataSO.actualWave].Enemies.Count;
    }

    public int GetActualLVL()
    {
        return actualLvl;
    }
    public void IncreaseLVL()
    {
        if (actualLevelDataSO != null)
        {
            actualLvl++;
            actualLevelDataSO = levels[actualLvl];
            DayEnds?.Invoke();
            CheckLastLevel();
        }

    }
    public int GetActualWave()
    {
        if (actualLevelDataSO != null)
        {
            return actualLevelDataSO.actualWave;
        }
        return 0;
    }
    public void IncreaseActualWave()
    {
        actualLevelDataSO.actualWave++;
    }
    public int GetTotalWaves()
    {
        return actualLevelDataSO.waves.Count;
    }
    public bool CompareActualWaveAndTotalWavesAreEquals()
    {
        return ((GetTotalWaves() == GetActualWave()));
    }
    public float GetTimeBetweenWaves()
    {
        if (actualLevelDataSO.actualWave < actualLevelDataSO.waves.Count)
            return actualLevelDataSO.waves[actualLevelDataSO.actualWave].timeBetweenWaves;
        else
            return actualLevelDataSO.waves[0].timeBetweenWaves;
    }
    public void StartDay()
    {
        SetDayOn?.Invoke("Day");
    }
    public void StartNight()
    {
        //Con este evento llamar a la funcion del gamemanager que controla los ciclos
        if (!actualLevelDataSO.itIsDay)
        {
            SetNightOn?.Invoke("Night");
        }
    }
    public void InvokeNPCShow()
    {
        ShowNPCs?.Invoke(actualLevelDataSO.npcToTalk, actualLevelDataSO.dialog);
    }
    public bool AskIfDay()
    {
        return actualLevelDataSO.itIsDay;
    }

    public void CheckLastLevel()
    {
        if (actualLvl >= GameManager.instance.lastLvl)
            FinishLastLevel?.Invoke();
    }

    //public void StopMusic()
    //{
    //    AkSoundEngine.StopPlayingID(levelManagerMusicID);
    //}
}
