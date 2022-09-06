using System;
using System.ComponentModel;

namespace Calculator.Models
{
    public class CalculatorModel : ICalculatorModel
    {
        #region Private vars
        private int? _firstNumber;
        private int? _secondNumber;
        private Operation _action;
        private double? _result;
        #endregion

        #region Props
        public int? FirstNumber
        {
            get => _firstNumber;
            set
            {
                _firstNumber = value;
                OnPropertyChanged("FirstNumber");
            }
        }
        public int? SecondNumber
        {
            get => _secondNumber;
            set
            {
                _secondNumber = value;
                OnPropertyChanged("SecondNumber");
            }
        }
        public Operation Action
        {
            get => _action;
            set
            {
                _action = value;
                OnPropertyChanged("Operation");
            }
        }
        public double? Result
        {
            get
            {
                return _result;
            }
            private set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Validators
        private bool Validate(int? num)
        {
            return num is not null;
        }

        private bool Validate(Operation op)
        {
            return op is not Operation.Null;
        }

        private bool ValidateInputs()
        {
            return Validate(FirstNumber) && Validate(SecondNumber) && Validate(Action);
        }
        #endregion

        public void ResetData()
        {
            FirstNumber = null;
            SecondNumber = null;
            Action = Operation.Null;
            Result = null;
        }

        public void Calculate()
        {
            if (!ValidateInputs())
            {
                return;
            }

            switch(Action)
            {
                case Operation.Add:
                {
                    Result = FirstNumber + SecondNumber;
                    break;
                }
                case Operation.Substract:
                {
                    Result = FirstNumber - SecondNumber;
                    break;
                }
                case Operation.Multiply:
                {
                    Result = FirstNumber * SecondNumber;
                    break;
                }
                case Operation.Divide:
                {
                    if (SecondNumber == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    Result = Convert.ToDouble(FirstNumber) / SecondNumber;
                    break;
                }
                default:
                {
                    Result = null;
                    break;
                }
            };
        }
    }

    public enum Operation
    {
        Null,
        Add,
        Substract,
        Multiply,
        Divide
    }

    public class CalculatorModelFactory
    {
        public ICalculatorModel Create()
        {
            return new CalculatorModel();
        }
    }
}

