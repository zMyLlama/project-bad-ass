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
    [SerializeField] GameObject roomClearedFX;

    Dictionary<int, int[]> _roomGivesAccessTo = new Dictionary<int, int[]>();
    ShakeManager _shakeManager;

    private void Awake() {
        _shakeManager = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<ShakeManager>();

        _roomGivesAccessTo.Add(1, new int[] { 2, 3, 5, 6 }); /* Clearing room 1 gives access to room 2, 3, 5 and 6 */
        _roomGivesAccessTo.Add(2, new int[] { 1, 4, 17 });
        _roomGivesAccessTo.Add(3, new int[] { 1, 7, 9 });
        _roomGivesAccessTo.Add(4, new int[] { 2, 21 });
        _roomGivesAccessTo.Add(5, new int[] { 1, 22 });
        _roomGivesAccessTo.Add(6, new int[] { 1, 7, 23 });
        _roomGivesAccessTo.Add(7, new int[] { 3, 6, 8 });
        _roomGivesAccessTo.Add(8, new int[] { 7, 9 });
        _roomGivesAccessTo.Add(9, new int[] { 3, 8, 10 });
        _roomGivesAccessTo.Add(10, new int[] { 12, 11, 9 });
        _roomGivesAccessTo.Add(11, new int[] { 10 });
        _roomGivesAccessTo.Add(12, new int[] { 13, 10 });
        _roomGivesAccessTo.Add(13, new int[] { 14, 12, 15 });
        _roomGivesAccessTo.Add(14, new int[] { 13 });
        _roomGivesAccessTo.Add(15, new int[] { 13, 16 });
        _roomGivesAccessTo.Add(16, new int[] { 15, 18, 17 });
        _roomGivesAccessTo.Add(17, new int[] { 16, 2 });
        _roomGivesAccessTo.Add(18, new int[] { 16, 5 });
        _roomGivesAccessTo.Add(19, new int[] { 18, 20 });
        _roomGivesAccessTo.Add(20, new int[] { 19, 21 });
        _roomGivesAccessTo.Add(21, new int[] { 20, 22, 4 });
        _roomGivesAccessTo.Add(22, new int[] { 21, 5 });
        _roomGivesAccessTo.Add(23, new int[] { 6 });
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
    bool _clearedEffectPlayed = false;
    private void FixedUpdate() {
        if (_enemiesHolder.transform.childCount == 0 && !_clearedEffectPlayed) {
            _shakeManager.addShakeWithPriority(10, 2, 0.3f, 100);
            roomClearedFX.GetComponent<ParticleSystem>().Play();
            _clearedEffectPlayed = true;
        }
        if (_enemiesHolder.transform.childCount != 0) {
            _clearedEffectPlayed = false;
            return;
        };

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
