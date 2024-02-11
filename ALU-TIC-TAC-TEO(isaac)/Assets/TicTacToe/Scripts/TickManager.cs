using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace tic_tac
{

    /// <summary>
    /// enum for easy casting to integer for selection of strike type
    /// </summary>
    public enum STRIKETYPE
    {
        VERTSTRIKELEFT,
        VERTSTRIKEMIDDLE,
        VERTSTRIKERIGHT,
        HORIZONTALSTRIKEUP,
        HORIZONTALSTRIKEMIDDLE,
        HORIZONTALSTRIKEDOWN,
        CROSSTRIKERIGHT,
        CROSSTRIKELEFT
    };

    /// <summary>
    /// type of ticks to select with int casting
    /// </summary>
    public enum TICK
    {
        CIRCLE,
        CROSS,
        EMPTY
    };

    /// <summary>
    ///  tyope of playtype to select to casting
    /// </summary>
    public enum PLAYTIME
    {
        PLAYER,
        OPPONENT,
        AI
    };

    public class TickManager : MonoBehaviour
    {

        // reference to all buttons
        [FormerlySerializedAs("allButtons")] public List<Button> AllButtons = new();

        // keep reference to line strikes
        [SerializeField] private List<GameObject> AllStrikes = new();

        // determines how fast the letter should grow when activated.
        [SerializeField] [Range(1f, 10f)] private float GrowFactor = 5.0f;

        // current tick type during runtime.
        public TICK currentTick;

        // reference to playing type
        [SerializeField] private TextMeshProUGUI PlayText;

        // tick types
        private readonly String[] TICKS = { "O", "X", "" };

        // playing state types
        private readonly String[] PLAY = { "Player O Move", "Player X Move", "Player VS AI" };

        // current player type 
        public bool AI = false;

        // reference to reset button
        public GameObject ResetButton;

        // keeps count of the maximum number of moves.
        private int NumberOfMoves = 0;

        private TextMeshProUGUI TextHold;
        private bool canGrow = false;
        private float MaxGrowSize = 160.0f;
        private const float MinGrowSize = 20.0f;

        private int[] Board = new int[]{ 0, 0, 0 , 0, 0, 0 , 0, 0, 0 };
        private int current = 0;
        
        // player check if there is a move to make for both ai and player
        // generate random move ai after player plays
        // check winning as always.
        private bool CheckAvailableMove()
        {
            if (NumberOfMoves <= 8)
            {
                return true;
            }
            return false;
        }

        private int currentMove = 0;
        private int GenerateRandomPosition()
        {
            int move = -1;
           currentMove = Random.Range(0, AllButtons.Count);
           for (int j = 0; j < 8; j++)
           {
               for (int i = 0; i < AllButtons.Count; i++)
               {
                   if (AllButtons[currentMove].interactable)
                   {
                       //if (i == currentMove)
                       //{
                       //    return currentMove;
                       //}
                       return currentMove;
                       continue;
                   }

                   continue;
               }
           }
           return move;
        }

        private void AIPlays()
        {
            if(CheckAvailableMove())
            {
                int position = GenerateRandomPosition();
                if (position < 0)
                {
                    // end the game
                    ResetButton.SetActive(true);
                    CheckBoardPlayer();
                    CheckCurrentBoard();
                    return;
                }
                AI_Move(position, (int)TICK.CROSS);
            }
        }
        
        
        
        /// <summary>
        /// /// checks board for two normal players playing
        /// </summary>
    public void CheckBoardPlayer()
    {
        Check_Template(0, 1, 2, (int)STRIKETYPE.HORIZONTALSTRIKEUP);
        Check_Template(3, 4, 5, (int)STRIKETYPE.HORIZONTALSTRIKEMIDDLE);
        Check_Template(6, 7, 8, (int)STRIKETYPE.HORIZONTALSTRIKEDOWN);
        
        Check_Template(0, 3, 6, (int)STRIKETYPE.VERTSTRIKELEFT);
        Check_Template(1,4,7,(int)STRIKETYPE.VERTSTRIKEMIDDLE);
        Check_Template(2, 5, 8, (int)STRIKETYPE.VERTSTRIKERIGHT);
        
        Check_Template(0, 4, 8, (int)STRIKETYPE.CROSSTRIKELEFT);
        Check_Template(2,4,6, (int)STRIKETYPE.CROSSTRIKERIGHT);
    }

        
        /// <summary>
        /// checks winning strike
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="DrawIndex"></param>
        private void Check_Template(int first, int second, int third, int DrawIndex)
        {
            if (Board[first] == 1 && Board[second] == 1 && Board[third] == 1)
            {
                StrikeLine(DrawIndex);
                // call winner here x for ai, x for x player
                if (AI)
                {
                    PlayText.text = "<color=red>AI Wins</color>";
                }
                else
                {
                    PlayText.text = "<color=green>X Player Wins</color>";
                }
                return;
            }
            
            if (Board[first] == 2 && Board[second] == 2 && Board[third] == 2)
            {
                StrikeLine(DrawIndex);
                // call winner here
                if (AI)
                {
                    PlayText.text = "<color=green>Human Player Wins</color>";
                }
                else
                {
                    PlayText.text = "<color=green>O Player Wins</color>";
                }
                return;
            }
        }

    /// <summary>
    /// updates our memory in board
    /// </summary>
    public void CheckCurrentBoard()
    {
        for (int i = 0; i < AllButtons.Count; i++)
        {
            Board[i] =  (AllButtons[i].interactable) ? 0 : 1;
            Board[i] = PlayerSelectionType(AllButtons[i]);
            // Debug.Log(Board[i]);
        }
    }

    /// <summary>
    /// selects the board based on the player input type
    /// </summary>
    /// <returns>state of comparism</returns>
    public int PlayerSelectionType(Button B)
    {
        TextMeshProUGUI current = B.GetComponentInChildren<TextMeshProUGUI>();
        if (current.text == "X")
        {
            return 1;
        }

        if (current.text == "O")
        {
            return 2;
        }

        return 0;
    }
    
    
    private void Start()
    {
        currentTick = TICK.CROSS;
        NumberOfMoves = 0;
    }


    private void Update()
    {
        AnimateGrowth();
        CountMoveNumber();
        UpdatePlayerState();
    }

    /// <summary>
    /// check player state based on frame counts.
    /// </summary>
    private void UpdatePlayerState()
    {
        if (Time.frameCount % 50 == 0)
        {
            CheckBoardPlayer();
            CheckCurrentBoard();
        }
    }

    /// <summary>
    /// checks the number of moves made.
    /// </summary>
    private void CountMoveNumber()
    {
        if (NumberOfMoves == 9)
        {
            Debug.LogError("Maximum moves arrived");
            ResetButton.SetActive(true);
            NumberOfMoves++;
        }
    }

    /// <summary>
    /// animate ticks for smooth transition
    /// </summary>
    private TextMeshProUGUI primaryHold;
    //private TextMeshProUGUI secondaryHold;
    private void AnimateGrowth()
    {
        if (canGrow && !AI)
        {
            primaryHold = TextHold;
            primaryHold.fontSize += Time.time * GrowFactor;
            
            
            if (primaryHold.fontSize >= 150)
            {
                canGrow = false;
            }
        }
    }

    /// <summary>
    /// binds all listeners to buttons
    /// </summary>
    private void OnEnable()
    {
        AllButtons[0].onClick.AddListener(PlayerMove00);
        AllButtons[1].onClick.AddListener(PlayerMove01);
        AllButtons[2].onClick.AddListener(PlayerMove02);
        
        AllButtons[3].onClick.AddListener(PlayerMove10);
        AllButtons[4].onClick.AddListener(PlayerMove11);
        AllButtons[5].onClick.AddListener(PlayerMove12);
        
        AllButtons[6].onClick.AddListener(PlayerMove20);
        AllButtons[7].onClick.AddListener(PlayerMove21);
        AllButtons[8].onClick.AddListener(PlayerMove22);
    }

    /// <summary>
    ///  activates the line striking
    /// </summary>
    /// <param name="index"></param>
    public void StrikeLine(int index)
    {
        AllStrikes[index].SetActive(true);
        foreach (Button B in AllButtons)
        {
            B.interactable = false;
        }
        
        ResetButton.SetActive(true);
    }

    /// <summary>
    /// updates thge current playing status
    /// </summary>
    /// <param name="index"></param>
    public void UpdatePlayType(int index)
    {
        PlayText.text = PLAY[index];
    }

  

    private void PlayerMove00()
    {
        Update_Button(AllButtons[0],(int)currentTick);
        SwapingTurns();
    }

    /// <summary>
    /// swap turns for playing
    /// </summary>
    private void SwapingTurns()
    {
        if (!AI)
        {
            UpdatePlayType(current);
            current = Math.Abs(current + (-1));
            if (current == 0)
            {
                currentTick = TICK.CROSS;
            }
            else
            {
                currentTick = TICK.CIRCLE;
            }

            NumberOfMoves++;
           
        }
        else
        {
            currentTick = TICK.CIRCLE;
            NumberOfMoves++;
            
            if(CheckAvailableMove())
            {
                AIPlays();
            }
        }
    }

    private void PlayerMove01()
    {
        Update_Button(AllButtons[1], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove02()
    {
        Update_Button(AllButtons[2], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove10()
    {
        Update_Button(AllButtons[3], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove11()
    {
        Update_Button(AllButtons[4], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove12()
    {
        Update_Button(AllButtons[5], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove20()
    {
        Update_Button(AllButtons[6], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove21()
    {
        Update_Button(AllButtons[7], (int)currentTick);
        SwapingTurns();
    }

    private void PlayerMove22()
    {
        Update_Button(AllButtons[8], (int)currentTick);
        SwapingTurns();
    }

    /// <summary>
    /// controls moves by the Ai
    /// </summary>
    /// <param name="position"></param>
    /// <param name="index"></param>
    public void AI_Move(int position, int index)
    {
        Update_Button_AI(AllButtons[position], index);
        AllButtons[position].interactable = false;
        NumberOfMoves++;
    }
    
    /// <summary>
    /// update buttons controlled by the player
    /// </summary>
    /// <param name="_b"></param>
    /// <param name="index"></param>
    private void Update_Button(Button _b,int index)
    {
        
        TextHold = _b.GetComponentInChildren<TextMeshProUGUI>();
        TextHold.fontSize = !AI ? MinGrowSize : MaxGrowSize;
        _b.interactable = false;
        TextHold.text = TICKS[index];
        canGrow = true;
    }
    
    private void Update_Button_AI(Button _b,int index)
    {
        
        TextHold = _b.GetComponentInChildren<TextMeshProUGUI>();
        TextHold.fontSize = MaxGrowSize;
        _b.interactable = false;
        TextHold.text = TICKS[index];
    }

    private void OnDisable()
    {
        AllButtons[0].onClick.RemoveListener(PlayerMove00);
        AllButtons[1].onClick.RemoveListener(PlayerMove01);
        AllButtons[2].onClick.RemoveListener(PlayerMove02);
        
        AllButtons[3].onClick.RemoveListener(PlayerMove10);
        AllButtons[4].onClick.RemoveListener(PlayerMove11);
        AllButtons[5].onClick.RemoveListener(PlayerMove12);
        
        AllButtons[6].onClick.RemoveListener(PlayerMove20);
        AllButtons[7].onClick.RemoveListener(PlayerMove21);
        AllButtons[8].onClick.RemoveListener(PlayerMove22);
    }
    
    }

}

