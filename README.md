![Logo](../release/logo.png)

# Intro
This project was inspired by the source code for the system-intern immutable collections. It expands on the idea of a strong enumerable with a strong enumerator instead of the usual interface. This improves performance in foreach-loops massively the bigger the dataset gets. The library additionally adds some minor extension methods for all collections.

# Explanation
## Sidenote
If you want the most performance possible for any enumerable type, refrain from using the ```yield``` keyword. While being a useful tool, performance-wise it is a nightmare.

## No interface needed
When implementing a collection we usually create an object, wheter it be a class or a struct, which implements the ```IEnumerable<T>``` interface. This also forces us to implement two methods ```IEnumerable.GetEnumerator()``` and ```IEnumerable<T>.GetEnumerator()```. We can now pass our object to various methods and iterate through it by using a foreach-loop. What most people probably don't know is that you technically don't need the interface. The compiler will recognize an object as foreach-iterable when it contains a ```GetEnumerator()``` method, which returns another object that contains a ```MoveNext()``` method returning ```Boolean``` as well as a property ```Current``` of type ```T```. Some may already have realized that this is just the ```IEnumerable<T>``` and the ```IEnumerator<T>``` interfaces minus the ```IEnumerator.Reset()``` method and without implementing the ```IDisposable``` interface.  
With that in mind the following code is actually a valid enumerable class and compiles.  
```csharp
public class WithoutInterfaceEnumerable
{
    public WithoutInterfaceEnumerator GetEnumerator()
    {
        return new WithoutInterfaceEnumerator(this);
    }

    public struct WithoutInterfaceEnumerator
    {
        public WithoutInterfaceEnumerator(WithoutInterfaceEnumerable source)
        {
            ...
        }

        public Boolean MoveNext()
        {
            ...
        }

        public Object Current
        { 
            get;
            private set;
        }
    }
}
```  
## Compilation
What happens when the compiler sees a foreach-loop? He resolves it, but what does he resolve it into? Arrays are actually a special case in that the compiler transforms them into a while loop like in the example below.  
```csharp
Int32[] array = new Int32[] { ... };
foreach (Int32 value in array)
{
    ...
}
```
**becomes...**
```csharp
Int32[] array = new Int32[] { ... };
Int32 index = 0;
while (index < array.Length)
{
    Int32 value = array[index];
    ...
    index++;
}
```
While this might not be the exact translation, this is roughly what the compiler generates.  
Now what happens when the compiler sees a foreach-loop for any other enumerable type? He calls the ```GetEnumerator()``` method of that type and uses the returned enumerator to iterate through the enumerable type.
```csharp
IEnumerable<Int32> enumerable = new Int32[] { ... };
foreach (Int32 value in enumerable)
{
    ...
}
```
**becomes...**
```csharp
IEnumerable<Int32> enumerable = new Int32[] { ... };
IEnumerator<Int32> enumerator = enumerable.GetEnumerator();
while (enumerator.MoveNext())
{
    Int32 value = enumerator.Current;
    ...
}
```  
So far so good. Now where do we actually lose perfomance in this scenario? The thing is, the interfaces themselves are the bottleneck. Interfaces are by definition an abstraction. All they do is tell the compiler that the object should have certain members, but nothing more. The compiler has to find the implementation whitin the object itself, as well as any possible base-calls or overrides. This is a "virtual call" which costs time to perform. But with modern computers the performance hit is actually quite minimal, so where goes all the performance?  
Well the thing is while ```GetEnumerator()``` gets called **once**, ```MoveNext()``` and ```Current``` get called *n*-times, where *n* is the amount of items in the enumerable type. What might have been 5µs performance cost for 1 item can quickly become 5000µs performance cost for 1000 items. This can escalate very quickly for bigger datasets and severly impact the performance of loops.

## The solution
Combining the knowledge we acquired to build a faster enumerable will lead us to use strongly typed enumerators for the enumeration. This means we replace the return type of ```GetEnumerator()``` with an explicit type instead of an interface. We will take the cost of **one** virtual call by implementing a new interface ```IStrongEnumerable<TElement, TEnumerator>``` and provide an explicit type for the return type of the ```GetEnumerator()``` method.  
```csharp
public interface IStrongEnumerable<TElement, TEnumerator>
    where TEnumerator : struct, IStrongEnumerator<TElement>
{
    public TEnumerator GetEnumerator();
}
```  
The interface requires the enumerator type to be a struct. The reason for this lies again in the "virtual calls". Why? Because a class could be abstract or the ```GetEnumerator()``` method could be virtual. This would force the compiler yet again to make a "virtual call" to search for the complete implementation. A struct can be neither a parent nor a child type. Therefore calls that are made towards a struct will always be direct.  
The returned enumerator will now only required the implementation of the ```MoveNext()``` method and the ```Current``` property, which are included in the ```IStrongEnumerator<TElement>``` interface.  
```csharp
public interface IStrongEnumerator<TElement>
{
    public Boolean MoveNext();

    public TElement Current { get; }
}
```  
Every enumerable type that implements the ```IStrongEnumerable<TElement, TEnumerator>``` interface should implement it **implicitly**. Otherwise the improved performance of the explicit enumerator type will get lost. Likewise the members of ```IStrongEnumerator<TElement>``` should also be implemented **implicitly**.

