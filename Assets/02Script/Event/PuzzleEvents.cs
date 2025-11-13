
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

    public struct SO_FixPin
    {
        //public int pinIdx;
        //public SO_FixPin(int pinIdx) { 
        //    this.pinIdx = pinIdx;
        //}
    }

    // CustomItem¿« ∆€¡Ò¿Ã ≥°≥µ¿ª∂ß
    public struct CutOff
    {
        public int itemID;
        public CutOff(int itemID) { 
            this.itemID = itemID;
        }
    }
}
