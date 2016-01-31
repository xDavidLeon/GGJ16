using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class ButtonAnswer : MonoBehaviour {
    public int id = 0;
    private GameManager gm;
    private Text text;
    private Image img;
    private Color colorOriginal;
    public Color colorFocused = Color.grey;
    public Image imgPrompt;

    public HashSet<Player> playersFocusing;
    public Player playerSelected = null;

	// Use this for initialization
	void Start () {
        gm = GameManager.instance;
        text = GetComponentInChildren<Text>();
        img = GetComponent<Image>();
        colorOriginal = img.color;
        text.text = gm.dialogDatabase.answers[id].text;
        gm.buttons.Add(this);
        playersFocusing = new HashSet<Player>();
	}
	
    public void SetColor(Color c)
    {
        DOTween.Kill(this);
        img.DOColor(c, 0.5f).SetId(this);
    }

    public void Reset()
    {
        playerSelected = null;
        playersFocusing.Clear();
        img.color = colorOriginal;
        imgPrompt.DOFade(0.0f, 0.1f);
    }

    void Update()
    {
        if (playerSelected == null)
        {
            if (playersFocusing.Count > 0)
            {
                img.color = colorFocused;
                imgPrompt.DOFade(1.0f, 0.25f);
            }
            else
            {
                img.color = colorOriginal;
                imgPrompt.DOFade(0.0f, 0.25f);
            }
            }
        }

    public void SelectByPlayer(Player p)
    {
        playerSelected = p;
        SetColor(p.characterColor);
        imgPrompt.DOFade(0.0f, 0.1f);
    }

}
