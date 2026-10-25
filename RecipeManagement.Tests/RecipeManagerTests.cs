using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

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
    public void AddRecipe_DuplicateId_ReturnFalse()
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
    public void AddRecipeToCookingPlan_DulicateId_ReturnFalse()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        bool result = manager.AddRecipeToCookingPlan(10);

        Assert.False(result);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void PeekLastRecipe_EmptyHistory_ReturnNull()
    {
        var manager = CreateManager();
        int? result = manager.PeekLastRemovedRecipe();
        Assert.Null(result);
    }

    [Fact]
    public void PeekNextInstruction_EmptyQueue_ReturnNull()
    {
        var manager = CreateManager();
        string? instruction = manager.PeekNextInstruction();
        Assert.Null(instruction);
    }

    [Fact]
    public void RemoveRecipe_RecipeInCookingPlan_ReturnNotNull()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        bool removed = manager.RemoveRecipe(10);

        Assert.False(removed);
        Assert.Equal(2, manager.RecipeCount);
        Assert.NotNull(manager.FindRecipe(10));
    }

    [Fact]
    public void ClearShoppingList_ListHasItem_Return0()
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

    [Fact]
    public void RemoveRecipe_ValidRecipe_ReturnTrue()
    {
        var manager = CreateManager();
        bool removed = manager.RemoveRecipe(20);

        Assert.True(removed);
        Assert.Equal(1,manager.RecipeCount);
        Assert.Null(manager.FindRecipe(20));

    }
    [Fact]
    public void SearchByTitle_MatchingName_ReturnsResult()
    {
        RecipeManager manager = CreateManager();
        var results = manager.SearchByTitle("Recipe");
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void SearchByTitle_LowercaseSearch_ReturnsMatches()
    {
        RecipeManager manager = CreateManager();
        var results = manager.SearchByTitle("recipe");
        Assert.Equal(2, results.Count);
    }

        [Fact]
    public void SearchByTitle_NoMatchingRecipes_ReturnsEmptyList()
    {
        RecipeManager manager = CreateManager();
        var results = manager.SearchByTitle("pizza");
        Assert.Empty(results);;
    }

    [Fact]
    public void SearchByIngredient_MatchingIngredient_ReturnsResult()
    {
        RecipeManager manager = CreateManager();
        var results = manager.SearchByIngredient("apple");
        Assert.Single(results);
    }

    [Fact]
    public void SearchByIngredient_UppercaseSearch_ReturnsResult()
    {
        RecipeManager manager = CreateManager();
        var results = manager.SearchByIngredient("APPLE");
        Assert.Single(results);
    }

    [Fact]
    public void SearchByIngredient_NoMatchingIngredient_ReturnsEmptyList()
    {
        RecipeManager manager = CreateManager();
        var results = manager.SearchByIngredient("banana");
        Assert.Empty(results);;
    }

    [Fact]
    public void GetHighestProteinRecipes_TopTwo_ReturnsDescendingOrder()
    {
        // Arrange
        RecipeManager manager = new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Recipe A",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 15
                }
            },

            new Recipe
            {
                Id = 2,
                Title = "Recipe B",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 30
                }
            },

            new Recipe
            {
                Id = 3,
                Title = "Recipe C",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 10
                }
            }

        });

        // Act
        var results = manager.GetHighestProteinRecipes(2);

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results[0].Id);
        Assert.Equal(1, results[1].Id);
    }

    [Fact]
    public void GetHighestProteinRecipes_MissingNutrition_ExcludesRecipe()
    {
        // Arrange
        RecipeManager manager = new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Recipe A",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 15
                }
            },

            new Recipe
            {
                Id = 2,
                Title = "Recipe B",
            },

            new Recipe
            {
                Id = 3,
                Title = "Recipe C",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 10
                }
            }

        });

        // Act
        var results = manager.GetHighestProteinRecipes(3);

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[0].Id);
        Assert.Equal(3, results[1].Id);
    }

    [Fact]
    public void GetHighestProteinRecipes_ZeroCount_ReturnsEmptyList()
    {
        // Arrange  
        RecipeManager manager = CreateManager();
        // Act
        var results = manager.GetHighestProteinRecipes(0);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void GetHighestProteinRecipes_MissingProteinInformation_ExcludesRecipe()
    {
        // Arrange
        RecipeManager manager = new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Recipe A",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 15
                }
            },
            new Recipe
            {
                Id = 2,
                Title = "Recipe B",
                Nutrition = new NutritionInfo
                {
                    CaloriesKcal = 200
                }
            }

        });

        // Act
        var results = manager.GetHighestProteinRecipes(2);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
        
    }

    [Fact]
    public void GetHighestProteinRecipes_NegativeCount_ReturnsEmptyList()
    {
        // Arrange  
        RecipeManager manager = CreateManager();
        // Act
        var results = manager.GetHighestProteinRecipes(-1);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void AddSavedRecipe_ExistingRecipe_ReturnsTrue()
    {
        // Arrange
        RecipeManager manager = CreateManager();

        // Act
        bool results = manager.AddSavedRecipe(10);
        // Assert
        Assert.True(results);
    }

    [Fact]
    public void AddSavedRecipe_DuplicateRecipe_ReturnsFalse()
    {
        // Arrange
        RecipeManager manager = CreateManager();

        // Act
        bool results = manager.AddSavedRecipe(10);
        bool result = manager.AddSavedRecipe(10);
        // Assert
        Assert.True(results);
        Assert.False(result);
    }

    [Fact]
    public void AddSavedRecipe_NonexistentRecipe_ReturnsFalse()
    {
        // Arrange
        RecipeManager manager = CreateManager();

        // Act
        bool results = manager.AddSavedRecipe(99);
        // Assert
        Assert.False(results);
    }
    
    [Fact]
    public void RemoveSavedRecipe_ExistingSavedRecipe_ReturnsTrue()
    {
        RecipeManager manager = CreateManager();
        bool results = manager.AddSavedRecipe(10);
        bool result = manager.RemoveSavedRecipe(10);
        Assert.True(result);
    }

    [Fact]
    public void RemoveSavedRecipe_RemoveTwice_ReturnsFalse()
    {
        RecipeManager manager = CreateManager();
        bool added = manager.AddSavedRecipe(10);
        bool result = manager.RemoveSavedRecipe(10);
        bool results = manager.RemoveSavedRecipe(10);
        Assert.True(result);
        Assert.False(results);
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
