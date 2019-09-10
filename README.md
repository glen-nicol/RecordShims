# Record Shims

## What are records?

[Records](https://en.wikipedia.org/wiki/Record_(computer_science)) are immutable classes that hold 
PODS(Plain old data structures). They represent set of data fields at a given time. Immutability
ensures that a record is reliable while in a multi-threaded environment. Records should not involve any complex business logic beyond data constraints. IE: don't hold a database connection in a record.

## Why use Record Shims

Immutability in C# typically requires to a lot of boilerplate to get a new record with one or more changed. 
Furthermore, this usually leads to brittle functions with default arguments or avoiding immutability entirely and asking developers to be good citizens with the public setters.

This library aims to reduce that boilerplate for older projects that cannot upgrade to [C# 8.0](https://github.com/dotnet/csharplang/blob/master/proposals/records.md) when it is released and for any current 
projects that don't want to wait for C# 8.0 to be released.

This is done using [Reflection](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection) and as such "high performance" is not the goal of this project. This library does aim to focus on:

* Increase developer productivity due to reduction of boiler plate
* Maintain a high degree of Readability
* Encourage Code Maintainability with static type safety APIs and constraint validation.
* Limit namespace pollution with extension methods

*Note:* If a non-invasive performance improvement can be made please make the suggestion or make a pull request.

## Examples

```csharp
void ExampleFunc()
{
    var original = new RecordExample("Record 1", 0);

    // With function is similar to C# 8.0 proposal syntax
    var newRecord = original.With(copy => copy
        .Mutate(r => r.Name, "Record 2")
        .AndMutate(r => r.Upvotes, r => r.UpVotes + 1));

    // changeset API provides a more typical fluent API chain.
    var anotherRecord = original.StartChangeSet()
        .Mutate(r => r.Name, "Record 3")
        .AndMutate(r => r.Upvotes, r => r.UpVotes + 1)
        .ToNewRecord(original);

    // using nameof bypasses expression walk performance hit but
    // incurs static type safety problem that may hinder maintainability.
    var yetAnotherRecord = original.StartChangeSet()
        .Mutate(nameof(original.Name), "Record 4")
        .AndMutate(nameof(original.Upvotes), 9001)
        .ToNewRecord(original);
}

class RecordExample : RecordBase<RecordExample> // Recordbase<T> implements IRecord<T>
{
    public string Name { get; }

    public int UpVotes { get; private set; }

    public RecordExample(string name, int upvotes)
    {
        Name = name;
        UpVotes = upvotes;
        ValidateNonVirtual(this);
    }

    public override void ThrowIfConstraintsAreViolated(RecordExample record)
    {
        ValidateNonVirtual(record);
    }

    private static ValidateNonVirtual(RecordExample record)
    {
        if(string.IsNullOrEmpty(record.Name))
        {
            throw new ArgumentNullException(nameof(Name));
        }

        if(record.UpVotes < 0)
        {
            throw new ArgumentOutOfRangeException("Must be non negative.");
        }
    }
}
```

Alternatively if you already have a base type or just hate inheritance in C#.
You can implement the IRecord<T> interface and use RecordUtil.MemberwiseClone to 
easily copy objects like so...

```csharp
class AlreadyHasABase : CommonBase, IRecord<AlreadyHasABase>
{
    AlreadyHasABase IRecord<AlreadyHasABase>.ShallowCopy()
    {
        // RecordUtil exposes MemberwiseClone for easy shallow copies.
        // Note: It is yet another performance drag so it is a good candidate
        // for an optimization of each class to implement a constructor that
        // copies each field.
        return RecordUtil.MemberwiseClone(this);
    }

    void ThrowIfConstraintsAreViolated(AlreadyHasABase record)
    {
        // constraints
    }
}
```

## How it Works

Please refer to the IRecord<TRecord> interface. Types that implement that interface
are elligible for the extensions that take care of the API displayed above.

1. A change set is built with the Mutate functions. 
2. if there are changes in the set, then a shallow copy of the original record is made
3. The changeset is applied to the copy
4. The copy is then validated against its constraints.
5. The copy is returned.

### Concerns

This library intentionally violates the accessibility rules of IRecord<T> types. 
Private setters and readonly fields are modifiable. Normally, this would be a huge
alarm in a code base. The API of this library is meant to limit the scope of this
violation so that it is useful but not dangerous.

## Building and Testing

The solution should be buildable in VS 2017 and 2019 Community. They're .Net core projects. Visual studio code can even build and test them.

To get coverage I ran the following command:

```
dotnet test /p:CollectCoverage=true /p:Exclude=[NUnit3.TestAdapter]\* /p:CoverletOutputFormat=\"json,opencover,lcov\" /p:CoverletOutput='./.coverage/'
```

This should generate a json, lcov, and opencover file for various coverage displays. I prefer Axocover but it does not support .Net Core yet.
I ended up using the following VS code extensions.

* [Coverage Gutters](https://marketplace.visualstudio.com/items?itemName=ryanluker.vscode-coverage-gutters)
* [.Net Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer)

To display code with Coverage gutters I had to chagne a setting to look for coverage.info as well as the default lcov.info.

```
    "coverage-gutters.coverageFileNames": [
        "lcov.info",
        "coverage.info",
        "cov.xml",
        "jacoco.xml"
      ]
```

The test explorer settings have test arugment setting that can be set. I wish it was per repo.

```
"dotnet-test-explorer.testArguments": "/p:CollectCoverage=true /p:Exclude=[NUnit3.TestAdapter]\\* /p:CoverletOutputFormat=\\\"json,opencover,lcov\\\" /p:CoverletOutput=\\\"./.coverage/\\\""
```




