using Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core
{
    class SecondNumberState: CalculatorState
    {
        public override void Handle(CalculatorViewModel vm)
        {
            vm.SecondNumber = vm.UserInput;
            vm.ResetUserInput();
            vm.SetState(vm.GetResultState);
        }
        public override string GetName()
        {
            return "SecondNumberState";
        }
    }
}
