using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LeaderBoard
{
    public class LeaderBoard : MonoBehaviour
    {
        LeaderBoardManager data;
        [SerializeField]public Tiers ActiveTier;
        public TMP_Text LeaderBoardTitle;
        public GameObject TheLeaderBoard;
        public GameObject Banner;
        public bool IsUseTiers = false;

        // Start is called before the first frame update
        void Start ()
        {
            data = FindObjectOfType<LeaderBoardManager>(true);
        }

        // Update is called once per frame
        void Update ()
        {
            if(data != null)
            {
                IsUseTiers = data.CanUseTiers;
            }
        }

        public void ResetLeaderBoard ()
        {
            HideLeaderBoard();
        }

        public void refreshLeaderBoard ()
        {
            SetLeaderBoardTitle();
        }

        public void SetLeaderBoardTitle ()
        {
            
            if (!IsUseTiers)
            {
                LeaderBoardTitle.text = "WORLD RANK";
                SetLeaderBoardColor(false,LeaderBoardTitle);
            }
            else
            { 
                LeaderBoardTitle.text = "WORLD RANK" + " : " + ActiveTier.ToString();
                SetLeaderBoardColor(true, LeaderBoardTitle);
            }
        }

        public void SetLeaderBoardColor (bool IsUseTiers,TMP_Text text)
        {
            const string DefaultBanner_color = "#45D9FF";
            const string DefaultText_color = "#282828";
       
            if (IsUseTiers)
            {
                Color newColor = Banner.GetComponent<Image>().color = data.GetColorForTier(ActiveTier);
                if (ColorUtils.IsColorBright(newColor))
                {
                    if (ColorUtility.TryParseHtmlString(DefaultText_color , out Color color_))
                    {
                        text.color = color_;
                        text.outlineColor = color_;
                    }
                }
                else
                {
                    text.color = Color.white;
                    text.outlineColor = Color.white;
                }

            }
            else
            {
                if (ColorUtility.TryParseHtmlString(DefaultBanner_color , out Color color))
                {
                    Banner.GetComponent<Image>().color = color;

                }

                if(ColorUtility.TryParseHtmlString(DefaultText_color , out Color color_))
                {
                    text.color = color_;
                    text.outlineColor = color_;
                }
            }
        }

        public void ShowLeaderBoard ()
        {
            TheLeaderBoard.SetActive(true);
            TheLeaderBoard.transform.localScale = Vector3.one;
            SetLeaderBoardTitle();
        }

        public void HideLeaderBoard ()
        {
            TheLeaderBoard.SetActive(false);
            TheLeaderBoard.transform.localScale = Vector3.zero;
        }

        public void IncreaseTier ()
        {
            ActiveTier++;

            Array tiersArray = System.Enum.GetValues(typeof(Tiers));
            int enumLength = tiersArray.Length;

            // Check if ActiveTier exceeds the highest tier
            if ((int)ActiveTier >= enumLength) 
            {
                ActiveTier = (Tiers)( enumLength - 1 ); // Set to the last enum value
            }
            SetLeaderBoardTitle();
        }


        public void LowerTier ()
        {
            ActiveTier--;

            if ((int)ActiveTier < 0) 
            {
                ActiveTier = (Tiers)0; 
            }

            SetLeaderBoardTitle();
        }


    }
}

