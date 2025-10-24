
public static class PuzzleEvents
{

    // HappyOneScene

    public struct HO_GetKey { }
    
    public struct HO_AppearEnemy { }

    public struct PassedQuiz
    {
        public string npcID;
        public PassedQuiz(string npcID) { 
            this.npcID = npcID;
        }
    }
}
