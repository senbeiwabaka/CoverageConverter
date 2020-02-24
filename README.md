1. Download coverage file from a successful build on Azure
2. Build project ```dotnet build``` from commandline or use Visual Studio
3. Navigate to build directory (default is /CoverageConverter/bin/debug/netcoreapp3.1)
4. Run ```.\CoverageConverter.exe 'C:\Users\rr611454\Downloads\20200214.5.Release.Any CPU.1351.coverage' C:\Users\rr611454\Downloads\Testing testing "C:\Users\rr611454\source\repos\CoverTaggerLibrary\"``` where the first is the full path to the file to convert. the second is the output path. third is the name? I kind of forget.