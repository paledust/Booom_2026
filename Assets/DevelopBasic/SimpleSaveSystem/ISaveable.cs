namespace SimpleSaveSystem{
    public interface ISaveable
    {
        System.Guid guid{get;}
        void RestoreState(SaveData state);
        void CaptureState(ref SaveData saveData);
    }
}