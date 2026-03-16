# Philiprehberger.Disposer

[![CI](https://github.com/philiprehberger/dotnet-disposer/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-disposer/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.Disposer.svg)](https://www.nuget.org/packages/Philiprehberger.Disposer)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-disposer)](LICENSE)

Composite disposable container — collect multiple `IDisposable`/`IAsyncDisposable` objects and dispose them all at once in reverse order with error aggregation.

## Install

```bash
dotnet add package Philiprehberger.Disposer
```

## Usage

```csharp
using Philiprehberger.Disposer;

using var disposer = new Disposer();

var stream = new FileStream("data.bin", FileMode.Open);
disposer.Add(stream);

var reader = new StreamReader(stream);
disposer.Add(reader);

// Add a cleanup delegate
disposer.Add(() => Console.WriteLine("All cleaned up!"));

// When disposer is disposed, items are cleaned up in reverse order:
// 1. Cleanup delegate runs
// 2. reader is disposed
// 3. stream is disposed
```

### Fluent Registration with DisposeWith

```csharp
using Philiprehberger.Disposer;

using var disposer = new Disposer();

var stream = new FileStream("data.bin", FileMode.Open)
    .DisposeWith(disposer);

var reader = new StreamReader(stream)
    .DisposeWith(disposer);
```

### Async Disposal

```csharp
using Philiprehberger.Disposer;

await using var disposer = new Disposer();

var connection = new HttpClient()
    .DisposeWith(disposer);

// DisposeAsync is called when the await using block exits
```

### Error Aggregation

All registered items are disposed even if some throw. Exceptions are collected and thrown as a single `AggregateException`.

## API

### `Disposer`

| Member | Description |
|--------|-------------|
| `Add(params IDisposable[])` | Register one or more disposable objects |
| `Add(Action)` | Register a cleanup delegate |
| `AddAsync(IAsyncDisposable)` | Register an async disposable object |
| `Count` | Number of registered disposables |
| `Dispose()` | Dispose all in reverse order (LIFO) |
| `DisposeAsync()` | Async dispose all in reverse order (LIFO) |

### `DisposerExtensions`

| Method | Description |
|--------|-------------|
| `DisposeWith<T>(Disposer)` | Register an `IDisposable` and return it for fluent use |
| `DisposeAsyncWith<T>(Disposer)` | Register an `IAsyncDisposable` and return it for fluent use |

## Development

```bash
dotnet build src/Philiprehberger.Disposer.csproj --configuration Release
```

## License

MIT
