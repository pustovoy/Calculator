using System.ComponentModel;

namespace Calculator.Models
{
    public interface ICalculatorModel: INotifyPropertyChanged
    {
        int? FirstNumber { get; set; }
        int? SecondNumber { get; set; }
        Operation Action { get; set; }
        double? Result { get; }
        void Calculate();
        void ResetData();
    }
}
