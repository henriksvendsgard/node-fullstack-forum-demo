using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Konfigurer JSON-opsjoner om ønskelig
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

// Aktiver CORS
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// In-memory list for posts
var posts = new List<Post>
{
    new Post { Id = 1, Author = "Seigmann", Content = "Au! Noen spiste beina mine!" },
    new Post { Id = 2, Author = "ikke Henrik", Content = "Jeg har ikke spist noen bein..." }
};

// GET /api/posts
app.MapGet("/api/posts", () => Results.Json(posts));

// POST /api/posts
app.MapPost("/api/posts", (PostDto input) =>
{
    if (string.IsNullOrWhiteSpace(input.Author) || string.IsNullOrWhiteSpace(input.Content))
    {
        return Results.BadRequest(new { error = "Navn og innhold er påkrevd på server." });
    }

    var newPost = new Post
    {
        Id = posts.Count + 1,
        Author = input.Author,
        Content = input.Content
    };

    posts.Add(newPost);
    return Results.Created($"/api/posts/{newPost.Id}", newPost);
});

app.Run();

// Model for input og lagring
record Post
{
    public int Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

record PostDto(string Author, string Content);
