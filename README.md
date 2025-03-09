# RememberToDo.Analyzer
A simple Roslyn-based analyzer that allows the use of [RememberTo()] instead of spreading TODOs all over your code that you will just ignore. (and by "you" I mean "me")


## Usage

Include the using directive in your code:
```csharp
	using AnalyzerTools.RememberToDo;
```

Then add the following attribute to your code:
```csharp
  [RememberTo("clean this code")]  // this will trigger a warning on Release
```

```csharp
  [RememberTo("clean this code", true)]  // this will trigger a warning both on Debug and Release
```

