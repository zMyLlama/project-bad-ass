using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RoomLink : MonoBehaviour
{
    public GameObject linksTo;
    public Vector2 goTo;
    [ReadOnly] public bool hasAccess = false;

    GameObject _confinerObject;
    RoomController _roomController;

    private void Awake() {
        _confinerObject = GameObject.FindGameObjectWithTag("Confiner");
        _roomController = GameObject.FindGameObjectWithTag("Player").GetComponent<RoomController>();
    }

    IEnumerator handleRoomSwitch() {
        _roomController.travellingBetweenRooms = true;
        _confinerObject.transform.DOMove(linksTo.transform.position, 1f);

        DOTween.Kill("Dash");
        GameObject.FindGameObjectWithTag("Player").transform.position = goTo;
        yield return new WaitForSeconds(0.5f);

        if(!_roomController.visitedRooms.Find(o => o == linksTo)) {
            if (linksTo.transform.GetChild(0).gameObject.name == "EnemySpawnLocations") {
                for (int i = 0; i < linksTo.transform.GetChild(0).childCount; i++)
                {
                    linksTo.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        _roomController.currentlyInRoom = int.Parse(linksTo.name.Split("_")[1]);
        _roomController.visitedRooms.Add(linksTo);
        _roomController.travellingBetweenRooms = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag != "Player") return;
        if (_roomController.travellingBetweenRooms) return;
        if (!hasAccess) return;
        if (transform.parent.name.Split("_")[1] != _roomController.currentlyInRoom.ToString()) return;

        StartCoroutine("handleRoomSwitch");
    }
}
