//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;

//public class ItemDatabaseManager : Singleton<ItemDatabaseManager>, IItemDatabase
//{
//    [Header("Data SO 연결")]
//    [SerializeField] private ObjectTable SOobject; // Object SO연결
//    [SerializeField] private EventTable SOevent; // event SO 연결


//    // 게임에 존재하는 모든 아이템 정보 Dictionary로 관리
//    private Dictionary<int, PickableData> pickableDict = new();
//    private Dictionary<int, InteractableData> interactableDict = new();
//    private Dictionary<int, InspectableData> inspectableDict = new();
//    private Dictionary<int, ReadableData> readableDict = new();

//    // 이벤트 정보 (같은 이벤트ID를 가진 행이 많으므로, List로 관리)
//    private Dictionary<string, List<NarrationData>> narrationDict = new();
//    private Dictionary<string, List<CutsceneData>> cutsceneDict = new();
//    private Dictionary<string, List<ConversationData>> conversationDict = new();
//    private Dictionary<string, ChoiceData> choiceDict = new();

//    private void Awake()
//    {
//        InitDict(); // 아이템 정보 딕셔너리에 넣기
//    }

//    // 아이템 정보 딕셔너리에 넣기
//    private void InitDict()
//    {
//        // 1. Pickable
//        foreach (var item in SOobject.Pickable)
//        {
//            PickableData data = new PickableData
//            {
//                itemID = item.ItemID,
//                itemName = item.ItemName,
//                type = ItemType.Pickable,
//                description = item.Description,
//                reply = item.Reply,
//                icon = null,  // Adressable로 비동기 로드 예정
//                pairID = item.PairID,
//            };
//            pickableDict[item.ItemID] = data;

//            //아이콘 비동기 로드
//            Addressables.LoadAssetAsync<Sprite>(item.IconName).Completed += handle =>
//            {
//                if (handle.Status == AsyncOperationStatus.Succeeded)
//                {
//                    data.icon = handle.Result;
//                }
//                else
//                {
//                    Debug.Log($"PickableItem ({item.ItemID} 아이콘 로드 실패)");
//                }
//            };

//        }

//        // 2. Interactable
//        foreach (var item in SOobject.Interactable)
//        {
//            InteractableData data = new InteractableData
//            {
//                itemID = item.ItemID,
//                itemName = item.ItemName,
//                type = ItemType.Interactable,
//                deactiveMSG = item.DeactiveMSG,
//                rejectMSG = item.RejectMSG,
//                pairID = item.PairID,
//            };
//            interactableDict[item.ItemID] = data;
//        }

//        // 3. Inspectable
//        foreach (var item in SOobject.Inspectable)
//        {
//            InspectableData data = new InspectableData
//            {
//                itemID = item.ItemID,
//                itemName = item.ItemName,
//                type = ItemType.Inspectable,
//                monologue = item.Monologue,
//            };
//            inspectableDict[item.ItemID] = data;
//        }

//        // 4. Readable
//        foreach (var item in SOobject.Readable)
//        {
//            ReadableData data = new ReadableData
//            {
//                itemID = item.ItemID,
//                itemName = item.ItemName,
//                type = ItemType.Readable,
//                monologue = item.Monologue,
//                narrative = item.Narrative,
//                reply = item.Reply,
//            };
//            readableDict[item.ItemID] = data;
//        }

//        // 5. Narration
//        foreach (var evt in SOevent.Narration)
//        {
//            NarrationData data = new NarrationData
//            {
//                eventID = evt.EventID,
//                eventName = evt.EventName,
//                type = EventType.Narration,
//                order = evt.Order,
//                text = evt.Text,
//            };
//            // eventID가 이미 있으면 리스트추가, 없으면 리스트 생성
//            if (!narrationDict.ContainsKey(evt.EventID))
//            {
//                narrationDict[evt.EventID] = new List<NarrationData>();
//            }
//            narrationDict[evt.EventID].Add(data);
//        }

