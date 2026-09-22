using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // TODO Part A: add your private collection fields here.
    private Dictionary<int, Recipe> _recipes = new(); 
    private List<string> _shoppingList = new();
    private LinkedList<int> _cookingPlan = new();
    private Stack<int> _removedRecipes = new();
    private Queue<string> _pendingInstructions = new();
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.
        if (recipes == null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }
        foreach (Recipe recipe in recipes)
        {
            if (recipe.Id <= 0)
            {
                throw new ArgumentException();
            }
            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException();
            }
            if (_recipes.ContainsKey(recipe.Id))
            {
                throw new ArgumentException();
            }
            _recipes.Add(recipe.Id, recipe);
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _pendingInstructions.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException();
        }
        if (recipe.Id <= 0)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }

        if (_recipes.ContainsKey(recipe.Id))
        {
            return false;
        }
        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        if (_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return recipe;
        }
        return null;
    }


    public bool RemoveRecipe(int recipeId)
    {
        if (_cookingPlan.Contains(recipeId))
        {
            return false;
        }
        bool removed = _recipes.Remove(recipeId);
        return removed;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe == null)
        {
            return 0;
        }
        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
        }
        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
       return new List<string>(_shoppingList); 
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }
    public bool AddRecipeToCookingPlan(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return false;
        }

        if (_cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }
    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        bool removed = _cookingPlan.Remove(recipeId);
        if (removed == true)
        {
            _removedRecipes.Push(recipeId);
        }
        return removed;
    }
        
    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false;
        }

        if (FindRecipe(_removedRecipes.Peek()) == null)
        {
            return false; 
        }
        if (_cookingPlan.Contains(_removedRecipes.Peek()))
        {
            return false;
        }

        int value = _removedRecipes.Pop();
        _cookingPlan.AddLast(value);
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return null;
        }
        return _removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(_cookingPlan);
    }
    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe == null)
        {
            return false;
        }
        if (recipe.Instructions.Count == 0)
        {
            return false; 
        }
        _pendingInstructions.Clear();
        foreach (string pendingInstruction in recipe.Instructions)
        {
            _pendingInstructions.Enqueue(pendingInstruction);
        }
        return true; 
    }
    
    public string? PeekNextInstruction()
    {
        if (_pendingInstructions.Count == 0)
        {
            return null;
        }
        return _pendingInstructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (_pendingInstructions.Count == 0)
        {
            return null;
        }
        string nextInstruction = _pendingInstructions.Dequeue();
        return nextInstruction;
    }
    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
