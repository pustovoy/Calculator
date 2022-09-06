using Calculator.ViewModels;

namespace Calculator.Core
{
    class ResultState: CalculatorState
    {
        public override void Handle(CalculatorViewModel vm)
        {
            vm.SetState(vm.FirstNumberState);
            vm.Model.ResetData();
            vm.WriteSaveFile();
        }
        public override string GetName()
        {
            return "ResultState";
        }
    }
}
