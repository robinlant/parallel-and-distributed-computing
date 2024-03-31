namespace FormulaCalculator;
/// <summary>
/// Operations that dont consume much resources to make them async
/// </summary>
public static class Operations
{
	/// <summary>
	/// v1 + v2
	/// </summary>
	public static double[] SumVectors(double[] v1, double[] v2)
	{
		if (v1.Length != v2.Length) throw new ArgumentException("Vectors should be the same length to be subtracted");

		var res = new double[v1.Length];

		for (var i = 0; i < v1.Length; i++)
		{
			res[i] = v1[i] + v2[i];
		}

		return res;
	}

	/// <summary>
	/// v1 - v2
	/// </summary>
	public static double[] SubtractVectors(double[] v1, double[] v2)
	{
		if (v1.Length != v2.Length) throw new ArgumentException("Vectors should be the same length to be subtracted");

		var res = new double[v1.Length];

		for (var i = 0; i < v1.Length; i++)
		{
			res[i] = v1[i] - v2[i];
		}

		return res;
	}
	/// <summary>
	/// m * scalar
	/// </summary>
	public static double[][] MultiplyMatrixByScalar(double[][] m, double s)
	{
		var res = new double[m.Length][];

		for (var i = 0; i < m.Length; i++)
		{
			res[i] = MultiplyVectorByScalar(m[i], s);
		}

		return res;
	}
	/// <summary>
	/// v * scalar
	/// </summary>
	public static double[] MultiplyVectorByScalar(IEnumerable<double> v, double s)
	{
		return v.Select(x => x * s).ToArray();
	}

	public static double GetMaxValInVector(IEnumerable<double> v)
	{
		return v.Max();
	}

	public static double GetMinValInVector(IEnumerable<double> v)
	{
		return v.Min();
	}

	// Kahan Summation Algorithm
	public static double AccurateSum(double[] numbers)
	{
		double sum = 0.0;
		double c = 0.0;

		for (int i = 0; i < numbers.Length; i++)
		{
			double y = numbers[i] - c;
			double t = sum + y;
			c = t - sum - y;
			sum = t;
		}

		return sum;
	}
}