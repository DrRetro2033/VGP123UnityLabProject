using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public GameObject AbilityPanel;
    public GameObject HealthBar;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void on_player_damaged(int health) {
		Animator ani = AbilityPanel.GetComponent<Animator>();
        ani.Play("ABL_Damage", 0);
		HealthBar.GetComponent<HealthBar>().set_value(health);
	}
}
