# TrulyRandom
A powerful .NET library for generating and testing high-quality truly random data using consumer hardware (generation is available only for Windows). Includes easy-to-use implementation of the NIST NIST SP 800-22 test library. No external dependencies.

The library allows developers to retrieve low-entropy data from microphones, cameras and user input (mouse and keyboard), process this low-entropy data into high-entropy using different randomness extractors, test it using a library of tests defined in NIST SP 800-22, store and retrieve it and finally use it to get random data of various types:

```csharp
//Using camera as an entropy source
VideoSource source = new VideoSource();

//Creating extractors
DeflateExtractor deflateExtractor = new DeflateExtractor();
ShuffleExtractor shuffleExtractor = new ShuffleExtractor();
HashExtractor hashExtractor = new HashExtractor();

//Creating tester
Tester tester = new Tester();

//Creating buffer
Buffer buffer = new Buffer();

//Linking all the modules together
deflateExtractor.AddSource(source);
shuffleExtractor.AddSource(deflateExtractor);
hashExtractor.AddSource(shuffleExtractor);
tester.AddSource(hashExtractor);
buffer.AddSource(tester);

//Starting the chain
source.Start();
deflateExtractor.Start();
shuffleExtractor.Start();
hashExtractor.Start();
tester.Start();
buffer.Start();

//Waiting for data to collect
while (buffer.BytesInBuffer < 1000)
{
    Thread.Sleep(100);
}

//Using the data
int result = buffer.DataSource.GetInt();
```

Library can be simply used to test user data with test functions defined in NIST SP 800-22:

```csharp
if (TrulyRandom.NistTests.Frequency(userData).PValues[0] > 0.01)
{
    System.Console.WriteLine("Success!");
}
```

And finally it provides a number of additional features:
1. Creation of the detailed test reports;
2. Long-term evaluation of the generator quality;
3. Automatic dumping of the exccess random data to the disk and retrieval from it when required;
4. Automatic test parameter selection in accordance to NIST SP 800-22;
5. Extractor seeding;
6. Dynamic compression adjustment depending on the output buffer state;
7. Module statistics: throughput in bytes per second, enthropy level, success rate, etc.;
8. Device autoselection for video and audio sources;
9. Still image detection;
10. Generation of different types of random data from the collected entropy;
11. Additional utilites like random array shuffle.
