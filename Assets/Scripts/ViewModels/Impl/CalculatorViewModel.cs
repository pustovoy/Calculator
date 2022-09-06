using Calculator.Core;
using Calculator.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : ViewModel, ICalculatorViewModel
    {
        #region Private vars
        private string _configurationPath;
        #endregion

        #region Props
        public ICalculatorModel Model { get; private set; }
        public ReactiveProperty<string> Output { get; set; }
        public ReactiveProperty<CalculatorState> State { get; set; }
        public CalculatorState FirstNumberState { get; private set; }
        public CalculatorState OperatorState { get; private set; }
        public CalculatorState SecondNumberState { get; private set; }
        public CalculatorState GetResultState { get; private set; }
        public CalculatorState ResultState { get; private set; }
        public CalculatorState ExceptionState { get; private set; }
        public string UserInput { get; private set; }
        public string FirstNumber {
            get { return Model.FirstNumber is null ? string.Empty : Model.FirstNumber.ToString(); }
            set {
                if (int.TryParse(value, out int parsed))
                {
                    Model.FirstNumber = parsed;
                }
            }
        }
        public string SecondNumber {
            get { return Model.SecondNumber is null ? string.Empty : Model.SecondNumber.ToString(); }
            set
            {
                if (int.TryParse(value, out int parsed))
                {
                    Model.SecondNumber = parsed;
                }
            }
        }
        public string Op
        {
            get {
                return Model.Action switch
                {
                    Operation.Add => "+",
                    Operation.Substract => "-",
                    Operation.Multiply => "x",
                    Operation.Divide => "/",
                    _ => "",
                };
            }
            set {
                Model.Action = value switch {
                    "+" => Operation.Add,
                    "-" => Operation.Substract,
                    "x" => Operation.Multiply,
                    "/" => Operation.Divide,
                    _ => Operation.Null
                };
            }
        }
        public string Result
        {
            get { return Model.Result is null ? string.Empty : Model.Result.ToString(); }
            private set { CalculateResult(); }
        }

        public string ActiveState
        {
            get { return State.Value.GetName(); }
            private set { 
                SetState(value switch
                {
                    "FirstNumberState" => FirstNumberState,
                    "SecondNumberState" => SecondNumberState,
                    "OperatorState" => OperatorState,
                    "GetResultState" => GetResultState,
                    "ResultState" => ResultState,
                    "ExceptionState" => ExceptionState,
                    _ => null
                });
            }
        }
        #endregion

        #region Ctor
        public CalculatorViewModel()
        {
            InitStateMachine();
            Output = new ReactiveProperty<string>();
            State = new ReactiveProperty<CalculatorState>();
            var modelFactory = new CalculatorModelFactory();
            Model = modelFactory.Create();
            Model.PropertyChanged += OnPropertyChanged;
        }
        #endregion

        #region Init
        public void Init()
        {
            UserInput = string.Empty;
            Output.Value = string.Empty;

            _configurationPath = $"{Application.persistentDataPath}/CalculatorSaveFile.json";
            var data = ReadSaveFile();
            if (String.IsNullOrEmpty(data))
            {
                SetState(FirstNumberState);
                WriteSaveFile();
            }
            else
            {
                SetDataFromSave(data);
            }
        }

        private void InitStateMachine()
        {
            FirstNumberState = new FirstNumberState();
            OperatorState = new OperatorState();
            SecondNumberState = new SecondNumberState();
            GetResultState = new GetResultState();
            ResultState = new ResultState();
            ExceptionState = new ExceptionState();
        }
        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UserInput = string.Empty;
            UpdateOutput();
        }

        public void OnUserInput(string val)
        {
            if (int.TryParse(val, out _))
            {
                UserInput += val;
            }
            else
            {
                UserInput = val;
            }

            UpdateOutput();
            WriteSaveFile();
        }

        public void OnControlButtonClicked()
        {
            State.Value.Handle(this);
        }

        public void SetState(CalculatorState state)
        {
            State.Value = state;
            WriteSaveFile();
        }

        public void ResetUserInput()
        {
            UserInput = string.Empty;
        }

        internal void UpdateOutput()
        {
            var operation = Op == Operation.Null.ToString() ? string.Empty : Op;
            var state = State.Value is ResultState ? $"={Result}" : string.Empty;

            Output.Value = $"{FirstNumber}{operation}{SecondNumber}{state}{UserInput}";
        }

        internal void CalculateResult()
        {
            try
            {
                Model.Calculate();
            } 
            catch (DivideByZeroException)
            {
                SetState(ExceptionState);
            }
        }

        #region Save/Read JSON 
        public string ReadSaveFile()
        {
            if (!File.Exists(_configurationPath))
            {
                return null;
            }
            return File.ReadAllText(_configurationPath);
        }

        public void WriteSaveFile()
        {
            var saveString = GetSaveString();
            File.WriteAllText(_configurationPath, saveString);
        }

        public void SetDataFromSave(string saveString)
        {
            var deserializedObject = JsonConvert.DeserializeObject<CalculatorViewModelDataSerializable>(saveString);
            FirstNumber = deserializedObject.FirstNumber;
            SecondNumber = deserializedObject.SecondNumber;
            Op = deserializedObject.Op;
            Result = deserializedObject.Result;
            ActiveState = deserializedObject.ActiveState;
            UserInput = deserializedObject.UserInput;
            UpdateOutput();
        }

        public string GetSaveString()
        {
            return JsonConvert.SerializeObject(new CalculatorViewModelDataSerializable(FirstNumber, SecondNumber, Op, Result, UserInput, ActiveState));
        }
        #endregion
    }

    public class CalculatorViewModelFactory
    {
        public ICalculatorViewModel Create()
        {
            return new CalculatorViewModel();
        }
    }
}