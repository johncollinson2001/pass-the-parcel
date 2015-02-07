using UnityEngine;
public static class Constants
{
    // Game play defaults
    public static class Game
    {
        public static int startingLives = 3;
        public static int levelUpPauseLength = 10;
        public static int levelUpCountdownLength = 10;
        public static int lifeLostPauseLength = 10;
        public static int lifeLostCountdownLength = 10;
    }

    // Game start level defaults
    public static class LevelStartDefaults
    {
        public static float conveyorBeltSpeed = 2;
        public static int truckCapacity = 3;
        public static float spawnRate = 7;
        public static int minimumSpawnsPerBurst = 2;
        public static int maximumSpawnsPerBurst = 3;
        public static float spawnBurstGap = 25;
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

    // Truck defaults
    public static class Truck
    {
        public static int secondsToStartTruck = 2;
        public static float reverseSpeedModifier = 1.5f;
    }

    // Truck defaults
    public static class Parcel
    {
        public static Color flashHighlightColor = new Color(255f, 0f, 0f);
        public static float flashSpeed = 0.5f;
    }

    // Conveyor belt
    public static class ConveyorBelt
    {
        public static float aboutToDropParcelBuffer = 1.5f;
    }

    // Score constants
    public static class Scores
    {
        public static int passedParcel = 10;
        public static int parcelLoaded = 25;
        public static int levelUp = 1000;
    }
}