using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public enum ItemType
{
    Pickable,
    Eatable,
    Interactable,
    Inspectable,
    //Readable,
    Note,
}

public class DatabaseManager : Singleton<DatabaseManager>, IDatabase
{
    [Header("Data SO 연결")]
    [SerializeField] private ObjectTable SOobject;  // Object SO연결
    [SerializeField] private EventTable SOevent;    // event SO 연결
    [SerializeField] private NpcTable SOnpc;         // npc SO 연결


    // 게임에 존재하는 모든 아이템 정보 Dictionary로 관리
    private Dictionary<int, PickableData> pickableDict = new();
    private Dictionary<int, EatableData> eatableDict = new();
    private Dictionary<int, InteractableData> interactableDict = new();
    private Dictionary<int, InspectableData> inspectableDict = new();
    //private Dictionary<int, ReadableData> readableDict = new();
    private Dictionary<int, NoteData> noteDict = new();

    // 이벤트 정보 (하나의 이벤트에 속해있는 Text의 집합은 List형태로 정의)
    private Dictionary<string, EventData> eventDict = new();
    private Dictionary<string, List<NarrationData>> narrationDict = new();
    //private Dictionary<string, Dictionary<int, DialogData>> dialogDict = new();
    private Dictionary<string, Dictionary<string, Dictionary<int, DialogData>>> dialogDict = new();
    private Dictionary<string, ChoiceData> choiceDict = new();

    // NPC 정보
    private Dictionary<string, NPCData> npcDict = new();
    //private Dictionary<string, Dictionary<int, NPCDialogData>> npcDialogDict = new();
    private Dictionary<string, Dictionary<string, Dictionary<int, NPCDialogData>>> npcDialogDict = new();
    private Dictionary<string, NPCReDialogData> npcRedialogDict = new();


    protected override void DoAwake()
    {
        base.DoAwake();
        InitDict(); // 아이템 정보 딕셔너리에 넣기
    }

