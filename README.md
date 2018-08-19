# Monad
C# implementation of the result and other monads with query syntax support and extensions for Tasks and IEnumerable

# NuGet
https://www.nuget.org/packages/Monad/1.0.0

# Task result monad

```csharp
var query =
    from i in Task.FromResult("pippo".ToResult())
    from p in (i + "peppe").ToResult()
    select p;

var chain =
    Task.FromResult("pippo".ToResult())
        .SelectMany(i => (i + "peppe").ToResult());

Assert.That(query.Result(), Is.EqualTo(chain.Result()));
Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
```

# Enumerable result monad

```csharp
var query =
    from i in new[] { "pippo" }.AsEnumerable().ToResult()
    from p in (i + "peppe").ToResult()
    select p;

var chain =
    new[] { "pippo" }.AsEnumerable()
                     .ToResult()
                     .SelectMany(i => (i + "peppe").ToResult());

Assert.That(query.Result(), Is.EqualTo(chain.Result()));
Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
```

# Plain result monad

```csharp
var query =
    from i in "pippo".ToResult()
    from p in (i + "peppe").ToResult()
    from q in (p + "peppa").ToResult()
    from r in (q + "pippi").ToResult()
    select r;

var chain =
    "pippo".ToResult()
           .SelectMany(i => (i + "peppe").ToResult())
           .SelectMany(p => (p + "peppa").ToResult())
           .SelectMany(q => (q + "pippi").ToResult());

Assert.That(query.Result(), Is.EqualTo(chain.Result()));
Assert.That(chain.Result(), Is.EqualTo("pippopeppepeppapippi"));
```
