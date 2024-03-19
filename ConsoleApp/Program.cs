 using SampleDataGenerator;

 var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/data.json");

Console.Write(sampleData);