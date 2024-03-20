using ConsoleApp;
using FormulaCalculator.Implementations.laboratory_1;
using SampleDataGenerator;

var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/data.json");
var calc = new FormulaCalculatorLabOne(sampleData!);

Printer.PrintResults(calc.CalcFormulaA(),5,"Formula A Lab 1");
Printer.PrintResults(calc.CalcFormulaB(),5,"Formula B Lab 1");
