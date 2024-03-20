using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject Kirby;
	public GameObject AbilityPanel;
    public GameObject HealthBar;
    public GameObject LifeCounter;
    public Sprite[] Numbers = new Sprite[10];
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int x = Kirby.GetComponent<PlayerController>().lives;
        int y = x.ToString().Length;
        int z = 0;
        while (y > 0) {
			LifeCounter.transform.GetChild(y-1).GetComponent<Image>().sprite = Numbers[int.Parse(x.ToString()[z].ToSafeString())];
            z++;
            y--;
		}
	}

    public void on_player_damaged(int health) {
		Animator ani = AbilityPanel.GetComponent<Animator>();
        ani.Play("ABL_Damage", 0);
		HealthBar.GetComponent<HealthBar>().set_value(health);
	}


}
