using AtmApp.Settings;
using AtmApp.Result;

namespace AtmApp.Atm
{
    internal class ATM(ISettings settings)
    {
        private MoneyCasesCeeper _casesCeeper = new(settings);
        private ISettings _settings { get; } = settings;

        public RunResult AddMoneyBatch(int value, int ammount)
        {
            try
            {
                if (_casesCeeper.TryAddMoney(value, ammount))
                {
                    return new RunResult()
                    {
                        Success = true,
                        Continuer = null,
                        Message = null
                    };
                }
                else
                {
                    return new RunResult()
                    {
                        Success = false,
                        Continuer = GetContinuerAction(_casesCeeper.ForceAddMoney, value, ammount),
                        Message = $"There are not enough room left for all bancknotes. Do you want to add before case filled?"
                    };
                }
            }
            catch (ArgumentException ex)
            {

                return new RunResult
                {
                    Success = false,
                    Continuer = null,
                    Message = ex.Message
                };
            }
        }

        public RunResult GetMoneyBig(int value)
        {
            if (value > _casesCeeper.GetTotalMoneysInStorage)
            {
                return new RunResult
                {
                    Success = false,
                    Continuer = GetContinuerAction(_casesCeeper.ForceGetAllMoney),
                    Message = $"Moneys left {_casesCeeper.GetTotalMoneysInStorage} is less than you planing to get. Do you want to get all the left money?"
                };
            }
            _casesCeeper.SaveCurrentState();
            for (int i = _settings.ValuesCount - 1; i>=0; i--)
            {
                int currentValue = _settings.MoneyCases[i].value;
                while (value >= currentValue && _casesCeeper.GetCurrentAmmount(i) > 0)
                {
                    value-= currentValue;
                    _casesCeeper.TryGetMoney(currentValue, 1);
                }
            }
            if (value > 0)
            {
                _casesCeeper.RestoreState();
                return new RunResult
                {
                    Success = false,
                    Continuer = null,
                    Message = $"Current value cannot be givvent with existing bancknotes"
                };
            }
            return new RunResult
            {
                Success = true,
                Continuer = null,
                Message = null
            };
        }

        public RunResult GetMoneySmall(int value)
        {
            if (value > _casesCeeper.GetTotalMoneysInStorage)
            {
                return new RunResult
                {
                    Success = false,
                    Continuer = GetContinuerAction(_casesCeeper.ForceGetAllMoney),
                    Message = $"Moneys left {_casesCeeper.GetTotalMoneysInStorage} is less than you planing to get. Do you want to get all the left money?"
                };
            }
            _casesCeeper.SaveCurrentState();
            for (int i = 0; i< _settings.ValuesCount; i++)
            {
                int currentValue = _settings.MoneyCases[i].value;
                while (value > currentValue && _casesCeeper.GetCurrentAmmount(i) > 0)
                {
                    value-= currentValue;
                    _casesCeeper.TryGetMoney(currentValue, 1);
                }
            }
            if (value > 0)
            {
                _casesCeeper.RestoreState();
                return new RunResult
                {
                    Success = false,
                    Continuer = null,
                    Message = $"Current value cannot be givvent with existing bancknotes"
                };
            }
            return new RunResult
            {
                Success = true,
                Continuer = null,
                Message = null
            };
        }

        public RunResultWithValue<int> GetCurrency()
        {
            return new RunResultWithValue<int>
            {
                Success = true,
                Continuer = null,
                Message = null,
                Value = _casesCeeper.GetTotalMoneysInStorage
            };
        }

        public RunResultWithValue<IReadOnlyCollection<(int value,int ammount)>> GetBanknotesAmmounts()
        {
            (int,int)[] result = new (int,int)[_settings.ValuesCount];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (_settings.MoneyCases[i].value, _casesCeeper.GetCurrentAmmount(i));
            }
            return new RunResultWithValue<IReadOnlyCollection<(int,int)>>
            {
                Success = true,
                Continuer = null,
                Message = null,
                Value = result
            };
        }

        private Action<bool> GetContinuerAction(Action<int,int> nextAction, int value, int ammount)
        {
            return (b) =>
            {
                if (b)
                {
                    nextAction(value, ammount);
                }
            };
        }

        private Action<bool> GetContinuerAction(Action nextAction)
        {
            return (b) =>
            {
                if (b)
                {
                    nextAction();
                }
            };
        }
    }
}
