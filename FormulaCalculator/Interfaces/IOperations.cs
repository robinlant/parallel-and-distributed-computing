namespace FormulaCalculator.Interfaces;

public interface IOperations
{
	double[][] MultiplyMatrices(double[][] m1, double[][] m2);

	double[] MultiplyVectorByMatrix(double[] v, double[][] m);

	double[][] MultiplyMatrixByScalar(double[][] m, double scalar);

	double[] MultiplyVectorByScalar(double[] v, double scalar);

	double[][] AddMatrices(double[][] m1, double[][] m2); // uses Operations inside

	double[][] MinusMatrices(double[][] m1, double[][] m2); // uses Operations inside

	double[][] MinusVectors(double[] v1, double[] v2); // Operations

	double MinValInVector(double[] v); // Linq => Min
}