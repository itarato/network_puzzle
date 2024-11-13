using Sandbox;

Console.WriteLine("Level Testing Sandbox\n\n");

Level level = LevelGenerator.Generate(4, 4, false, 0);
Utility.DumpLevel(level);
