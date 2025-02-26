var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var colors = new[]
{
    "red", "blue", "green", "pink", "white", "black"
};

var brands = new[]
{
    "daica", "opel", "ford"
};

app.MapGet("/car", () =>
    {
        var car = Enumerable.Range(1, 5).Select(index =>
                new Car
                (
                    index,
                    colors[Random.Shared.Next(colors.Length)],
                    $"car with id {index}",
                    brands[Random.Shared.Next(brands.Length)],
                    Random.Shared.Next(0, 5).ToString(),
                    Random.Shared.Next(1970, 2024),
                    Random.Shared.Next(10000, 50000)
                ))
            .ToArray();
        return car;
    })
    .WithName("GetCar")
    .WithOpenApi();

app.Run();

class Car
{
    public int Id { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int ProductionYear { get; set; }
    public float Price { get; set; }
    
    public Car(int id, string color, string description, string brand, string model, int productionYear, float price)
    {
        Id = id;
        Color = color;
        Description = description;
        Brand = brand;
        Model = model;
        ProductionYear = productionYear;
        Price = price;
    }
}