using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox {
    public class Utility {
        public static void DumpLevel(Level level) {
            Dictionary<int, string> pipesMap = new Dictionary<int, string>();
            pipesMap[0b0000] = " ";

            pipesMap[0b1000] = "<";
            pipesMap[0b0100] = "v";
            pipesMap[0b0010] = ">";
            pipesMap[0b0001] = "^";

            pipesMap[0b1100] = "┐";
            pipesMap[0b0110] = "┌";
            pipesMap[0b0011] = "└";
            pipesMap[0b1010] = "-";
            pipesMap[0b0101] = "|";
            pipesMap[0b1001] = "┘";

            pipesMap[0b1110] = "┬";
            pipesMap[0b1101] = "┤";
            pipesMap[0b1011] = "┴";
            pipesMap[0b0111] = "├";

            pipesMap[0b1111] = "+";

            for (int y = 0; y < level.height; y++) {
                for (int x = 0; x < level.width; x++) {
                    int mask = 0;
                    int cellIndex = y * level.width + x;

                    for (int i = 0; i < 4; i++) {
                        if (level.cells[cellIndex].paths[i]) mask |= (1 << i);
                    }
                    Console.Write(pipesMap[mask]);
                }
                Console.WriteLine();
            }
        }
    }
}
