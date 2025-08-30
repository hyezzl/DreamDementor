using System.Collections.Generic;

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

    // 대화끝 이벤트
    public struct EndDialog
    {
        public string eventID;
        public EndDialog(string eventID) { 
            this.eventID = eventID;
        }
    }
}
