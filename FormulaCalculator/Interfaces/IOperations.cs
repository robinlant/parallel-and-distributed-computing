namespace FormulaCalculator.Interfaces;

public interface IOperations
{
	double[][] MultiplyMatrices(double[][] m1, double[][] m2);

	double[] MultiplyVectorByMatrix(double[] v, double[][] m);

	double[][] MultiplyMatrixByScalar(double[][] m, double scalar);

	double[] MultiplyVectorByScalar(double[] v, double scalar);

	double[][] AddMatrices(double[][] m1, double[][] m2);

	double[][] MinusMatrices(double[][] m1, double[][] m2);

	double[][] MinusVectors(double[] v1, double[] v2);

	double MinValInVector(double[] v);
}