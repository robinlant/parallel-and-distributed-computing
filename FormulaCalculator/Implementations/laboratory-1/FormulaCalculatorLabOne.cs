using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_1;

public class FormulaCalculatorLabOne : IFormulaCalculator
{
	private readonly SampleData _data;

	public FormulaCalculatorLabOne(SampleData data)
	{
		_data = data;
	}
	/// <summary>
	/// МG=МВ*MО+МM*(МO-MB)d
	/// </summary>
	public double[][] CalcFormulaA()
	{
		throw new NotImplementedException();
	}
	/// <summary>
	/// A=min(В)*D-C*MO*d
	/// </summary>
	public double[] CalcFormulaB()
	{
		throw new NotImplementedException();
	}

	private static class AsyncOperations
	{

	}
}