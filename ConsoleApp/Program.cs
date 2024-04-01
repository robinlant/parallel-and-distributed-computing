﻿using BenchmarkDotNet.Running;
using ConsoleApp;
using ConsoleApp.Benchmarks;
using FormulaCalculator.Implementations.laboratory_3;
using FormulaCalculator.Implementations.laboratory_4;
using SampleDataGenerator;

var data = "data.json";
Console.WriteLine("DataSet:  " + data);
var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/" + data);

var calc3 = new FormulaCalculatorLabThree(sampleData!, 5);
var calc4 = new FormulaCalculatorLabFour(sampleData!, 5);

Printer.PrintResults(calc3.CalcFormulaA(),5,"Formula A Lab 3");
Printer.PrintResults(calc3.CalcFormulaB(),5,"Formula B Lab 3");

Printer.PrintResults(calc4.CalcFormulaA(),5,"Formula A Lab 4");
Printer.PrintResults(calc4.CalcFormulaB(),5,"Formula B Lab 4");

BenchmarkRunner.Run<Benchmark>();