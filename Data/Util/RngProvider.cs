using System;

namespace InitiativeTracker.Data.Util
{
    /// <summary>
    /// Fun dice functions!
    /// </summary>
    public static class RngProvider
    {
        public static Random RNG = new Random();

        //xd
        public static int d2()
        {
            return RNG.Next(1, 3);
        }

        //rock thrown by silth
        public static int d4()
        {
            return RNG.Next(1, 5);
        }

        //hit dice
        public static int d6()
        {
            return RNG.Next(1, 7);
        }

        //longsword
        public static int d8()
        {
            return RNG.Next(1, 9);
        }

        //also longsword
        public static int d10()
        {
            return RNG.Next(1, 11);
        }

        //witch bolt
        public static int d12()
        {
            return RNG.Next(1, 13);
        }

        //inititive
        public static int d20()
        {
            return RNG.Next(1, 21);
        }

        //literally nothing and everything at the same time
        public static int dX(int x)
        {
            return RNG.Next(1, x + 1);
        }

        //everything
        public static int XdY(int x, int y)
        {
            int sum = 0;
            for (int i = 0; i < x; i++) sum += RNG.Next(1, y + 1);
            return sum;
        }
    }
}
