using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_2;

public class FormulaCalculatorLabTwo : IFormulaCalculator
{
	private readonly SampleData _data;
	private readonly int _maxThreadsPerMethod;

	public FormulaCalculatorLabTwo(SampleData data, int maxThreadsPerMethod)
	{
		_data = data;
		_maxThreadsPerMethod = maxThreadsPerMethod;
	}
	
	/// <summary>
	/// МG=МВ*MО+МM*(МO-MB)d
	/// </summary>
	public double[][] CalcFormulaA()
	{
		//МO-MB
		var p3 = new double[_data.MO.Length][];
		var p3Thread = new Thread(() =>
		{
			AsyncOperations.SubtractMatrices(_data.MO,_data.MB,p3, _maxThreadsPerMethod);
		});
		p3Thread.Start();

		//МВ*MО
		var p1 = new double[_data.MB.Length][];
		var p1Thread = new Thread(() =>
		{
			AsyncOperations.MultiplyMatrices(_data.MB,_data.MO,p1, _maxThreadsPerMethod);
		});
		p1Thread.Start();

		p3Thread.Join();

		var p2 = new double[_data.MM.Length][];
		AsyncOperations.MultiplyMatrices(_data.MM,Operations.MultiplyMatrixByScalar(p3,_data.d),p2, _maxThreadsPerMethod);

		p1Thread.Join();

		var res = new double[p1.Length][];
		AsyncOperations.SumMatrices(p1,p2,res, _maxThreadsPerMethod);
		return res;
	}
	/// <summary>
	/// A=min(В)*D-C*MO*d
	/// </summary>
	public double[] CalcFormulaB()
	{
		//C*MO*d
		double[] p2 = new double[_data.C.Length];
		var p2Thread = new Thread(() =>
		{
			AsyncOperations.MultiplyVectorByMatrix(
				Operations.MultiplyVectorByScalar(_data.C, _data.d), _data.MO, p2, _maxThreadsPerMethod);
		});
		p2Thread.Start();
		//min(В)*D
		var p1 = Operations.MultiplyVectorByScalar(_data.D, Operations.GetMinValInVector(_data.B));

		p2Thread.Join();

		return Operations.SubtractVectors(p1, p2);
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
	    /// <param name="threadsLimit">Max amount of threads to be used</param>
	    private static void ExecuteInParallel(Action<int> action, int startIndex, int endIndex, int threadsLimit)
	    {
		    var totalWork = endIndex - startIndex;
		    var countdown = new CountdownEvent(threadsLimit);
		    var maxWorkPerThread = (int)Math.Ceiling((double)totalWork / threadsLimit);

		    for (var threadIndex = 0; threadIndex < threadsLimit; threadIndex++)
		    {
			    var startWork = startIndex + threadIndex * maxWorkPerThread;
			    var endWork = Math.Min(startWork + maxWorkPerThread, endIndex);

			    var thread = new Thread(() =>
			    {
				    for (var workIndex = startWork; workIndex < endWork; workIndex++)
				    {
					    action(workIndex);
				    }
				    countdown.Signal();
			    });
			    thread.Start();
		    }

		    countdown.Wait();
	    }

	    public static void MultiplyVectorByMatrix(double[] v, double[][] m, double[] resultVector, int threadsLimit)
	    {
		    var columns = m[0].Length;
		    ExecuteInParallel(colIndex =>
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
		    }, 0, columns, threadsLimit);
	    }

	    public static void MultiplyMatrices(double[][] m1, double[][] m2, double[][] resultMatrix, int threadsLimit)
	    {
		    var rows = m1.Length;
		    ExecuteInParallel(rowIndex =>
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
		    }, 0, rows, threadsLimit);
	    }

	    public static void SubtractMatrices(double[][] m1, double[][] m2, double[][] resultMatrix, int threadsLimit)
	    {
		    ExecuteInParallel(rowIndex =>
		    {
			    resultMatrix[rowIndex] = Operations.SubtractVectors(m1[rowIndex], m2[rowIndex]);
		    }, 0, m1.Length, threadsLimit);
	    }

	    public static void SumMatrices(double[][] m1, double[][] m2, double[][] resultMatrix, int threadsLimit)
	    {
		    ExecuteInParallel(rowIndex =>
		    {
			    resultMatrix[rowIndex] = Operations.SumVectors(m1[rowIndex], m2[rowIndex]);
		    }, 0, m1.Length, threadsLimit);
	    }
	}
}