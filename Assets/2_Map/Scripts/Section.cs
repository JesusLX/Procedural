using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour {
    public int Ypos;
    public int Xpos;
    public int type;
    int waterN;
    int earthN;

    public void Regenerate() {
        Init();
    }
    public void CheckNeightbors() {
        //Hace el checkeo en un hilo distinto para que no paralice el juego
        StartCoroutine(DoCheckNeightbors());
    }
    private IEnumerator DoCheckNeightbors() {
        waterN = 0;
        earthN = 0;
        for (int resX = -1; resX <= 1; resX++) {
            for (int resY = -1; resY <= 1; resY++) {
                //Si no es la casilla pripia
                if (resX != 0 || resY != 0)
                    //Si la casilla no se por los bordes
                    if ((((Xpos + resX) >= 0) && ((Xpos + resX) < MapGenerator.Instance.Width))
                        &&
                        (((Ypos + resY) >= 0) && ((Ypos + resY) < MapGenerator.Instance.Heigth))) {
                        CheckAndIncrease(Xpos + resX, Ypos + resY);
                    }
            }
        }
        //Cambia los Sprites 
        if (earthN > waterN) {
            if (type != 0) {
                type = 0;
                GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.EarthSprite;
            }
        } else {
            if (earthN < waterN) {
                if (type != 1) {
                    type = 1;
                    GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.WaterSprite;
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }
    public void Actualize() {
        //Cambia los Sprites 
        if (earthN > waterN) {
            if (type != 0) {
                type = 0;
                GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.EarthSprite;
            }
        } else if (earthN < waterN) {
            if (type != 1) {
                type = 1;
                GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.WaterSprite;
            }
        } else {
            Debug.Log("Iguales");
            if (type != 2) {
                type = 2;
                GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.MountainSprite;
            }
        }

    }
    private void CheckAndIncrease(int x, int y) {
        //Cuenta cuantas fichas de cada tipo hay 
        if (MapGenerator.Instance.map[y, x].type == 0)
            earthN++;
        else if (MapGenerator.Instance.map[y, x].type == 1)
            waterN++;

    }

    internal void Init() {
        //Decide si es agua o no
        type = Convert.ToInt16(!(UnityEngine.Random.Range(0, 101) > MapGenerator.Instance.waterProb));
        if (type == 0) {
            GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.EarthSprite;
        } else {
            GetComponent<SpriteRenderer>().sprite = MapGenerator.Instance.WaterSprite;
        }
    }
}
