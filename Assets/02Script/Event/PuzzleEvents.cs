
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

    // 코인 얻음 (2개얻으면 자판기 접근 가능)
    public struct SO_GetCoin { }

    // 자판기 접근 가능
    public struct SO_SetMachine { }

    // 자판기 Complete
    public struct SO_WorkVendingMachine { }

    // 코인 올바르게 넣음
    public struct SO_InsertCoin { }


    public struct SO_GetAllNumber { }


    // 슬픔1인칭 맵의 타임어택이 끝나고 술래잡기 종료
    public struct SO_EndChase { }



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

    // SO 타임어택
    public struct SO_TimeAttack { }
}
