using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinionZapper
{
    /* In a video game, the player has the ability to cast a "chain lightning" spell that hits
     * multiple enemy minions. It works by first hitting an initial minion for some amount of
     * damage X, and then bounces to other minions, the spell's damage decreasing by an amount
     * Y with every subsequent hit. This continues until there are either no more minions to hit,
     * or until the spell's damage drops to 0 or less. Each minion can only be hit once.
     * 
     * You're asked to program a "smart casting" feature for this spell. Instead of the spell hitting
     * enemies at random, the spell will auto-target to kill as many of them as possible. This is a
     * smart strategy because you want to reduce the number of live enemies, who are dealing damage
     * to you, as quickly as possible.
     * 
     * Given the hit points (health) of all minions within range of your spell, determine a strategy
     * for what order you should hit the minions in. A minion dies if the damage dealt to it by the
     * spell is greater than, or equal to, its hit points. Remember that the spell can only affect
     * each minion once.
     * 
     * Input
     * 
     * The input should be read from standard input. The first line contains three space-separated
     * integers: N, X, and Y, in that order (N < 10, X < 10000, Y < 10000). N is the number of minions,
     * X is the damage of the spell to the first target, and Y is the damage reduction with every
     * subsequent target. The second line contains N space-separated integers. Each integer represents
     * the initial hit points of each minion.
     * 
     * Output
     * 
     * Output a single integer indicating the maximum number of minions you can kill. (We don't ask you
     * to output the hit sequence because many sequences can result in the same number of kills.)
     * 
     * What metrics are associated with this question?
     * 
     * This question counts towards the code correctness metric.
     * 
     * Why this question?
     * 
     * This problem is a simple problem that demonstrates your ability to analyze a situation and write
     * correct code to address it, which is the first competency companies usually look for.
     */

    public abstract class Spell
    {
        public abstract bool TryHit(Minion minion);
    }

    public class ChainLightningSpell : Spell
    {
        private static int VerifyValue(int value)
        {
            if (value >= 10000)
                throw new ArgumentException("value must be < 10000", "value");
            return value;
        }

        private int _damage;

        public int Damage
        {
            get { return _damage; }
            set { _damage = VerifyValue(value); }
        }

        private int _decrease;

        public int Decrease
        {
            get { return _decrease; }
            set { _decrease = VerifyValue(value); }
        }

        public bool CanDamage
        {
            get { return Damage > 0; }
        }

        public bool CannotDamage
        {
            get { return !CanDamage; }
        }

        public override bool TryHit(Minion minion)
        {
            if (!CanDamage) return false;
            minion.Health -= Damage;
            Damage -= Decrease;
            // This might not be the best coupling of the two concerns (i.e. spell vs. minion) but will work for sake of example.
            minion.HasBeenStruck = true;
            return true;
        }

        public ChainLightningSpell()
            : this(0, 0)
        {
        }

        // Could potentially do more to help with error trapping for bad int formats.
        public ChainLightningSpell(string damage, string decrease)
            : this(int.Parse(damage), int.Parse(decrease))
        {
        }

        public ChainLightningSpell(int damage, int decrease)
        {
            Damage = damage;
            Decrease = decrease;
        }
    }

    public class Minion
    {
        public int Health { get; set; }

        public bool HasBeenStruck { get; set; }

        public bool IsDead
        {
            get { return Health <= 0; }
        }

        public Minion()
            : this(0)
        {
        }

        // Ditto bad int formats.
        public Minion(string health)
            : this(int.Parse(health))
        {
        }

        public Minion(int health)
        {
            Health = health;
        }
    }

    public class Player
    {
        /// <summary>
        /// Casts the <paramref name="spell"/> on a group of <paramref name="minions"/>.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="minions"></param>
        /// <returns>The number of minions actually struck.</returns>
        public int Cast(ChainLightningSpell spell, params Minion[] minions)
        {
            /* Will brute force compare them like this for now. Really, this sounds like a
             * Constraint Satisfaction Problem, close neighbor to the Knapsack problem:
             * i.e. fill knapsack with items within a budget. */

            Minion target;

            do
            {
                var remaining
                    = (from m in minions
                        orderby m.Health descending
                        where !m.HasBeenStruck
                        select m).ToArray();

                // All done.
                if (!remaining.Any()) break;

                // Hold onto the last one in the event we cannot eliminate any of the minions.
                var last = remaining.Last();

                while (remaining.Any() && remaining.First().Health > spell.Damage)
                    remaining = remaining.Skip(1).ToArray();

                // Target the first remaining one or the last one.
                target = remaining.FirstOrDefault() ?? last;

                // Keep going while we can hit a target.
            } while (spell.TryHit(target));

            return minions.Count(m => m.IsDead);
        }
    }

    public class Scenario
    {
        public Player Wizard { get;set; }

        public ChainLightningSpell Spell { get; private set; }

        public IEnumerable<Minion> Minions { get; set; }

        public readonly IList<string> Lines = new List<string>(); 

        /// <summary>
        /// Works with any <see cref="TextReader"/>, including <see cref="Console.In"/>.
        /// Allows for more flexible testability of the code through canned test cases or
        /// through live input.
        /// </summary>
        /// <param name="reader"></param>
        public Scenario(TextReader reader)
        {
            Read(reader);
        }

        private static string PrepareText(string text)
        {
            var length = -1;

            while (length != text.Length)
            {
                // Might could replace all whitespaces with spaces.
                text = text.Replace("  ", " ");
                length = text.Length;
            }

            return text;
        }

        /// <summary>
        /// Reads the scenario from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        private void Read(TextReader reader)
        {
            int minionCount;

            TryParseFirst(reader.ReadLineAsync().Result, out minionCount);

            TryParseSecond(reader.ReadLineAsync().Result, minionCount);

            Wizard = new Player();
        }

        /// <summary>
        /// Tries to parse the first line <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="minionCount"></param>
        /// <returns></returns>
        private bool TryParseFirst(string text, out int minionCount)
        {
            var parts = PrepareText(text).Split(' ');

            minionCount = 0;

            if (parts.Length != 3)
                throw new ArgumentException(string.Format("Input line 1 must contain N X Y: {{{0}}}", text), "text");

            minionCount = int.Parse(parts[0]);
            Spell = new ChainLightningSpell(parts[1], parts[2]);

            if (minionCount >= 10)
                throw new ArgumentException(string.Format("Input line 1 must not specify more than 10 minions: {{{0}}}", text), "text");

            Lines.Add(text);

            return true;
        }

        private bool TryParseSecond(string text, int expectedCount)
        {
            var parts = PrepareText(text).Split(' ');

            if (parts.Length != expectedCount)
                throw new ArgumentException(string.Format("Input line 2 must contain {0} minions: {{{1}}}", expectedCount, text), "text");

            Minions = parts.Select(h => new Minion(h)).ToArray();

            Lines.Add(text);

            return true;
        }

        public void Run()
        {
            var affected = Wizard.Cast(Spell, Minions.ToArray());

            Console.WriteLine("Input:");
            Console.WriteLine(Lines[0]);
            Console.WriteLine(Lines[1]);

            Console.WriteLine();

            Console.WriteLine("Output:");
            Console.WriteLine("{0}", affected);

            Console.WriteLine();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }

    class Program
    {
        /* Ex 1)
         * 
         * Input:
         * 6 200 45
         * 250 100 95 90 80 60
         * 
         * Output:
         * 4
         * 
         * Explanation:
         * We can kill 4 minions by hitting the minions in the following order: 100, 95, 90, 60, 80.
         * The first 4 hits deal 200, 155, 110, and 65 damage, so all 4 of those minions die. The 5th
         * hit only deals 20 damage, so it won’t kill the minion with 80 health (or any other minion
         * we could have chosen). Since the spell can never deal 250 damage, we can never kill the
         * minion with 250 health.
         */

        /* Ex 2)
         * 
         * Input:
         * 5 600 200
         * 500 400 325 300 250
         * 
         * Output:
         * 2
         * 
         * Explanation:
         * The spell will hit for 600, 400, and then 200 damage. The last hit is unable to kill any
         * minion. The first two hits can be used productively, for example to kill the 400 and 300
         * health minions.
         */

        public static readonly string[] Scenarios =
        {
            @"6 200 45
250 100 95 90 80 60",
            @"5 600 200
500 400 325 300 250"
        };

        private static void RunScenario(string text)
        {
            using (var reader = new StringReader(text))
            {
                new Scenario(reader).Run();
            }
        }

        private static void Main(string[] args)
        {
            foreach (var text in Scenarios)
                RunScenario(text);
        }
    }
}
