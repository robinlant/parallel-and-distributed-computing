using SampleDataGenerator;

var g = new DataGenerator();

var sampleData = new SampleData(
	g.GenerateMatrix(100, 100, 1, 100),
	g.GenerateMatrix(100, 100, 1, 100),
	g.GenerateMatrix(100, 100, 1, 100),
	g.GenerateVector(100, 1, 100),
	g.GenerateVector(100, 1, 100),
	g.GenerateVector(100, 1, 100),
	g.GenerateScalar(1, 100)
);

DataSerializer.Serialize(sampleData, "../../../../ConsoleApp/Data/data.json"); // Automatically adds data to the ConsoleApp
