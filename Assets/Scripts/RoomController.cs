using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Data")]
    public int currentlyInRoom = 1;
    public List<GameObject> roomsAccess = new List<GameObject>();
    public List<GameObject> visitedRooms = new List<GameObject>();
    [HideInInspector] public bool travellingBetweenRooms = false;
    [Header("Settings")]
    public List<GameObject> rooms = new List<GameObject>();
    [Header("Objects")]
    [SerializeField] GameObject _enemiesHolder;

    Dictionary<int, int[]> _roomGivesAccessTo = new Dictionary<int, int[]>();

    private void Awake() {
        _roomGivesAccessTo.Add(1, new int[] { 2, 3 }); /* Clearing room 1 gives access to room 2 and 3 */
        _roomGivesAccessTo.Add(2, new int[] { 1, 4 });
        _roomGivesAccessTo.Add(3, new int[] { 1 });
        _roomGivesAccessTo.Add(4, new int[] { 2 });
    }

    public void addAccessToRoom(GameObject room) {
        if (!rooms.Find(o => o == room)) {
            Debug.LogError("Room has not been added to the room list.\nPlease add the room you are trying to give access to.");
            return;
        };
        if (!rooms.Find(o => int.Parse(o.name.Split("_")[1]) == currentlyInRoom)) {
            Debug.LogError("The current room you are in does not exist in the rooms list.\nPlease add the room you are currently occupying.");
            return;
        };

        if (!roomsAccess.Find(o => o == room)) roomsAccess.Add(room);
        
        GameObject _currentRoomObject = rooms.Find(o => int.Parse(o.name.Split("_")[1]) == currentlyInRoom);
        for (int i = 0; i < _currentRoomObject.transform.childCount; i++)
        {
            if (!_currentRoomObject.transform.GetChild(i).GetComponent<RoomLink>()) continue;
            if (_currentRoomObject.transform.GetChild(i).GetComponent<RoomLink>().linksTo != room) continue;
            _currentRoomObject.transform.GetChild(i).GetComponent<RoomLink>().hasAccess = true;
            _currentRoomObject.transform.GetChild(i).GetComponent<BoxCollider2D>().isTrigger = true;
            _currentRoomObject.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
        }
    }

    int _lastAttemptedUnlockIndex = -1;
    private void FixedUpdate() {
        if (_enemiesHolder.transform.childCount != 0) return;
        if (_lastAttemptedUnlockIndex == currentlyInRoom) return;
        if (!_roomGivesAccessTo.ContainsKey(currentlyInRoom)) return;

        Debug.Log("UNLOCK");
        foreach (int roomNumber in _roomGivesAccessTo[currentlyInRoom])
        {
            if (!rooms.Find(o => int.Parse(o.name.Split("_")[1]) == roomNumber)) continue;
            addAccessToRoom(rooms.Find(o => int.Parse(o.name.Split("_")[1]) == roomNumber));
            print("GIVE ACCESS TO: " + roomNumber);
        }

        _lastAttemptedUnlockIndex = currentlyInRoom;
    }
}
