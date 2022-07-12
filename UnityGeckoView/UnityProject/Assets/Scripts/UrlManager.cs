using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;


public class UrlManager : MonoBehaviourPun
{

    public Launcher l;


    public BrowserView screenOne;
    public BrowserView screenTwo;
    public BrowserView screenThree;
    public BrowserView screenFour;
    public BrowserView screenFive;
    public BrowserView screenSix;
    public BrowserView screenSeven;
    public BrowserView screenEight;

    public string urlOne;
    public string urlTwo;
    public string urlThree;
    public string urlFour;
    public string urlFive;
    public string urlSix;
    public string urlSeven;
    public string urlEight;

    public string mainUrl;


    private void Update()
    {

            if (Input.GetKeyDown("1"))
            {
                LoadVis1();
            }
            else if (Input.GetKeyDown("2"))
            {
                LoadVis2();
            }
            else if (Input.GetKeyDown("3"))
            {
                LoadVis3();
            }

    }

    public void LoadVis1() 
    {
        if (MasterManager.GameSettings.Observer)
        {

            changeMaterial(screenOne.gameObject, "Materials/" + urlOne );
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo );
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree );
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour );
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive );
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix );
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven );
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight );
        }
        else
        {
            if (!screenOne.enabled)
            {

                screenOne.startUrl = mainUrl + urlOne + ".html";
                screenTwo.startUrl = mainUrl + urlTwo + ".html";
                screenThree.startUrl = mainUrl + urlThree + ".html";
                screenFour.startUrl = mainUrl + urlFour + ".html";
                screenFive.startUrl = mainUrl + urlFive + ".html";
                screenSix.startUrl = mainUrl + urlSix + ".html";
                screenSeven.startUrl = mainUrl + urlSeven + ".html";
                screenEight.startUrl = mainUrl + urlEight + ".html";

                screenOne.enabled = true;
                screenTwo.enabled = true;
                screenThree.enabled = true;
                screenFour.enabled = true;
                screenFive.enabled = true;
                screenSix.enabled = true;
                screenSeven.enabled = true;
                screenEight.enabled = true;
            }
            else
            {

                screenOne.LoadURL(mainUrl + urlOne + ".html");
                screenTwo.LoadURL(mainUrl + urlTwo + ".html");
                screenThree.LoadURL(mainUrl + urlThree + ".html");
                screenFour.LoadURL(mainUrl + urlFour + ".html");
                screenFive.LoadURL(mainUrl + urlFive + ".html");
                screenSix.LoadURL(mainUrl + urlSix + ".html");
                screenSeven.LoadURL(mainUrl + urlSeven + ".html");
                screenEight.LoadURL(mainUrl + urlEight + ".html");
            }
        }

    }

    public void LoadVis2()
    {


        if (MasterManager.GameSettings.Observer)
        {


            changeMaterial(screenOne.gameObject, "Materials/" + urlOne + "_Gender");
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo + "_Gender");
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree + "_Gender");
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour + "_Gender");
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive + "_Gender");
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix + "_Gender");
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven + "_Gender");
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight + "_Gender");


        }
        else
        {
            if (!screenOne.enabled)
            {

                screenOne.startUrl = mainUrl + urlOne + "_Gender.html";
                screenTwo.startUrl = mainUrl + urlTwo + "_Gender.html";
                screenThree.startUrl = mainUrl + urlThree + "_Gender.html";
                screenFour.startUrl = mainUrl + urlFour + "_Gender.html";
                screenFive.startUrl = mainUrl + urlFive + "_Gender.html";
                screenSix.startUrl = mainUrl + urlSix + "_Gender.html";
                screenSeven.startUrl = mainUrl + urlSeven + "_Gender.html";
                screenEight.startUrl = mainUrl + urlEight + "_Gender.html";

                screenOne.enabled = true;
                screenTwo.enabled = true;
                screenThree.enabled = true;
                screenFour.enabled = true;
                screenFive.enabled = true;
                screenSix.enabled = true;
                screenSeven.enabled = true;
                screenEight.enabled = true;
            }
            else
            {

                screenOne.LoadURL(mainUrl + urlOne + "_Gender.html");
                screenTwo.LoadURL(mainUrl + urlTwo + "_Gender.html");
                screenThree.LoadURL(mainUrl + urlThree + "_Gender.html");
                screenFour.LoadURL(mainUrl + urlFour + "_Gender.html");
                screenFive.LoadURL(mainUrl + urlFive + "_Gender.html");
                screenSix.LoadURL(mainUrl + urlSix + "_Gender.html");
                screenSeven.LoadURL(mainUrl + urlSeven + "_Gender.html");
                screenEight.LoadURL(mainUrl + urlEight + "_Gender.html");
            }
        }
    }

    public void LoadVis3()
    {


        if (MasterManager.GameSettings.Observer)
        {


            changeMaterial(screenOne.gameObject, "Materials/" + urlOne + "_Third");
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo + "_Third");
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree + "_Third");
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour + "_Third");
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive + "_Third");
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix + "_Third");
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven + "_Third");
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight + "_Third");


        }
        else
        {
            if (!screenOne.enabled)
            {

                screenOne.startUrl = mainUrl + urlOne + "_Third.html";
                screenTwo.startUrl = mainUrl + urlTwo + "_Third.html";
                screenThree.startUrl = mainUrl + urlThree + "_Third.html";
                screenFour.startUrl = mainUrl + urlFour + "_Third.html";
                screenFive.startUrl = mainUrl + urlFive + "_Third.html";
                screenSix.startUrl = mainUrl + urlSix + "_Third.html";
                screenSeven.startUrl = mainUrl + urlSeven + "_Third.html";
                screenEight.startUrl = mainUrl + urlEight + "_Third.html";

                screenOne.enabled = true;
                screenTwo.enabled = true;
                screenThree.enabled = true;
                screenFour.enabled = true;
                screenFive.enabled = true;
                screenSix.enabled = true;
                screenSeven.enabled = true;
                screenEight.enabled = true;
            }
            else
            {

                screenOne.LoadURL(mainUrl + urlOne + "_Third.html");
                screenTwo.LoadURL(mainUrl + urlTwo + "_Third.html");
                screenThree.LoadURL(mainUrl + urlThree + "_Third.html");
                screenFour.LoadURL(mainUrl + urlFour + "_Third.html");
                screenFive.LoadURL(mainUrl + urlFive + "_Third.html");
                screenSix.LoadURL(mainUrl + urlSix + "_Third.html");
                screenSeven.LoadURL(mainUrl + urlSeven + "_Third.html");
                screenEight.LoadURL(mainUrl + urlEight + "_Third.html");
            }
        }
    }
    public void changeMaterial(GameObject g, string s) {

        Material[] a = new Material[1];
        a[0] = Resources.Load(s, typeof(Material)) as Material;
        g.GetComponent<MeshRenderer>().materials = a;
    }
}