//        // 6. Cutscene
//        foreach (var evt in SOevent.Cutscene)
//        {
//            CutsceneData data = new CutsceneData
//            {
//                eventID = evt.EventID,
//                eventName = evt.EventName,
//                type = EventType.Cutscene,
//                convID = evt.ConvID,  // 고유값
//                text = evt.Text,
//                nextConvID = evt.NextConvID,
//                speaker = System.Enum.Parse<Speaker>(evt.Speaker),
//            };
//            if (!cutsceneDict.ContainsKey(evt.EventID))
//            {
//                cutsceneDict[evt.EventID] = new List<CutsceneData>();
//            }
//            cutsceneDict[evt.EventID].Add(data);  // 같은 EventID 안에 List로 Add
//        }

//        // 7. Choice
//        foreach (var evt in SOevent.Choice)
//        {
//            ChoiceData data = new ChoiceData
//            {
//                choiceID = evt.ChoiceID,
//                texts = new List<string> {
//                    evt.Choice0,
//                    evt.Choice1,
//                    evt.Choice2,
//                }  // LINQ 로 만들기
//                .Where(x => !string.IsNullOrEmpty(x))
//                .ToList()
//            };
//            // 중복값 없음
//            choiceDict[evt.ChoiceID] = data;
//        }


//        // 8. Conversation
//        foreach (var evt in SOevent.Conversation)
//        {
//            // choiceID에 맞는 ChoiceData 미리 세팅
//            choiceDict.TryGetValue(evt.ChoiceID, out var choiceData); // ChoiceID에 맞는 Choice값들

//            ConversationData data = new ConversationData
//            {
//                eventID = evt.EventID,
//                eventName = evt.EventName,
//                type = EventType.Conversation,
//                convID = evt.ConvID,
//                text = evt.Text,
//                nextConvID = evt.NextConvID,
//                speaker = System.Enum.Parse<Speaker>(evt.Speaker),
//                choiceID = evt.ChoiceID,
//                emotion = (Emotion)evt.Emotion,
//                choices = string.IsNullOrEmpty(evt.ChoiceID) ? null : new ChoiceData
//                {
//                    choiceID = evt.ChoiceID,
//                    texts = choiceData.texts,
//                }
//            };
//        }
//    }



//    // 외부 호출 함수 (Getter)
//    public PickableData GetPickable(int itemID)
//    {
//        if (pickableDict.TryGetValue(itemID, out var data))
//        {
//            return data;
//        }
//        return null;
//    }

//    public InteractableData GetInteractable(int itemID)
//    {
//        if (interactableDict.TryGetValue(itemID, out var data))
//        {
//            return data;
//        }
//        return null;
//    }

//    public InspectableData GetInspectable(int itemID)
//    {
//        if (inspectableDict.TryGetValue(itemID, out var data))
//        {
//            return data;
//        }
//        return null;
//    }


//    public ReadableData GetReadable(int itemID)
//    {
//        if (readableDict.TryGetValue(itemID, out var data))
//        {
//            return data;
//        }
//        return null;
//    }

//    public List<NarrationData> GetNarration(string eventID)
//    {
//        if (narrationDict.TryGetValue(eventID, out var dataList))
//        {
//            return dataList;   // 리스트 반환
//        }
//        return null;
//    }

//    public List<CutsceneData> GetCutscene(string eventID)
//    {
//        if (cutsceneDict.TryGetValue(eventID, out var dataList))
//        {
//            return dataList;
//        }
//        return null;
//    }

//    public List<ConversationData> GetConversation(string eventID)
//    {
//        if (conversationDict.TryGetValue(eventID, out var dataList))
//        {
//            return dataList;
//        }
//        return null;

//    }

//    public ChoiceData GetChoice(string choiceID)
//    {
//        if (choiceDict.TryGetValue(choiceID, out var data))
//        {
//            return data;
//        }
//        return null;
//    }
//}
