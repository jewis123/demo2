using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string[] transportScenes;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(transportScenes!=null && transportScenes.Length>0)
            GameCore.singleton.uiManager.OpenUI("TransportSceneUI",transportScenes);
    }
}
