using System;
using System.IO;

namespace Dominoes
{
    /* We have a game app that involves knocking over dominoes of different heights that are
     * arranged in a line. When dominoes fall, they may knock over other dominoes, which, as
     * you might expect, may in turn knock over even more dominoes.
     * 
     * See: dominoes.jpg
     * 
     * A player may knock over a domino either to the left or to the right. Let dominoHeights[x]
     * denote the height of the domino at position x. The effect of knocking over to the right a
     * domino at position x is that all dominoes at positions x + 1 to x + dominoHeights[x],
     * inclusive, are knocked over to the right as well. These dominoes may then knock over even
     * more dominoes until nothing else is in range to be knocked over. Similarly, the effect of
     * knocking over the same domino to the left is that all dominoes at positions x - 1 to
     * x - dominoHeights[x], inclusive, are knocked over to the left.
     * 
     * In the above diagram, we have five dominoes of different heights. If we knock over to the
     * right the red domino at position 2 (on the x-axis), which has height 4, it falls and knocks
     * over all the dominoes up to and including position 2 + 4 = 6. The purple and the green
     * dominoes are knocked over in the same direction in this process. The green domino then
     * knocks over the blue domino. The right-most domino is not affected by this cascade.
     * 
     * Note: Take care that if a domino at position 5 has a height of 2, it will still knock over
     * the domino at position 7 when it falls to the right (we assume the dominoes have some very
     * small thickness to them).
     * 
     * In order to build an AI that can do well in playing this game against a human, we want to
     * know, for each domino that could be initially knocked over, how far the cascade will
     * propagate. Define a domino's <em>right cascade distance</em> to be the absolute difference
     * between the initial position of the domino and the position furthest away that is touched
     * by the dominoes that fall in the cascade, when the initial domino is knocked over to the
     * right. Left cascade distance is defined the same way, but when the domino is pushed to the
     * left.
     * 
     * Example:
     *
     * In the above image, the right cascade distance of the red domino at position 2 is 6. This
     * is because the domino falling furthest right as a result of the cascade will be the blue
     * domino, falling to position 8. The absolute difference of 2 and 8 is 6. The left cascade
     * distance for the red domino is 4. Since there are no other dominoes to the left of it,
     * the cascade will be limited to the domino's own height.
     * 
     * Input:
     * 
     * The input should be read from standard input. The first line contains a single integer
     * denoting N, the length of the line of dominoes. Each domino will have a position between
     * 0 and N-1.
     * 
     * The second line contains N space-separated integers. Counting from zero, the i-th integer
     * denotes the height of the domino at position i, or 0 if there is no domino at that position.
     * All domino heights are between 1 and 2,147,483,647.
     * 
     * Test case set A (code correctness metric): 0 <= N <= 100
     * 
     * Test case set B (algorithmic problem solving metric): Your code will be run against
     * increasingly larger N, up to N <= 500000, testing the efficiency of your solution. Your
     * score will increase in proportion to the size of the test cases you can handle.
     *
     * Output:
     * 
     * The output should be written to standard output and consists of two lines. On the first line,
     * output N space-separated integers, where the i-th (counting from 0) integer denotes the right
     * cascade distance of the domino at position i, or 0 for positions that do not have a domino. On
     * the second line, output the same data for left cascade distances.
     *
     * Example: the case shown in the above diagram
     * 
     * Input:
     * 11
     * 0 0 4 1 0 2 0 1 0 0 3
     * 
     * Output:
     * 0 0 6 1 0 3 0 1 0 0 3
     * 0 0 4 5 0 7 0 1 0 0 4
     *

     * Explanation:
     * It was already discussed above why the right cascade distance for the domino at position 2 is 6.
     * For position 3, the domino is of height 1 and doesn't knock anything else over, hence the cascade
     * distance is 1. For position 5, you will knock over the domino at position 7, which will fall to
     * position 8. Abs(8-5) = 3. The remaining dominoes don't knock any others over, so their cascade
     * distances are equal to their heights.
     * 
     * The left cascade distances use similar logic, but reasoning about dominoes falling to the left.
     * 
     * Example 2:
     * 
     * Input:
     * 12
     * 2 3 0 0 0 0 0 6 10 1 0 2
     * 
     * Output:
     * 4 3 0 0 0 0 0 11 10 1 0 2  
     * 2 3 0 0 0 0 0 9 10 11 0 13
     * 
     * What metrics are associated with this question?
     * 
     * Test case set A, whose parameters are shown in the Input description, counts towards code
     * correctness. Your solution needs to be correct, but not necessarily clever or efficient to pass
     * those cases. Test case set B tests whether you developed a well-optimized solution and counts
     * towards the Algorithmic Problem Solving metric.
     * 
     * Why this question?
     * 
     * This question is designed to challenge you to work out the intricacies of how an intuitive,
     * yet complex process works, and how to optimize its computation to take the least possible
     * time. Some of the companies looking for strong backend developers are looking for this skill.
     */

    /// <summary>
    /// Verified in Visual Studio 2013 and tested.
    /// </summary>
    class Program
    {
        private static readonly string[] Scenarios =
        {
            @"11
0 0 4 1 0 2 0 1 0 0 3",
            @"12
2 3 0 0 0 0 0 6 10 1 0 2"
        };

        private static void Run(string text, TextWriter writer)
        {
            using (var reader = new StringReader(text))
            {
                new DistanceCalculator(reader, writer).Run();
            }
        }

        static void Main(string[] args)
        {
            foreach (var text in Scenarios)
            {
                Run(text, Console.Out);
            }
            //new DistanceCalculator(Console.In, Console.Out).Run();
        }
    }
}
