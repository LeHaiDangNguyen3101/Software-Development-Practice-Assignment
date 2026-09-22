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
    public int PendingInstructionCount => 0;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
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

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException("Part A: implement GetCookingPlan.");

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

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
