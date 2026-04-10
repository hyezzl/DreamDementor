using System.Collections.Generic;
using System.Diagnostics;

public static class UIEvents
{
    public struct OpenDialog
    {
        public string eventID;
        public Dictionary<int, DialogData> texts;
        public GameMode afterMode;

        public OpenDialog(string eventID, Dictionary<int, DialogData> texts, GameMode afterMode = GameMode.None)
        {
            this.eventID = eventID;
            this.texts = texts;
            this.afterMode = afterMode;
        }
    }

    public struct OpenNpcDialog
    {
        public string npcID;
        public string npcEventID;
        public Dictionary<int, NPCDialogData> texts;
        public GameMode afterMode;
        public OpenNpcDialog(string npcID, string npcEventID, Dictionary<int, NPCDialogData> texts, GameMode afterMode = GameMode.None)
        {
            this.npcID = npcID;
            this.npcEventID = npcEventID;
            this.texts = texts;
            this.afterMode = afterMode;
        }
    }

    public struct OpenNpcReDialog
    {
        public string npcID;
        public NPCReDialogData data;
        public OpenNpcReDialog(string npcID, NPCReDialogData data) { 
            this.npcID = npcID;
            this.data = data;
        }
    }


    // 선택지 발생
    public struct OccurSelection
    {
        public int idx;
        public ChoiceData choice;
        public bool isNpc;

        public OccurSelection(int idx, ChoiceData choice, bool isNpc)
        {
            this.idx = idx;
            this.choice = choice;
            this.isNpc = isNpc;
        }
    }


    // 선택지 선택
    public struct MakeChoice
    {
        public string choiceID;
        public int selectIdx;
        public bool isNpc;
        public ChoiceData data;
        public MakeChoice(string choiceID, int selectIdx, bool isNpc, ChoiceData data)
        {
            this.choiceID = choiceID;
            this.selectIdx = selectIdx;
            this.isNpc = isNpc;
            this.data = data;
        }
    }


    // 미니 선택지 (Y/N) 발생
    public struct OccurYN { }

    // 미니 선택지 선택
    public struct SelectYN
    {
        public bool YN;
        public SelectYN(bool YN) { 
            this.YN = YN;
        }
    }


    // 대화끝 이벤트
    public struct EndDialog
    {
        public string eventID;
        public EndDialog(string eventID) {
            this.eventID = eventID;
        }
    }

    public struct EndNpcDialog
    {
        public string npcID;
        public string npcEventID;
        public EndNpcDialog(string npcID, string npcEventID = null) { 
            this.npcID = npcID;
            this.npcEventID = npcEventID;
        }
    }


    // 모놀로그 대화창 (아이템)
    public struct ItemMonologue
    {
        public int itemID;
        public string text;
        public ItemMonologue(int itemID, string text) {
            this.text = text;
            this.itemID = itemID;
        }
    }

    // 모놀로그 대화창 (대화)
    public struct OpenMonologue
    {
        public string eventID;
        public Dictionary<int, DialogData> texts;
        public OpenMonologue(string eventID, Dictionary<int, DialogData> texts) {
            this.eventID = eventID;
            this.texts = texts;
        }
    }


    // 아이템 슬롯 클릭
    public struct SlotClicked
    {
        public ItemInstance item;
        public InventorySlot slot;
        public SlotClicked(ItemInstance item, InventorySlot slot) { 
            this.item = item;
            this.slot = slot;
        }
    }


    // 인벤토리 새로고침
    public struct InventoryChanged { }

    // 인벤토리 열림
    public struct OpenInventory { }


    public struct SceneCover {
        public string illID;
        public SceneCover(string illID) { 
            this.illID = illID;
        }
    }

    public struct SceneDiscover { }

    // 대화창 강제 닫기
    public struct CloseDialog {
        public bool isNpc;
        public CloseDialog(bool isNpc) { 
            this.isNpc = isNpc;
        }
    }


    // 퍼즐 UI 열기
    public struct OpenPuzzle
    {
        public PuzzleType puzzle;
        public OpenPuzzle(PuzzleType puzzle) { 
            this.puzzle = puzzle;
        }
    }

    // 게임종료 경고창 띄우기
    public struct WarningShutdown { }

    // 범용 이벤트 PuzzleUI를 닫을때
    public struct UIPuzzleComplete { }
}
