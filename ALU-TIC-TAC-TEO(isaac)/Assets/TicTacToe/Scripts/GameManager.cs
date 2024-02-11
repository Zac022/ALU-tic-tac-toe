using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace tic_tac
{
    public class GameManager : MonoBehaviour
    {
        public TickManager Tick;
        public Button PlayButton;
      

        private void OnEnable()
        {
            PlayButton.onClick.AddListener(StartGame);
           
        }
        

        void StartGame()
        {
            // set the start player move text
            Tick.UpdatePlayType((int)PLAYTIME.OPPONENT);
            Tick.AI = false;
        }

        void StartGameAI()
        {
            Tick.UpdatePlayType((int)PLAYTIME.AI);
            Tick.AI = true;
            Tick.currentTick = TICK.CIRCLE;
        }

        private void OnDisable()
        {
            PlayButton.onClick.RemoveListener(StartGame);
           
        }


        public void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
