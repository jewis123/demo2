using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Scene;
using UI;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    [SerializeField]
    public UIManager uiManager;

    [SerializeField] 
    public GameSceneManager gameSceneManager;
    
    [SerializeField] 
    public BattleManager battleManager;
    // Start is called before the first frame update

    public static GameCore singleton { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        singleton = this;
    }

    void Start()
    {
        uiManager.OpenUI("LoginUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}
