using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plat : MonoBehaviour {
    public enum Type {
        School, House, Factory, Road, Empty
    }
    public Type type;

    Vector2 Pos;


    public void Set(int XPos, int YPos, Type type = Type.Empty) {
        this.Pos = new Vector2(XPos, YPos);
        this.type = type;
    }

    public void InstantiatePlat() {
        Invoke(nameof(InstaInstantiatePlat), Vector2.Distance( new Vector2(/*CityGenerator.Instance.width, CityGenerator.Instance.heigth*/0,0), Pos) * 0.05f);
    }
    public void InstaInstantiatePlat() {
        SoftClear();
        Instantiate(CityGenerator.Instance.PlatPrefabs[(int)type], transform);
    }
    public void Check() {
        if (type == Type.Empty) {
            List<Plat> platsAround = new List<Plat>();
            bool canHouse = false;
            for (int resX = -1; resX <= 1; resX++) {
                for (int resY = -1; resY <= 1; resY++) {
                    if ((((Pos.x + resX) >= 0) && ((Pos.x + resX) < CityGenerator.Instance.width))
                           &&
                           (((Pos.y + resY) >= 0) && ((Pos.y + resY) < CityGenerator.Instance.heigth))) {
                        Plat cellType = CityGenerator.Instance.map[(int)(Pos.x + resX), (int)(Pos.y + resY)];
                        if (cellType.type == Type.Factory) {
                            //Debug.Log(cellType.gameObject.name + " Factory");
                            return;
                        } else
                        if (cellType.type == Type.House) {
                            //Debug.Log(cellType.gameObject.name + " Casa");
                            canHouse = true;
                        } else
                        if (cellType.type == Type.School) {
                            // Debug.Log(cellType.gameObject.name + " Escuela");

                            canHouse = true;
                        } else if (cellType.type == Type.Empty) {
                            //   Debug.Log(cellType.gameObject.name + type.ToString());
                            platsAround.Add(cellType);
                        }

                    }
                }
            }
            if (canHouse) {
                type = Type.House;
                platsAround.ForEach(p => p.Check());
            }
        }
    }
    public void Clear() {
       // SoftClear();
        type = Type.Empty;
    }
    public void SoftClear() {
        if (transform.childCount > 0) {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
    public void Die() {
        Destroy(gameObject);
    }

    public bool CheckPosWithVector(Vector3 compare) {
        return Pos.x == compare.x && Pos.y == compare.y;
    }
    private void OnMouseDown() {
        Check();
        InstaInstantiatePlat();
    }
}
