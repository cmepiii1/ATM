using AtmApp.Settings;

namespace AtmApp.Atm
{
    internal class MoneyCasesCeeper
    {
        private Dictionary<int, int> _indexes;
        private ISettings _settings { get; }
        private int[] _leftAmmount;
        private int[] _storedleftAmmount;
        private int _total;
        private int _storedTotal;

        public MoneyCasesCeeper(ISettings settings)
        {
            _total = 0;
            _settings = settings;
            _indexes = [];
            _leftAmmount = new int[settings.ValuesCount];
            for (int i = 0; i< settings.ValuesCount; i++)
            {
                _indexes[_settings.MoneyCases[i].value] = i;
                _leftAmmount[i] = 0;
            }
        }

        public bool IsValidValue(int value)
        {
            return _indexes.ContainsKey(value);
        }

        public bool TryAddMoney(int value, int ammount)
        {
            if (!IsValidValue(value))
            {
                throw new ArgumentException($"Unknown value {value}");
            }
            int index = _indexes[value];
            if (_leftAmmount[index] + ammount <= _settings.MoneyCases[index].ammount)
            {
                _leftAmmount[index]+=ammount;
                _total += value * ammount;
                return true;
            }
            return false;
        }

        public void ForceAddMoney(int value, int ammount)
        {
            int index = _indexes[value];
            _leftAmmount[index] = int.Min(_settings.MoneyCases[index].ammount, _leftAmmount[index] + ammount);
            RefreshTotal();
        }

        public bool TryGetMoney(int value, int ammount)
        {
            if (!IsValidValue(value))
            {
                throw new ArgumentException($"Unknown value {value}");
            }
            int index = _indexes[value];
            if (_leftAmmount[index] >= ammount)
            {
                _leftAmmount[index]-=ammount;
                _total -= value * ammount;
                return true;
            }
            return false;
        }

        public void ForceGetMoney(int value, int ammount)
        {
            int index = _indexes[value]; 
            _leftAmmount[index] = int.Max(0, _leftAmmount[index] - ammount);
            RefreshTotal();
        }

        public void ForceGetAllMoney()
        {  
            for(int i = 0; i < _settings.ValuesCount; i++)
            {
                _leftAmmount[i] = 0;
            }
            _total = 0;
        }

        public int GetCurrentAmmount(int index) => _leftAmmount[index];

        public void SaveCurrentState()
        {
            _storedleftAmmount = new int[_settings.ValuesCount];
            _leftAmmount.CopyTo(_storedleftAmmount, 0);
            _storedTotal = _total;
        }

        public void RestoreState()
        {
            _leftAmmount = _storedleftAmmount;
            _total = _storedTotal;
        }

        public int GetTotalMoneysInStorage => _total;

        private void RefreshTotal()
        {
            _total = 0;
            for(int i = 0; i < _settings.ValuesCount; i++)
            {
                _total += _settings.MoneyCases[i].value * _leftAmmount[i];
            }
        }
    }
}
