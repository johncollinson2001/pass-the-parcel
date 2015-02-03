public static class Defaults
{
    // Game play defaults
    public static class Game
    {
        public static int startingLives = 3;
        public static int levelUpPauseLength = 10;
        public static int levelUpCountdownLength = 3;
        public static int lifeLostPauseLength = 10;
        public static int lifeLostCountdownLength = 3;
    }

    // Game start level defaults
    public static class LevelStart
    {
        public static float conveyorBeltSpeed = 2;
        public static int truckCapacity = 2;
        public static float spawnRate = 8;
        public static int minimumSpawnsPerBurst = 2;
        public static int maximumSpawnsPerBurst = 3;
        public static float spawnBurstGap = 30;
        public static bool useSpawnRateRandomiser = false;
    }

    // Parcel spawner defaults
    public static class ParcelSpawner
    {
        public static float reduceSpawnWaveGapModifier = 0.5f;
        public static int spawnWaveAlertTime = 3;
        public static int startSpawningBuffer = 5;
        public static float spawnAlertBlinkSpeed = 0.5f;
    }

    // Worker defaults
    public static class Worker
    {
        public static float jumpySkippyness = 0.3f;
    }
}