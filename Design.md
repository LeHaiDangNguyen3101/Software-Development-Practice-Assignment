# Design

Complete this document for **Part B**.

## 1. Saved recipe collection

I chose `HashSet<int>` to store saved recipe IDs because it automatically prevents duplicate IDs. I use `Add()` to save a recipe and `Remove()` to remove it from the collection. Moreover, I also use `Contains()` to check whether a recipe is already saved. These operations have an average time complexity of O(1), which makes `HashSet<int>` an efficient choice for this feature.

## 2. Integration

In Part B, I used the same `RecipeManager` and recipe dictionary from Part A. The `SearchByTitle()` and `SearchByIngredient()` methods use LINQ to search through the existing recipe catalogue. The `GetHighestProteinRecipes()` method uses the same catalogue to find recipes with the highest protein values. The `AddSavedRecipe()` method checks whether a recipe exists before saving its ID in `_savedRecipes`. Finally, my integrated test searches for a recipe, saves it, checks whether it is saved, removes it, and checks again to make sure it is no longer saved.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | HashSet<int> |Average O(1) | Hash-based membership lookup |
