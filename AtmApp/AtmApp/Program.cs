using AtmApp.Atm;
using AtmApp.Result;
using AtmApp.Settings;

namespace AtmApp
{
    internal class Program
    {
        private static ATM _atm;
        static void Main(string[] args)
        {
            _atm = new ATM(new DefaultSettings());
            char ch= '\0';
            while(ch != '0')
            {
                Console.WriteLine("1 - Add money");
                Console.WriteLine("2 - Get money");
                Console.WriteLine("3 - Current ballance");
                Console.WriteLine("0 - Exit");
                Console.Write("Input operation: ");
                var str = Console.ReadLine();
                if (!string.IsNullOrEmpty(str))
                {
                    foreach (var c in str)
                    {
                        ch = c;
                        if (ch == '0')
                        {
                            Console.WriteLine("Good bye!");
                            return;
                        }
                        CommandOperate(ch);
                    }
                }
            }
        }

        private static void CommandOperate(char ch)
        {
            switch (ch)
            {
                case '1': AddMoneyOperation(); break;
                case '2': GetMoneyOperation(); break;
                case '3': GetCurrencyOperation(); break;
                default: Console.WriteLine($"Unknown command {ch}"); break;
            }
        }

        private static void AddMoneyOperation()
        {
            Console.Write("Input banknotes value: ");
            int value = int.Parse(Console.ReadLine());
            Console.Write("Input ammount of banknotes: ");
            int ammount = int.Parse(Console.ReadLine());
            var run = _atm.AddMoneyBatch(value, ammount);
            if (run.Success)
            {
                Console.WriteLine("Money successfully added");
                return;
            }
            else
            {
                UnsuccessfullRun(run);
            }
        }

        private static void GetMoneyOperation()
        {
            Console.Write("Input expected currency to get: ");
            int value = int.Parse(Console.ReadLine());
            Console.Write("1 - biggest possible values, 2 - with changes: ");
            char ch = Console.ReadLine()[0];
            var run = ch == '1' ? _atm.GetMoneyBig(value) : _atm.GetMoneySmall(value);
            if (run.Success)
            {
                Console.WriteLine("Money successfully got");
            }
            else
            { 
                UnsuccessfullRun(run); 
            }
        }

        private static void GetCurrencyOperation()
        {
            Console.WriteLine($"Total money in ATM now : {_atm.GetCurrency().Value}");
            Console.WriteLine("Bancknotes ammount:");
            var runResult = _atm.GetBanknotesAmmounts().Value;
            foreach (var pair in runResult!)
            {
                Console.WriteLine($"There are {pair.ammount} bancknotes with value {pair.value}");
            }
        }

        private static void UnsuccessfullRun(RunResult run)
        {
            Console.WriteLine(run.Message);
            var continuer = run.Continuer;
            if (continuer != null)
            {
                Console.WriteLine("1 - yes, anything else - no");
                char ch = Console.ReadLine()[0];
                continuer.Invoke(ch == '1');
            }
        }
    }
}