# Metrics
Benchmarks were run multiple times under controlled conditions in the exact same environment. Collections would be preallocated with 1.000.000 randomized elements before the benchmarks would be run.
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.418 (1909/November2019Update/19H2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-preview.4.22252.9
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT


```
## Control Benchmarks
### Array
***Fastest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 421.5 μs | 1.48 μs | 1.31 μs |         - |
| ForeachLoop | 221.4 μs | 1.54 μs | 1.29 μs |         - |

***Slowest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 426.0 μs | 2.77 μs | 2.59 μs |         - |
| ForeachLoop | 235.8 μs | 2.91 μs | 2.72 μs |         - |

### Collection
***Fastest Measurement***  

|      Method |     Mean |     Error |    StdDev | Allocated |
|------------ |---------:|----------:|----------:|----------:|
|     ForLoop | 2.552 ms | 0.0239 ms | 0.0224 ms |       2 B |
| ForeachLoop | 3.800 ms | 0.0117 ms | 0.0110 ms |      42 B |

***Slowest Measurement***  

|      Method |     Mean |     Error |    StdDev | Allocated |
|------------ |---------:|----------:|----------:|----------:|
|     ForLoop | 2.760 ms | 0.0093 ms | 0.0087 ms |       2 B |
| ForeachLoop | 4.048 ms | 0.0297 ms | 0.0263 ms |      44 B |

### List
***Fastest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 424.1 μs |  1.83 μs |  1.71 μs |         - |
| ForeachLoop | 641.9 μs | 3.99 μs | 3.74 μs |       1 B |

***Slowest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 426.4 μs | 2.89 μs | 2.56 μs |         - |
| ForeachLoop | 650.4 μs | 11.55 μs | 10.81 μs |       1 B |

## New Collections
```ReadOnlyCollection<TElement>``` and ```ReadOnlyList<TElement>``` are both readonly structs, which might be an unfair comparison at first glance. However looking at the ```ObservableList<TElement>``` it becomes clear, that the performance is similar for all enumerable types that are implemented in this manner.
### ReadOnlyCollection
***Fastest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
| ForeachLoop | 495.0 μs | 1.04 μs | 0.81 μs |         - |

***Slowest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
| ForeachLoop | 496.4 μs | 1.82 μs | 1.62 μs |         - |

### ReadOnlyList
***Fastest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 422.1 μs | 1.56 μs | 1.38 μs |         - |
| ForeachLoop | 495.4 μs | 1.58 μs | 1.48 μs |         - |

***Slowest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 425.4 μs | 4.67 μs | 4.37 μs |         - |
| ForeachLoop | 497.1 μs | 1.80 μs | 1.59 μs |         - |

### ObservableList
***Fastest Measurement*** 

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 424.7 μs | 3.35 μs | 2.80 μs |         - |
| ForeachLoop | 423.5 μs | 1.68 μs | 1.57 μs |         - |

***Slowest Measurement***  

|      Method |     Mean |   Error |  StdDev | Allocated |
|------------ |---------:|--------:|--------:|----------:|
|     ForLoop | 449.3 μs | 8.86 μs | 6.92 μs |         - |
| ForeachLoop | 445.6 μs | 2.88 μs | 2.70 μs |         - |

## Conclusion
Looping through an array is by all means the fastest, which should be no surprise. What is interesting however is the fact that the foreach-loop is faster than a for-loop. This might be some optimizations that the emitted IL bytecode contains, which aren't visible on the surface.  
Continuing on to the ```Collection<T>``` we can clearly see the impact that the delegation through the "virtual calls" has. In comparison to the array we are now taking 4-digit ms of time.  
We'll quickly continue to the ```List<T>```, where we actually see a not-so-far-off time descrepancy with the array. The thing is, for some reason some classes in the ```System.Collections``` namespace actually contain an explicitly typed enumerator while others don't. ```List<T>``` is a prime example here as it's publicly visible ```GetEnumerator()``` method returns the ```List<T>.Enumerator``` struct, which is the exact method we are using for our new collections.  
Which is also why, if we continue onto the new collections, we can observe a mean ~500µs for every collection. The fact that the ```List<T>.Enumerator``` takes slightly longer might be some boilerplate.  
With this we have successfully proven the usefulness of explicitly typed enumerators.
  
# Installation
[![NuGet](https://img.shields.io/nuget/v/Narumikazuchi.Collections.svg)](https://www.nuget.org/packages/Narumikazuchi.Collections)[![Github](https://img.shields.io/badge/github-package-f34b7d)](https://github.com/Narumikazuchi/Collections/packages/1386993)  
The installation can be simply done via installing the nuget package, the github package or by downloading the latest release here from github and referencing it in your project.