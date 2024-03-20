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
		//МO-MB
		var p3 = new double[_data.MO.Length][];
		var p3Thread = new Thread(() =>
		{
			AsyncOperations.SubtractMatrices(_data.MO,_data.MB,p3);
		});
		p3Thread.Start();

		//МВ*MО
		var p1 = new double[_data.MB.Length][];
		var p1Thread = new Thread(() =>
		{
			AsyncOperations.MultiplyMatrices(_data.MB,_data.MO,p1);
		});
		p1Thread.Start();

		p3Thread.Join();

		var p2 = new double[_data.MM.Length][];
		AsyncOperations.MultiplyMatrices(_data.MM,Operations.MultiplyMatrixByScalar(p3,_data.d),p2);

		p1Thread.Join();

		var res = new double[p1.Length][];
		AsyncOperations.SumMatrices(p1,p2,res);
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
				Operations.MultiplyVectorByScalar(_data.C, _data.d),
				_data.MO,
				p2);
		});
		p2Thread.Start();
		//min(В)*D
		var p1 = Operations.MultiplyVectorByScalar(_data.D, Operations.GetMinValInVector(_data.B));

		p2Thread.Join();

		return Operations.SubtractVectors(p1, p2);
	}

	private static class AsyncOperations
	{
		public static void MultiplyVectorByMatrix(double[] v, double[][] m, double[] resultVector)
		{
			var threads = new Thread[m[0].Length];

			for (var i = 0; i < m[0].Length; i++)
			{
				var localI = i;

				threads[i] = new Thread(() =>
				{
					resultVector[localI] = 0;
					for (int k = 0; k < v.Length; k++)
					{
						resultVector[localI] += v[k] * m[k][localI];
					}
				});

				threads[i].Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}
		}


		public static void MultiplyMatrices(double[][] m1, double[][] m2, double[][] resultMatrix)
		{
			var threads = new Thread[m1.Length];

			for (var i = 0; i < m1.Length; i++)
			{
				resultMatrix[i] = new double[m2[0].Length];

				var localI = i;
				threads[i] = new Thread(() =>
				{
					for (var j = 0; j < m2[0].Length; j++)
					{
						resultMatrix[localI][j] = 0;

						for (var k = 0; k < m1[localI].Length; k++)
						{
							resultMatrix[localI][j] += m1[localI][k] * m2[k][j];
						}
					}
				});
				threads[i].Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}
		}

		public static void SubtractMatrices(double[][] m1, double[][] m2, double[][] resultMatrix)
		{
			var threads = new Thread[m1.Length];

			for (var i = 0; i < m1.Length; i++)
			{
				var localI = i; // i is gonna change each loop so we need a stable i

				threads[i] = new Thread(() =>
				{
					resultMatrix[localI] = Operations.SubtractVectors(m1[localI], m2[localI]);
				});

				threads[i].Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

		}

		public static void SumMatrices(double[][] m1, double[][] m2, double[][] resultMatrix)
		{
			var threads = new Thread[m1.Length];

			for (var i = 0; i < m1.Length; i++)
			{
				var localI = i;

				threads[i] = new Thread(() =>
				{
					resultMatrix[localI] = Operations.SumVectors(m1[localI], m2[localI]);
				});

				threads[i].Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}
		}
	}
}