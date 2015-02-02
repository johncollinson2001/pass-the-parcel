public class LevelModel {
    public int LevelNumber { get; set; }
    public float ConveyorBeltSpeed { get; set; }
    public int TruckCapacity { get; set; }
    public float SpawnRate { get; set; }
    public int MinimumSpawnsPerBurst { get; set; }
    public int MaximumSpawnsPerBurst { get; set; }
    public float SpawnBurstGap { get; set; }
    public bool UseSpawnRateRandomiser { get; set; }
}