# Simple decision tree classifier written completely in C#

*The idea was taken from video by Josh from Google devs (https://www.youtube.com/watch?v=LDRbO9a6XPU&t=122s)*

## How to use

1. **Load Dataset** with Datarow class static functions like `GetDatarowsFromCSV(path_to_csv)`
2. Create a **DecisionTree** object by passing Dataset into constructor
3. Build **decision tree classifier** by calling method `BuildTree()` on your DecisionTree object
4. **Classify data** using `Classify(datarow)` method on your DecisionTree object

You can find **examples** of using this library in **Program.cs** (including comments)

## To-do
* **DONE** ~~rewrite algorithms to use virtual stack or be non-recursive because of stackoverflow exception~~ 
* Save decision tree to file (probably via binary convertor?)
* Print decision tree to file (pdf?)
