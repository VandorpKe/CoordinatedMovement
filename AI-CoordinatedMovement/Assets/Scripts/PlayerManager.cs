using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Accesing input
using RTS.InputManager;

namespace RTS.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;
        public Transform units;

        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            InputHandler.instance.HandleInput();
        }
    }
}