    // 아이템 정보 딕셔너리에 넣기
    private void InitDict()
    {
        // 1. Pickable
        foreach (var item in SOobject.Pickable)
        {
            PickableData data = new PickableData
            {
                itemID = item.ItemID,
                itemName = item.ItemName,
                type = ItemType.Pickable,
                description = item.Description,
                reply = item.Reply,
                icon = null,  // Adressable로 비동기 로드 예정
                pairID = item.PairID,
                subsist = item.Subsist == 1 ? true : false,
            };
            pickableDict[item.ItemID] = data;

            //아이콘 비동기 로드
            Addressables.LoadAssetAsync<Sprite>(item.IconName).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    data.icon = handle.Result;
                }
                else
                {
                    Debug.Log($"PickableItem ({item.ItemID} 아이콘 로드 실패)");
                }
            };
        }

        // 2 . Eatable
        foreach (var item in SOobject.Eatable) {
            EatableData data = new EatableData
            {
                itemID = item.ItemID,
                itemName = item.ItemName,
                type = ItemType.Eatable,
                description = item.Description,
                reply = item.Reply,
                icon = null,
                mental = item.Mental,
            };
            eatableDict[item.ItemID] = data;

            //아이콘 비동기 로드
            Addressables.LoadAssetAsync<Sprite>(item.IconName).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    data.icon = handle.Result;
                }
                else
                {
                    Debug.Log($"EatableItem ({item.ItemID} 아이콘 로드 실패)");
                }
            };
        }


        // 3. Interactable
        foreach (var item in SOobject.Interactable)
        {
            InteractableData data = new InteractableData
            {
                itemID = item.ItemID,
                itemName = item.ItemName,
                type = ItemType.Interactable,
                monologue = item.Monologue,
                activeMSG = item.ActiveMSG,
                rejectMSG = item.RejectMSG,
                pairID = item.PairID,
                hnum = item.HNum
            };
            interactableDict[item.ItemID] = data;
        }

        // 4. Inspectable
        foreach (var item in SOobject.Inspectable)
        {
            InspectableData data = new InspectableData
            {
                itemID = item.ItemID,
                itemName = item.ItemName,
                type = ItemType.Inspectable,
                monologue = item.Monologue,
            };
            inspectableDict[item.ItemID] = data;
        }

        // 4. Readable
        //foreach (var item in SOobject.Readable)
        //{
        //    ReadableData data = new ReadableData
        //    {
        //        itemID = item.ItemID,
        //        itemName = item.ItemName,
        //        type = ItemType.Readable,
        //        monologue = item.Monologue,
        //        narrative = item.Narrative,
        //        reply = item.Reply,
        //    };
        //    readableDict[item.ItemID] = data;
        //}

        // 4. Note
        foreach (var item in SOobject.Note)
        {
            NoteData data = new NoteData
            {
                itemID = item.ItemID,
                itemName = item.ItemName,
                type = ItemType.Note,
                text = item.Text,
                reply = item.Reply,
            };
            noteDict[item.ItemID] = data;
        }


        // 5. Event
        foreach (var evt in SOevent.Event) {
            EventType curType;
            if (!System.Enum.TryParse(evt.EventType, out curType)) { 
                Debug.Log("EventType변환 실패");
                curType = EventType.Conversation;
            } 

            EventData data = new EventData
            {
                eventID = evt.EventID,
                eventName = evt.EventName,
                type = curType,
            };
            eventDict[evt.EventID] = data;
        }

        // 6. Narration
        foreach (var evt in SOevent.Narration)
        {
            NarrationData data = new NarrationData
            {
                eventID = evt.EventID,
                order = evt.Order,
                text = evt.Text,
            };
            if (!narrationDict.ContainsKey(evt.EventID))
            {
                narrationDict[evt.EventID] = new List<NarrationData>();
            }
            narrationDict[evt.EventID].Add(data);
        }

        // 7. Choice
        foreach (var evt in SOevent.Choice)
        {
            ChoiceData data = new ChoiceData
            {
                choiceID = evt.ChoiceID,
                texts = new List<string> {
                    evt.Choice0,
                    evt.Choice1,
                    evt.Choice2,
                }  // LINQ 로 만들기
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList(),
                scores = new List<int> {
                    evt.Score0,
                    evt.Score1,
                    evt.Score2,
                },
                continueIDs = new List<string> { 
                    evt.Continue0,
                    evt.Continue1,
                    evt.Continue2,
                },
            };
            choiceDict[evt.ChoiceID] = data;
        }


        // 8. Dialog
        foreach (var evt in SOevent.Dialog)
        {
            // choiceID에 맞는 ChoiceData 미리 세팅
            ChoiceData choiceData = null;
            if (!string.IsNullOrEmpty(evt.ChoiceID)) {
                choiceDict.TryGetValue(evt.ChoiceID, out choiceData);
            }

            DialogData data = new DialogData
            {
                eventDetailID = evt.EventDetailID,
                logID = evt.LogID,
                dialog = evt.Dialog,
                nextID = evt.NextID,
                speaker = System.Enum.Parse<Speaker>(evt.Speaker),
                speakerName = evt.SpeakerName,
                choiceID = evt.ChoiceID,
                textbox = System.Enum.Parse<Textbox>(evt.Textbox),
                emotion = evt.Emotion,
                choices = choiceData,
            };
            // 3중 딕셔너리
            if (!dialogDict.ContainsKey(evt.EventID)) {
                dialogDict[evt.EventID] = new Dictionary<string, Dictionary<int, DialogData>>();
            }
            if (!dialogDict[evt.EventID].ContainsKey(evt.EventDetailID)) {
                dialogDict[evt.EventID][evt.EventDetailID] = new Dictionary<int, DialogData>();
            }
            if (!dialogDict[evt.EventID][evt.EventDetailID].ContainsKey(evt.LogID))
            {
                dialogDict[evt.EventID][evt.EventDetailID][evt.LogID] = data;
            }
            else {
                Debug.Log($"{evt.LogID} : 이미 존재하는 대화");
            }

        }

        // NPC 정보 생략

        // 9. NPC Dialog
        foreach (var npc in SOnpc.NpcDialog) {
            // choiceID에 맞는 ChoiceData 미리 세팅
            ChoiceData choiceData = null;
            string choiceID = npc.ChoiceID?.Trim();
            if (!string.IsNullOrEmpty(choiceID))
            {
                choiceDict.TryGetValue(choiceID, out choiceData);
            }

            NPCDialogData data = new NPCDialogData
            {
                npcID = npc.NpcID,
                npcEventID = npc.NpcEventID,
                logID = npc.LogID,
                dialog = npc.Dialog,
                nextID = npc.NextID,
                speaker = System.Enum.Parse<Speaker>(npc.Speaker),
                speakerName = npc.SpeakerName,
                choiceID = npc.ChoiceID,
                textbox = System.Enum.Parse<Textbox>(npc.Textbox),
                emotion = npc.Emotion,
                choices = choiceData
            };

            // 삼중 딕셔너리 세팅
            if(!npcDialogDict.ContainsKey(npc.NpcID))
            {
                //npcDialogDict[npc.NpcID] = new Dictionary<int, NPCDialogData>();
                npcDialogDict[npc.NpcID] = new Dictionary<string, Dictionary<int, NPCDialogData>>();
            }
            if (!npcDialogDict[npc.NpcID].ContainsKey(npc.NpcEventID)) 
            {
                npcDialogDict[npc.NpcID][npc.NpcEventID] = new Dictionary<int, NPCDialogData>();
            }
            // logID가 없으면 생성
            if (!npcDialogDict[npc.NpcID][npc.NpcEventID].ContainsKey(npc.LogID))
            {
                npcDialogDict[npc.NpcID][npc.NpcEventID].Add(npc.LogID, data);
            }
            else
            {
                Debug.Log($"{npc.LogID} : 이미 존재하는 대화");
            }
        }

        // 10. ReDialog
        foreach (var npc in SOnpc.NpcReDialog)
        {
            NPCReDialogData data = new NPCReDialogData
            {
                npcID = npc.NpcID,
                dialog = npc.Dialog,
                speaker = System.Enum.Parse<Speaker>(npc.Speaker),
                speakerName = npc.SpeakerName,
                textbox = System.Enum.Parse<Textbox>(npc.Textbox),
                emotion = npc.Emotion
            };
            npcRedialogDict[npc.NpcID] = data;
        }
    }








    // 외부 호출 함수 (Getter)
    public PickableData GetPickable(int itemID)
    {
        if (pickableDict.TryGetValue(itemID, out var data))
        {
            return data;
        }
        return null;
    }

    public EatableData GetEatable(int itemID) {
        if (eatableDict.TryGetValue(itemID, out var data)) {
            return data;
        }
        return null;
    }


    public InteractableData GetInteractable(int itemID)
    {
        if (interactableDict.TryGetValue(itemID, out var data))
        {
            return data;
        }
        return null;
    }

    public InspectableData GetInspectable(int itemID)
    {
        if (inspectableDict.TryGetValue(itemID, out var data))
        {
            return data;
        }
        return null;
    }


    //public ReadableData GetReadable(int itemID)
    //{
    //    if (readableDict.TryGetValue(itemID, out var data))
    //    {
    //        return data;
    //    }
    //    return null;
    //}


    public NoteData GetNote(int itemID) {
        if (noteDict.TryGetValue(itemID, out var data)) {
            return data;
        }
        return null;
    }


    public EventData GetEventData(string eventID) {
        if (eventDict.TryGetValue(eventID, out var data)) {
            return data;
        }
        return null;
    }

    public List<NarrationData> GetNarration(string eventID) {
        if (narrationDict.TryGetValue(eventID, out var dataList)) {
            return dataList;
        }
        return null;
    }

    public Dictionary<string, Dictionary<int, DialogData>> GetDialogEvent(string eventID)
    {
        if (dialogDict.TryGetValue(eventID, out var dataDict))
        {
            return dataDict;
        }
        return null;
    }

    public Dictionary<int, DialogData> GetDialog(string eventID, string eventDetailID) {
        if (dialogDict.TryGetValue(eventID, out var allDict)) {
            if (allDict.TryGetValue(eventDetailID, out var dataDict)) {
                return dataDict;
            }
        }
        return null;
    }
    

    public ChoiceData GetChoice(string choiceID)
    {
        if (choiceDict.TryGetValue(choiceID, out var data))
        {
            return data;
        }
        return null;
    }

    public Dictionary<string, Dictionary<int, NPCDialogData>> GetNpcEvent(string npcID) {
        if (npcDialogDict.TryGetValue(npcID, out var dataDict)) {
            return dataDict;
        }
        return null;
    }

    public Dictionary<int, NPCDialogData> GetNpcDialog(string npcID, string npcEventID) {
        if (npcDialogDict.TryGetValue(npcID, out var dataDict))
        {
            if (dataDict.TryGetValue(npcEventID, out var dialogDict)) { 
                return dialogDict;
            }
        }
        return null;
    }

    public NPCReDialogData GetNpcReDialog(string npcID){
        if (npcRedialogDict.TryGetValue(npcID, out var dataDict))
        {
            return dataDict;
        }
        return null;
    }
}
