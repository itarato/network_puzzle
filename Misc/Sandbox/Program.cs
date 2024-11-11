using Sandbox;

Console.WriteLine("Level Testing Sandbox\n\n");

Level level = LevelGenerator.Generate(16, 8, false, 0);
Utility.DumpLevel(level);
