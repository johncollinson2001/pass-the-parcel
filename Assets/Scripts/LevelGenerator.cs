using System;

public static class LevelGenerator
{
    public static LevelModel CreateStartingLevel()
    {
        // Return the starting level 
        return new LevelModel()
        {
            LevelNumber = 1,
            ConveyorBeltSpeed = LevelDefaults.conveyorBeltSpeed,
            TruckCapacity = LevelDefaults.truckCapacity,
            SpawnRate = LevelDefaults.spawnRate,
            MinimumSpawnsPerBurst = LevelDefaults.minimumSpawnsPerBurst,
            MaximumSpawnsPerBurst = LevelDefaults.maximumSpawnsPerBurst,
            SpawnBurstGap = LevelDefaults.spawnBurstGap,
            UseSpawnRateRandomiser = LevelDefaults.useSpawnRateRandomiser
        };
    }

    // Creates the next level
    public static LevelModel CreateNextLevel(LevelModel currentLevel)
    {
        // Copy the current level to the next level
        LevelModel nextLevel = new LevelModel()
        {
            LevelNumber = currentLevel.LevelNumber + 1,
            ConveyorBeltSpeed = GetConveyorBeltSpeedForNextLevel(currentLevel),
            TruckCapacity = GetTruckCapacityForNextLevel(currentLevel),
            SpawnRate = GetSpawnRateForNextLevel(currentLevel),
            MinimumSpawnsPerBurst = GetMinimumSpawnsPerBurstForNextLevel(currentLevel),
            MaximumSpawnsPerBurst = GetMaximumSpawnsPerBurstForNextLevel(currentLevel),
            SpawnBurstGap = GetSpawnBurstGapForNextLevel(currentLevel),
            UseSpawnRateRandomiser = GetUseSpawnRateRandomiserForNextLevel(currentLevel)
        };

        return nextLevel;
    }

    // Gets the use randomiser flag for the next level
    static bool GetUseSpawnRateRandomiserForNextLevel(LevelModel currentLevel)
    {
        if (currentLevel.LevelNumber % LevelUpRules.levelsPerRandomiserToggle == 0)
        {
            return !currentLevel.UseSpawnRateRandomiser;
        }
        else
        {
            return currentLevel.UseSpawnRateRandomiser;
        }
    }

    // Gets the conveyor belt speed for the next level
    static float GetConveyorBeltSpeedForNextLevel(LevelModel currentLevel)
    {
        // Check to see if the conveyor belt speed should be sped up
        if (currentLevel.LevelNumber % LevelUpRules.levelsPerBeltSpeedIncrease == 0
            && currentLevel.ConveyorBeltSpeed < LevelUpRules.maxConveyorBeltSpeed)
        {
            return currentLevel.ConveyorBeltSpeed + LevelUpRules.conveyorBeltSpeedModifier;
        }
        else
        {
            return currentLevel.ConveyorBeltSpeed;
        }
    }

    // Gets the truck capacity for the next level
    static int GetTruckCapacityForNextLevel(LevelModel currentLevel)
    {
        // Check to see if the truck capacity should be increased
        if (currentLevel.LevelNumber % LevelUpRules.levelsPerTruckCapacityIncrease == 0
            && currentLevel.TruckCapacity < LevelUpRules.maxTruckCapacity)
        {
            return GetNextFibonacciValue(currentLevel.TruckCapacity);
        }
        else
        {
            return currentLevel.TruckCapacity;
        }
    }

    // Gets the spawn rate for the next level
    static float GetSpawnRateForNextLevel(LevelModel currentLevel)
    {
        // Check to see if the spawn rate should be decreased
        if (currentLevel.SpawnRate > LevelUpRules.minSpawnRate)
        {
            // The logic below results in a saw like graph that decreases over time
            // it increases when the randomiser is first switched on and decreases otherwise

            // Check to see if this is the level that the randomiser is being introduced
            if (currentLevel.LevelNumber % LevelUpRules.levelsPerRandomiserToggle == 0
                && !currentLevel.UseSpawnRateRandomiser)
            {
                // Increase the spawn rate a little
                return currentLevel.SpawnRate + LevelUpRules.spawnRateModifier;
            }
            else
            {
                // Decrease the spawn rate a little
                return currentLevel.SpawnRate - LevelUpRules.spawnRateModifier;
            }
        }
        else
        {
            return currentLevel.SpawnRate;
        }
    }

    // Gets max spawns per burst for the next level
    static int GetMaximumSpawnsPerBurstForNextLevel(LevelModel currentLevel)
    {
        // Check to see if the lower bound should be increased
        if (currentLevel.LevelNumber % LevelUpRules.levelsPerMaximumSpawnsPerBurstIncrease == 0
            && currentLevel.MaximumSpawnsPerBurst < LevelUpRules.maxMaximumSpawnsPerBurst)
        {
            return currentLevel.MaximumSpawnsPerBurst + LevelUpRules.maximumSpawnsPerBurstModifier;
        }
        else
        {
            return currentLevel.MaximumSpawnsPerBurst;
        }
    }

    // Gets min spawns per burst for the next level
    static int GetMinimumSpawnsPerBurstForNextLevel(LevelModel currentLevel)
    {
        // Check to see if the upper bound should be increased
        if (currentLevel.LevelNumber % LevelUpRules.levelsPerMinimumSpawnsPerBurstIncrease == 0
            && currentLevel.MinimumSpawnsPerBurst < LevelUpRules.maxMinimumSpawnsPerBurst)
        {
            return currentLevel.MinimumSpawnsPerBurst + LevelUpRules.minimumSpawnsPerBurstModifier;
        }
        else
        {
            return currentLevel.MinimumSpawnsPerBurst;
        }
    }

    // Gets spawn burst gap for the next level
    static float GetSpawnBurstGapForNextLevel(LevelModel currentLevel)
    {
        // Check to see if the gap can be increased
        if (currentLevel.SpawnBurstGap > LevelUpRules.minSpawnBurstGap)
        {
            return currentLevel.SpawnBurstGap - LevelUpRules.spawnBurstGapModifier;
        }
        else
        {
            return currentLevel.SpawnBurstGap;
        }
    }

    // Gets the next fibonacci value for a given fibonacci value
    static int GetNextFibonacciValue(int currentFibonacciValue)
    {
        int nextFibonacciValue = 0;

        // Values used to work out the current and next fibonacci numbers
        // during the iteration
        int a = 0;
        int b = 1;

        // Loop until we find the next value
        for (int i = 0; i < int.MaxValue; i++)
        {
            // Work out current and next fibonacci values for this iteration
            int temp = a;
            a = b;
            b = temp + b;

            // If a (the current fibonacci value for this iteration) is greater than the current value passed in
            if (a > currentFibonacciValue)
            {
                // Then Get the next fibo value to a
                nextFibonacciValue = a;
                break;
            }
            // If a (the current fibonacci value for this iteration) is equal to the current value passed in
            else if (a == currentFibonacciValue)
            {
                // Then Get the next fibo value to b
                nextFibonacciValue = b;
                break;
            }
        }

        // If value is zero then we have not found a value
        if (nextFibonacciValue == 0)
        {
            throw new Exception("No fibonacci value found");
        }
        else
        {
            return nextFibonacciValue;
        }
    }
}