﻿using UnityEngine;
using System.Collections;

public class ScreenScript : MonoBehaviour {

    public GameObject glassScreen0;
    public GameObject glassScreen1;
    public GameObject glassScreen2;
    public GameObject glassScreen3;
    public GameObject blankScreen;
    public GameObject particle;

    public bool explode = false;
    private bool waitActive = false;

	// Use this for initialization
	void Start () {
        glassScreen0.GetComponent<SpriteRenderer>().enabled = false;
        glassScreen1.GetComponent<SpriteRenderer>().enabled = false;
        glassScreen2.GetComponent<SpriteRenderer>().enabled = false;
        glassScreen3.GetComponent<SpriteRenderer>().enabled = false;
        blankScreen.GetComponent<SpriteRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (explode) {
            Explode(2);
            explode = false;
        }

        if (waitActive) 
            blankScreen.GetComponent<SpriteRenderer>().enabled = true;
        else
            blankScreen.GetComponent<SpriteRenderer>().enabled = false;
	}

    void Explode (int screen) {
        StartCoroutine(Wait());
        particle.GetComponent<ParticleSystem>().Play();
        switch (screen) {
            case 0 : glassScreen0.GetComponent<SpriteRenderer>().enabled = true;
                     break;
            case 1 : glassScreen1.GetComponent<SpriteRenderer>().enabled = true;
                     break;
            case 2 : glassScreen2.GetComponent<SpriteRenderer>().enabled = true;
                     break;
            case 3 : glassScreen3.GetComponent<SpriteRenderer>().enabled = true;
                     break;
        }
    }

     IEnumerator Wait(){
         waitActive = true;
         yield return new WaitForSeconds (0.01f);
         waitActive = false;
     }
}
