Tempest.Functional helps you write more functional C# (hopefully!).

# Optional
The `Optional` type allows you avoid using `null` for missing data and instead wrap it in a monadic type. The type and supporting functions have been designed for ease of use and efficiency, allowing you to functionaly operate on data without needless memory allocations.

## Creating an optional containing a value

```csharp

// Using an implicit conversion
Optional<int> age = 20;

// or

var age = Optional.Some(20);

// or even

var age = new Optional<int>(20);
```

## Creating an optional containing nothing (none)

```csharp
Optional<int> age = Option.None;

// or

Optional<int> age = default;

// or even

Optional<int> age = new();
```

Alternatively you can bring in the `none` pseudo-keyword from the `Tempest.Functional.Keywords.None` class:

```csharp

using static Tempest.Functional.Keywords.None;

Option<int> age = none;

```

## Creating from nullable types
If you've got a nullable value or reference type then you can easily lift it into an `Option` using the `From` methods:

```csharp
int? age = 10;

// optionalAge is a Option<int>
var optionalAge = Option.From(age);

string? name = "Bob";

// optionalName is a Option<string>
var optionalName = Option.From(name);

```

If the nullable value or reference is `null` then the returned option is none:

```csharp
int? age = null;

// optionalAge is a Option<int>
var optionalAge = Option.From(age);

// This will be true
if(optionalAge.IsNone)
{
    Console.WriteLine("There's no age");
}

```

## Checking for none
You can test to see if an option is none using either the `IsNone` property of comparing with the `None` constant:

```csharp
Option<int> age = ...;

if(age.IsNone)
{
    Console.WriteLine("There's no age");
}

// or

if(age == Option.None)
{
    Console.WriteLine("There's no age");
}

// or

if(age != Option.None)
{
    Console.WriteLine("we've got an age");
}
```

## Extracting the value
If your option contains a value then you can get it out in a variety of ways.

To get the value unconditionally use `Value()`. Note that it will throw an exception if the option is none.

```csharp
// Value() will throw is the option in none
Option<int> age = ...;
var unpackedAge = age.Value();
```

To get the value or return a default use `ValueOr()`:

```csharp
Option<int> age = ...;
var unpackedAge = age.ValueOr(-1);

// Or to source the default from somewhere else
Option<int> age = ...;
var unpackedAge = age.ValueOr(() => MakeDefaultAge());

```

The `OrElse` extension method returns the option if it is something, otherwise it returns the value passed in:

```csharp
Option<int> age = ...;

// If age is none then it will be set to Some(30)
age = age.OrElse(30);

```

## Matching against the option
The `Match` function allows you to process the value help in the option.

```csharp
Option<string> name = ...;

int length = name.Match
             (
                some: x => x.Length,
                none: () => -1
             );

```

You can conditionally extract the value using `TryGetValue`:

```csharp
Option<int> age = ...;
if(age.TryGetValue(out var someAge))
{
    Console.WriteLine("Got an age");
}
```

## Transforming an option
The `Select` function allows you to transform an option into a new option.

```csharp
Option<string> name = "Robert";
Option<int> length = name.Select(n => n.Length);
```

If the option you are selecting against is none then the result will be none for the target type:

```csharp
Option<string> name = Option.None;
Option<int> length = name.Select(n => n.Length);

// length == Option.None

```

## Extracting to a nullable type
You can extract the option to a nullable value or reference type using the `ToNullable` extension method.

```csharp

Option<int> age = Option.None;
var nullableAge = age.ToNullable(); // age is int?

Option<string> name = ...;
var nullableName = name.ToNullable(); // name is string?


```

## Linq
Linq support is available via the `SelectMany` extension method.

```csharp
Option<int> x = new(10);
Option<int> y = new(20);

var total = from a in x
            from b in y
            select a + b;

// total is Some(30)
```

```csharp
Option<int> x = new(10);
Option<int> y = Option.None;

var total = from a in x
            from b in y
            select a + b;

// total is None
```

## Avoiding memory allocations
Many of the option methods allow you to pass in a lambda to process the option. If this lambda is a closure then this will lead to a memory allocation. Although these allocations are typically small gen-0 objects they may be a performance issue on a critical path.

To avoid these allocation methods that take a lambda allow you to pass a state argument that will be forwarded to the lambda, thus allowing you to avoid creating a closure. You can enforce this by marking the lambda as `static`. 

For example, this method will create a closure to capture the `surname` parameter:

```csharp
public void AddSurname(string surname)
{
    Option<string> firstname = LoadName();

    var fullname = firstname.Match
                   (
                      some: name => name + surname,
                      none: ()   => "no name"
                   );
}

```

You can avoid the allocation of the closure by passing in the surname as a state argument:

```csharp
public void AddSurname(string surname)
{
    Option<string> firstname = LoadName();

    var fullname = firstname.Match
                   (
                      surname,
                      some: static (name, surname) => name + surname,
                      none: static surname         => "no name"
                   );
}

```

If you have multiple bits of data you'd like to pass as state then wrap them in a tuple:

```csharp
public void AddSurname(string middlename, string surname)
{
    Option<string> firstname = LoadName();

    var fullname = firstname.Match
                   (
                      (middlename, surname)
                      some: static (name, names) => name + names.middlename + name.surname,
                      none: static names         => "no name"
                   );
}

```

When using the stateful overloads the state is always the final parameter to the lambda.


# Unit
The `Unit` type is used to indicate the absence of a value. 

Although `void` plays a similar role in C# it is very much a second class citizen in the type system
and can lead to awkward overload situations. For example, there is `Action<>` for lambdas that do not
return a value and `Func<>` for lambdas that do return a value. Likewise there is `Task` for async methods
that do not return a value and `Task<>` for async methods that do.

The `UnitTask` class in the `Tempest.Functional.Threading` namespace provides convenient ways to create a
`Task<Unit>` instances, and comes with a `CompletedTask` instance that can be reused in order to avoid
task allocations.

You can use the `unit` pseudo-keyword in `Tempest.Functional.Keywords.Unit` to provide a more descriptive way
of returning a `Unit` type:

```csharp

using static Tempest.Functional.Keywords.Unit;

Func<Unit> sayHello =
{
    Console.WriteLine("hello");
    return unit;
};

```