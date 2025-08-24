using System.Collections.Generic;

public static class UIEvents
{
    public struct OpenDialog
    {
        public List<string> texts;

        public OpenDialog(List<string> texts)
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

    public struct OpenSelectBox
    {
        public int idx; // 버튼의 개수를 넘겨줘야하나...?

        public OpenSelectBox(int idx)
        {
            this.idx = idx;
        }
    }
}
