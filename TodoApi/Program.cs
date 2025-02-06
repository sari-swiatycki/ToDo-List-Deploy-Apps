using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("server=localhost;user=root;password=Diti327770038!;database=ToDoDB",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "A simple ToDo API to manage tasks."
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod() 
              .AllowAnyHeader(); 
    });
});

var app = builder.Build();
app.UseSwagger();

// הפעלת Swagger UI
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    options.RoutePrefix = string.Empty;  
});


// הפעלת מדיניות CORS
app.UseCors("AllowSpecificOrigins");

app.MapGet("/tasks", async (ToDoDbContext context) =>
{
    var tasks = await context.Items.ToListAsync();
    return tasks.Any() ? Results.Ok(tasks) : Results.NoContent();
});

// הוספת משימה חדשה
app.MapPost("/tasks", async (ToDoDbContext context, Item newTask) =>
{
    context.items.Add(newTask);
    await context.SaveChangesAsync();
    return Results.Created($"/tasks/{newTask.Id}", newTask);
});

// עדכון משימה
app.MapPut("/tasks/{id}", async (int id, ToDoDbContext context, Item updatedTask) =>
{
    var task = await context.items.FindAsync(id);
    if (task == null) return Results.NotFound();
    task.IsComplete = updatedTask.IsComplete;
    await context.SaveChangesAsync();
    return Results.Ok(task);
});

// מחיקת משימה
app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext context) =>
{
    var task = await context.items.FindAsync(id);
    if (task == null) return Results.NotFound();
    context.items.Remove(task);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/", () => "TodoApi is running");
app.Run();
