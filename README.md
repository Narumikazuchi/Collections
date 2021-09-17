![Logo](../master/logo.png)

# Utility Library
This library originally contained all classes that I used in my projects more than once. I made them universal instead of being hardcoded for the project in question and added them to the library in order to reuse them whenever needed. With time the library grew and not all aspects were needed in every project. That's when I decided to split them into organized individual pieces and also publish them on github as well as nuget.org.

# Collections

The collection library has it's roots in the use of the Trie in an file-indexing project of mine. While the generic collections from the .NET library provide all the functionality that I needed so far, sometimes I wanted to limit the exposed functionality. For example I wanted to create a subtype of a List<T> but did not want the BinarySearch to be available. Maybe not the best example but that was basically the reason for creating a whole lot of base classes to extend from.
  
## Installation
### Abstract
[![NuGet](https://img.shields.io/nuget/v/Narumikazuchi.Collections.Abstract.svg)](https://www.nuget.org/packages/Narumikazuchi.Collections.Abstract)  
The installation can be simply done via installing the nuget package or by downloading the latest release here from github and referencing it in your project.
### Collections
[![NuGet](https://img.shields.io/nuget/v/Narumikazuchi.Collections.svg)](https://www.nuget.org/packages/Narumikazuchi.Collections)  
The installation can be simply done via installing the nuget package or by downloading the latest release here from github and referencing it in your project.
