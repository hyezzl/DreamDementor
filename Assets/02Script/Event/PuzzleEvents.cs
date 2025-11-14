
public static class PuzzleEvents
{

    // HappyOneScene

    public struct HO_GetKey
    {
        public string npcID;
        public HO_GetKey(string npcID) { 
            this.npcID = npcID;
        }
    }
    
    public struct HO_AppearEnemy 
    {
        public string npcID;
        public HO_AppearEnemy(string npcID) { 
            this.npcID = npcID;
        }
    }

    public struct PassedQuiz
    {
        public string npcID;
        public PassedQuiz(string npcID) { 
            this.npcID = npcID;
        }
    }

    public struct SO_ConnectLine
    {
        public LineColor color;
        public SO_ConnectLine(LineColor color) { 
            this.color = color;
        }
    }

    public struct SO_FixPin { }
    //{
        //public int pinIdx;
        //public SO_FixPin(int pinIdx) { 
        //    this.pinIdx = pinIdx;
        //}
    //}

    // CustomItem의 퍼즐이 끝났을때 (더이상 상호작용 하지 못하도록 함)
    public struct CutOff
    {
        public int itemID;
        public CutOff(int itemID) { 
            this.itemID = itemID;
        }
    }

    public struct PassMinigame
    {
        public PuzzleType puzzle;
        public PassMinigame(PuzzleType puzzle) { 
            this.puzzle = puzzle;
        }
    }
}
