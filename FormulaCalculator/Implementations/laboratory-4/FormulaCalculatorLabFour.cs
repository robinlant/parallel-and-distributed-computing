using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_4;

public class FormulaCalculatorLabFour : IFormulaCalculator
{
	private readonly SampleData _data;
	private readonly int _maxThreadsPerMethod;

	public FormulaCalculatorLabFour(SampleData data, int maxThreadsPerMethod)
	{
		_data = data;
		_maxThreadsPerMethod = maxThreadsPerMethod;
	}

	public double[][] CalcFormulaA()
	{
		var task = CalcFormulaAAsync();
		task.Wait();
		return task.GetAwaiter().GetResult();
	}

	public double[] CalcFormulaB()
	{
		var task = CalcFormulaBAsync();
		task.Wait();
		return task.GetAwaiter().GetResult();
	}

	/// <summary>
	/// МG=МВ*MО+МM*(МO-MB)d
	/// </summary>
	private async Task<double[][]> CalcFormulaAAsync()
	{
		//МO-MB
		var p3 = AsyncOperations.SubtractMatrices(_data.MO,_data.MB, _maxThreadsPerMethod);

		//МВ*MО
		var p1 = AsyncOperations.MultiplyMatrices(_data.MB,_data.MO, _maxThreadsPerMethod);

		var p2 = AsyncOperations.MultiplyMatrices(_data.MM,Operations.MultiplyMatrixByScalar(await p3,_data.d), _maxThreadsPerMethod);

		return await AsyncOperations.SumMatrices(await p1,await p2, _maxThreadsPerMethod);
	}

	/// <summary>
	/// A=min(В)*D-C*MO*d
	/// </summary>
	private async Task<double[]> CalcFormulaBAsync()
	{
		//C*MO*d
		var p2 = AsyncOperations.MultiplyVectorByMatrix(Operations.MultiplyVectorByScalar(_data.C, _data.d), _data.MO, _maxThreadsPerMethod);

		//min(В)*D
		var p1 = Operations.MultiplyVectorByScalar(_data.D, Operations.GetMinValInVector(_data.B));

		return Operations.SubtractVectors(p1, await p2);
	}

	private static class AsyncOperations
	{
	    /// <summary>
	    /// Executes an action in parallel within specified range of iterations.
	    /// Custom parallel for loop
	    /// </summary>
	    /// <param name="action">Action/<int/> that executes on every iteration, int represents current index of iteration</param>
	    /// <param name="startIndex">Starting index</param>
	    /// <param name="endIndex">End index, Action with this value IS NOT EXECUTED, UPPER BOUND</param>
	    /// <param name="tasksLimit">Max amount of threads to be used</param>
	    private static async Task ExecuteInParallel(Action<int> action, int startIndex, int endIndex, int tasksLimit)
	    {
		    var totalWork = endIndex - startIndex;
		    var tasks = new Task[tasksLimit];
		    var maxWorkPerTask = (int)Math.Ceiling((double)totalWork / tasksLimit);

		    for (var taskIndex = 0; taskIndex < tasksLimit; taskIndex++)
		    {
			    var startWork = startIndex + taskIndex * maxWorkPerTask;
			    var endWork = Math.Min(startWork + maxWorkPerTask, endIndex);

			    tasks[taskIndex] = Task.Run(() =>
			    {
				    for (var workIndex = startWork; workIndex < endWork; workIndex++)
				    {
					    action(workIndex);
				    }
			    });
		    }

		    await Task.WhenAll(tasks);
	    }

	    public static async Task<double[]> MultiplyVectorByMatrix(double[] v, double[][] m, int tasksLimit)
	    {
		    var resultVector = new double[m[0].Length];

		    await ExecuteInParallel(colIndex =>
		    {
			    double sum = 0.0;
			    double c = 0.0; // Kahan Summation compensation

			    for (int k = 0; k < v.Length; k++)
			    {
				    double y = v[k] * m[k][colIndex] - c;
				    double t = sum + y;
				    c = t - sum - y;
				    sum = t;
			    }

			    resultVector[colIndex] = sum;
		    }, 0, resultVector.Length, tasksLimit);

		    return resultVector;
	    }

	    public static async Task<double[][]> MultiplyMatrices(double[][] m1, double[][] m2, int tasksLimit)
	    {
		    var rows = m1.Length;
		    var resultMatrix = new double[m1.Length][];

		    await ExecuteInParallel(rowIndex =>
		    {
			    resultMatrix[rowIndex] = new double[m2[0].Length];

			    for (int j = 0; j < m2[0].Length; j++)
			    {
				    double sum = 0.0;
				    for (int k = 0; k < m1[rowIndex].Length; k++)
				    {
					    sum += m1[rowIndex][k] * m2[k][j];
				    }
				    resultMatrix[rowIndex][j] = sum;
			    }
		    }, 0, rows, tasksLimit);

		    return resultMatrix;
	    }

	    public static async Task<double[][]> SubtractMatrices(double[][] m1, double[][] m2, int tasksLimit)
	    {
		    var resultMatrix = new double[m1.Length][];

		    await ExecuteInParallel(rowIndex =>
		    {
			    resultMatrix[rowIndex] = Operations.SubtractVectors(m1[rowIndex], m2[rowIndex]);
		    }, 0, m1.Length, tasksLimit);

		    return resultMatrix;
	    }

	    public static async Task<double[][]> SumMatrices(double[][] m1, double[][] m2 , int tasksLimit)
	    {
		    var resultMatrix = new double[m1.Length][];

		    await ExecuteInParallel(rowIndex =>
		    {
			    resultMatrix[rowIndex] = Operations.SumVectors(m1[rowIndex], m2[rowIndex]);
		    }, 0, m1.Length, tasksLimit);

		    return resultMatrix;
	    }
	}
}