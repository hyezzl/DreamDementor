using System.Collections.Generic;
using System.Diagnostics;

public static class UIEvents
{
    public struct OpenDialog
    {
        public string eventID;
        public Dictionary<int, DialogData> texts;

        public OpenDialog(string eventID, Dictionary<int, DialogData> texts)
        {
            this.eventID = eventID;
            this.texts = texts;
        }
    }

    public struct InteractDialog
    {
        public IActionItem item;

        public InteractDialog(IActionItem item)
        {
            this.item = item;
        }

    }

    // 선택지 발생
    public struct OccurSelection
    {
        public int idx;
        public ChoiceData choice;

        public OccurSelection(int idx, ChoiceData choice)
        {
            this.idx = idx;
            this.choice = choice;
        }
    }


    // 선택지 선택
    public struct MakeChoice
    {
        public string choiceID;
        public int selectIdx;
        public MakeChoice(string choiceID, int selectIdx)
        {
            this.choiceID = choiceID;
            this.selectIdx = selectIdx;
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
}
