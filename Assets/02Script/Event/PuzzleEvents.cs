
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

    public struct SO_StartChase { }


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

    // Horror맵 창문 로직
    public struct H_CheckWindow 
    {
        public int windowID;
        public HorrorFloor floor;
        public H_CheckWindow(int windowID, HorrorFloor floor) { 
            this.windowID = windowID;
            this.floor = floor;
        }
    }


    // Horror맵 창문 모두 확인
    public struct H_GetPassword
    {
        public HorrorFloor floor;
        public H_GetPassword(HorrorFloor floor) { 
            this.floor = floor;
        }
    }

    // Horror : 층 자물쇠 퍼즐 완료
    public struct H_Unlock
    {
        public HorrorFloor floor;
        public H_Unlock(HorrorFloor floor) {
            this.floor = floor;
        }
    }

    // Horror : 다음 층 이동 (학교전용)
    public struct ToNextFloor
    {
        public HorrorFloor floor;   // 요청한 현재 층
        public ToNextFloor(HorrorFloor floor) {
            this.floor = floor;
        }
    }
}
