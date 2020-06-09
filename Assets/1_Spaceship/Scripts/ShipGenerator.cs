using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGenerator : MonoBehaviour {
    public List<GameObject> LeftWings;
    public List<GameObject> RightWings;
    public List<GameObject> DorsalsWings;
    public List<GameObject> Engnes;
    public List<GameObject> Weapons;
    public List<Material> materials;
    public GameObject body;
    public Transform LeftWingPos, RightWingPos, DorsalPos, EnginePos;
    public float speed = 20;

    void Start() {
        GenerateShip();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateShip();
        }
        body.transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }

    private void GenerateShip() {
        Material mat = materials[Random.Range(0, materials.Count)];
        GameObject wp = Weapons[Random.Range(0, Weapons.Count)];
        int wings = Random.Range(0, LeftWings.Count);
        body.GetComponent<Renderer>().material = mat;
        GeneratePart(LeftWingPos, LeftWings, mat, wings, wp);
        GeneratePart(RightWingPos, RightWings, mat, wings, wp);
        if (Random.Range(0, 2) == 0)
            GeneratePart(DorsalPos, DorsalsWings, mat);
        else
            DestroyChild(DorsalPos);
        GeneratePart(EnginePos, Engnes, mat);
    }
    private void GeneratePart(Transform pos, List<GameObject> list, Material material, int index = -1, GameObject weapons = null) {
        DestroyChild(pos);
        GameObject obj;
        if (index == -1)
            obj = Instantiate(list[Random.Range(0, list.Count)], pos);
        else
            obj = Instantiate(list[index], pos);
        if (weapons) {
            DestroyChild(pos.GetChild(0), 1);
            Instantiate(weapons, obj.transform.GetChild(0).position, obj.transform.GetChild(0).rotation).transform.SetParent(obj.transform, true);

        }
        if (obj.GetComponent<Renderer>())
            obj.GetComponent<Renderer>().material = material;
    }

    private void DestroyChild(Transform pos, int childPos = 0) {
        if (pos.childCount > childPos) {
            Destroy(pos.GetChild(childPos).gameObject);
        }
    }
}
