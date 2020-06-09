using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : Singleton<CityGenerator> {
    public Plat[,] map;
    public int heigth, width;
    public GameObject PlatBasePrefab;
    public List<GameObject> PlatPrefabs;
    public Vector2 roadStart, roadEnd;

    public Action OnReloadAction;
    public Action OnStartChecking;
    public Action OnInstantiatePlats;
    public Action OnDieAction;
    private int contador;
    public int SchoolsN;
    public int FactoryN;
    private void Start() {
        FillMap();
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            FillMap();
        }
    }
    public void FillMap() {
        roadStart = GenerateStartStreetPos();
        roadEnd = GenerateEndStreetPos();
        if (map == null || map.GetLength(0) != width || map.GetLength(1) != heigth) {
            if (map != null && (map.GetLength(0) != width || map.GetLength(1) != heigth)) {
                OnDieAction?.Invoke();
                OnStartChecking = null;
                OnInstantiatePlats = null;
                OnReloadAction = null;
                OnDieAction = null;
            }
            map = new Plat[width, heigth];
            for (int h = 0; h < heigth; h++) {
                for (int w = 0; w < width; w++) {
                    map[w, h] = Instantiate(PlatBasePrefab, new Vector3(w, 0, h), Quaternion.identity).GetComponent<Plat>();
                    OnStartChecking += map[w, h].Check;
                    OnInstantiatePlats += map[w, h].InstantiatePlat;
                    OnReloadAction += map[w, h].Clear;
                    OnDieAction += map[w, h].Die;
                    map[w, h].Set(w, h);
                    map[w, h].gameObject.name = "(" + w + "," + h + ")";

                }
            }
        }
        OnReloadAction?.Invoke();
        CkeckRoads();
        GenerateBuildings(Plat.Type.Factory, FactoryN);
        GenerateBuildings(Plat.Type.School, SchoolsN);
        OnStartChecking?.Invoke();
        OnInstantiatePlats?.Invoke();
    }
    private void GenerateBuildings(Plat.Type type, int units) {
        List<Plat> plats = new List<Plat>(FindObjectsOfType<Plat>());
        plats.RemoveAll(p => p.type != Plat.Type.Empty);
        plats = DesordenarLista(plats);
        int factoryNCounter = units;
        for (int i = 0; i < plats.Count; i++) {
            factoryNCounter--;
            if (factoryNCounter < 0)
                break;
            plats[i].type = type;
        }
    }
    public void CkeckRoads() {
        Vector2 center = Abs((roadStart + roadEnd) / 2);
        if (center.x < width || center.y < heigth) {
            RoadTo(roadStart, center);
            RoadTo(center, roadEnd);
        } else {
            RoadTo(roadStart, roadEnd);
        }
        map[(int)roadStart.x, (int)roadStart.y].type = Plat.Type.Road;
        map[(int)roadEnd.x, (int)roadEnd.y].type = Plat.Type.Road;


    }
    private void RoadTo(Vector2 target1, Vector2 target2) {
        int n, x, y;

        n = (int)Mathf.Abs(target1.x - target2.x);
        y = (int)target1.y;
        Vector2 endpoint = Vector2.negativeInfinity;
        if (target1.x > target2.x) {
            for (int i = 0; i <= n; i++) {
                x = (int)target1.x - i;
                map[x, y].type = Plat.Type.Road;
                endpoint = new Vector2(x, y);
            }
        } else if (target1.x < target2.x) {
            for (int i = 0; i <= n; i++) {
                x = (int)target1.x + i;
                map[x, y].type = Plat.Type.Road;
                endpoint = new Vector2(x, y);
            }
        }
        if (endpoint != Vector2.negativeInfinity && endpoint.x >= 0 && endpoint.y >= 0) {
            target1 = endpoint;
            Debug.Log("Middlepoint");
            if (target1.x > target2.x) {
                for (int i = 0; i <= n; i++) {
                    x = (int)target1.x - i;
                    map[x, y].type = Plat.Type.Road;
                    endpoint = new Vector2(x, y);
                }
            } else if (target1.x < target2.x) {
                for (int i = 0; i <= n; i++) {
                    x = (int)target1.x + i;
                    if (x < width) {
                        Debug.Log(x + ", " + y);
                        map[x, y].type = Plat.Type.Road;
                        endpoint = new Vector2(x, y);
                    }
                }
            }
        }

        n = (int)Mathf.Abs(target1.y - target2.y);
        x = (int)target1.x;
        if (target1.y > target2.y) {
            for (int i = 0; i <= n; i++) {
                y = (int)target1.y - i;
                map[x, y].type = Plat.Type.Road;
            }
        } else if (target1.y < target2.y) {
            for (int i = 0; i <= n; i++) {
                y = (int)target1.y + i;
                map[x, y].type = Plat.Type.Road;
            }
        }
    }
    public Vector2 Abs(Vector2 vector) {
        return new Vector2((int)vector.x, (int)vector.y);
    }
    private Vector2 GenerateStartStreetPos() {
        contador = 0;
        bool top = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        Vector2 pos;
        if (top) {
            Debug.Log(top);
            pos = new Vector2(UnityEngine.Random.Range(0, width), heigth - 1);
        } else {
            pos = new Vector2(0, UnityEngine.Random.Range(0, heigth));
        }
        return pos;
    }
    private Vector2 GenerateEndStreetPos() {
        bool bot = true;
            bot = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        Vector2 pos;
        if (bot) {
            pos = new Vector2(UnityEngine.Random.Range(0, width), heigth - 1);
        } else {
            pos = new Vector2(width - 1, UnityEngine.Random.Range(0, heigth));
        }
        if ((pos.x == roadStart.x || pos.y == roadStart.y) && contador <= 500) {
            contador++;
            Debug.LogError("Igual " + pos + " " + contador);
            pos = GenerateEndStreetPos();
        }
        
        return pos;
    }

    public static List<T> DesordenarLista<T>(List<T> input) {
        List<T> arr = input;
        List<T> arrDes = new List<T>();

        System.Random randNum = new System.Random();
        while (arr.Count > 0) {
            int val = randNum.Next(0, arr.Count - 1);
            arrDes.Add(arr[val]);
            arr.RemoveAt(val);
        }

        return arrDes;
    }
}
