using BooksWebAPI.Data;
using BooksWebAPI.Models;
using BooksWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BooksDb"));
});
//Database Configuration

//OpenAPI Configuration
builder.Services.AddOpenApi();
//OpenAPI Configuration

//DI
builder.Services.AddScoped<IBookService, BookService>();
//DI

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
   
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Books API V1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.MapGet("/books", async (IBookService BookService) =>
{
    try
    {
        var books = await BookService.GetAllBooksAsync();
        if (books == null || !books.Any())
        {
            return Results.NotFound();
        }
        return Results.Ok(books);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);

    }    
});
app.MapGet("/books/{id}", async (Guid id, IBookService BookService) =>
{
    try
    {
        var book = await BookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(book);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
app.MapPost("/books/addbook", async (Book book, IBookService BookService) =>
{
    try
    {
        var newBook = await BookService.AddBookAsync(book);
        return Results.Created($"/books/{newBook?.Id}", newBook);

    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
    
});
app.MapPut("/books/updatebook/{id}", async (Guid id, Book book, IBookService BookService) =>
{
    try
    {
        var updatedBook = await BookService.UpdateBookAsync(id, book);
        if (updatedBook == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(updatedBook);

    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
    
});
app.MapDelete("/books/deletebook/{id}", async (Guid id, IBookService BookService) =>
{
    try
    {
        var exist=await BookService.DeleteBookAsync(id);
        if(exist)
        {
            return Results.Ok();
        } 
        return Results.NoContent();

    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }    
});

app.Run();
