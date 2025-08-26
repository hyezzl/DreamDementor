using System.Collections.Generic;

public static class UIEvents
{
    public struct OpenDialog
    {
        public Dictionary<int, DialogData> texts;

        public OpenDialog(Dictionary<int, DialogData> texts)
        {
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
        public int idx; // 버튼의 개수를 넘겨줘야하나...?

        public OccurSelection(int idx)
        {
            this.idx = idx;
        }
    }
}
