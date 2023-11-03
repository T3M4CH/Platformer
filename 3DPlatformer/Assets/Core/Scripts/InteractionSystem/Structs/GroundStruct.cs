namespace Core.Scripts.InteractionSystem.Structs
{
    public struct GroundStruct
    {
        public GroundStruct(bool under, bool front, bool back)
        {
            Back = back;
            Under = under;
            Front = front;
        }
        
        public bool Under;
        public bool Front;
        public bool Back;
    }
}