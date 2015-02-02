public static class LevelUpRules
{
    // Conveyor belt speed rules
    public static int levelsPerBeltSpeedIncrease = 4;
    public static float maxConveyorBeltSpeed = 10;
    public static float conveyorBeltSpeedModifier = 0.25f;

    // Truck capacity rules
    public static int levelsPerTruckCapacityIncrease = 4;
    public static int maxTruckCapacity = int.MaxValue;

    // Spawn rate rules
    public static int minSpawnRate = 2;
    public static float spawnRateModifier = 0.125f;

    // Maximum spawns per burst rules
    public static int levelsPerMaximumSpawnsPerBurstIncrease = 2;
    public static int maxMaximumSpawnsPerBurst = int.MaxValue;
    public static int maximumSpawnsPerBurstModifier = 1;

    // Minimum spawns per burst rules
    public static int levelsPerMinimumSpawnsPerBurstIncrease = 4;
    public static int maxMinimumSpawnsPerBurst = int.MaxValue;
    public static int minimumSpawnsPerBurstModifier = 1;

    // Spawn burst gap rules
    public static int minSpawnBurstGap = 5;
    public static float spawnBurstGapModifier = 0.5f;  

    // Randomiser rules
    public static int levelsPerRandomiserToggle = 2;
}