
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
}
