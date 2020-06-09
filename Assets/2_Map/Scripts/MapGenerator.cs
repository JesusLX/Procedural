using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator> {
    public int Heigth, Width;
    public GameObject SectionPrefab;
    public Section[,] map;
    public Sprite EarthSprite;
    public Sprite WaterSprite;
    public Sprite MountainSprite;
    public float waterProb;

    Action OnRegenerateAction;
    Action OnActualiceAction;
    Action OnCheckAction;

    private void Start() {
        Generate();
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TimeToCheck();
        }
    }
    public void Actualize() {
        OnActualiceAction?.Invoke();
    }public void TimeToCheck() {
        OnCheckAction?.Invoke();
    }

    public void Generate() {
        if (map == null) {
            map = new Section[Heigth, Width];
            for (int h = 0; h < Heigth; h++) {
                for (int w = 0; w < Width; w++) {
                    map[h, w] = Instantiate(SectionPrefab, new Vector3(w * .3f, h * .3f), Quaternion.identity, this.transform).GetComponent<Section>();
                    OnRegenerateAction += map[h, w].Regenerate;
                //    OnActualiceAction += map[h, w].Actualize;
                    OnCheckAction += map[h, w].CheckNeightbors;
                    map[h, w].Xpos = w;
                    map[h, w].Ypos = h;
                    map[h, w].Init();
                }
            }
        } else {
            OnRegenerateAction?.Invoke();
        }
    }

}
