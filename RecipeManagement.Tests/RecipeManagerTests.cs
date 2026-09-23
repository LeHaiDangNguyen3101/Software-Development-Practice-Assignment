using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddIngredients_AddIngredientsToShoppingList()
    {
        var manager = CreateManager();
        int added = manager.AddIngredientsToShoppingList(10);
        Assert.Equal(1, added);
        Assert.Equal(1, manager.ShoppingItemCount);
        Assert.Equal("1 apple", manager.GetShoppingList()[0]);
    
    }

    [Fact]
    public void AddRecipe_DuplicatedId_ReturnFalse()
    {
        var manager = CreateManager();
        var recipe = new Recipe
        {
            Id = 10,
            Title = "Pizza"
        };

        bool result = manager.AddRecipe(recipe);

        Assert.False(result);
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void FindRecipe_MissingId_RemoveRecipe()
    {

        var manager = CreateManager();
        Recipe? recipe = manager.FindRecipe(99);
        Assert.Null(recipe);

    }

    [Fact]
    public void AddRecipeToCookingPlan_DulicatedId_returnFalse()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        bool result = manager.AddRecipeToCookingPlan(10);

        Assert.False(result);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void PeekLastRecipe_EmptyHistory_returnNull()
    {
        var manager = CreateManager();
        int? result = manager.PeekLastRemovedRecipe();
        Assert.Null(result);
    }

    [Fact]
    public void PeekNextInstruction_EmptyQueue_returnNull()
    {
        var manager = CreateManager();
        string? instruction = manager.PeekNextInstruction();
        Assert.Null(instruction);
    }

    [Fact]
    public void RemoveRecipe_RecipeInCookingPlan_returnNotNull()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        bool removed = manager.RemoveRecipe(10);

        Assert.False(removed);
        Assert.Equal(2, manager.RecipeCount);
        Assert.NotNull(manager.FindRecipe(10));
    }

    [Fact]
    public void ClearShoppingList_ListHasItem_return0()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        manager.ClearShoppingList();
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void CompleteNextInstruction_AllInstructionsDone_EmptyQueue()
    {
        var manager = CreateManager();
        manager.StartCooking(10);
        manager.CompleteNextInstruction();
        manager.CompleteNextInstruction();
        
        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Null(manager.PeekNextInstruction());
    }

    [Fact]
    public void AddRecipe_ValidRecipe_ReturnTrue()
    {
        var manager = CreateManager();
        var recipe = new Recipe
        {
            Id = 30,
            Title = "Pasta"
        };

        bool result = manager.AddRecipe(recipe);

        Assert.True(result);
        Assert.Equal(3, manager.RecipeCount);
        Assert.NotNull(manager.FindRecipe(30));
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
