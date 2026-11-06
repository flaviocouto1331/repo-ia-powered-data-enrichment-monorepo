using api_jogos.Endpoint;
using api_jogos.Extension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* flaviocouto1331 */
builder.Services.AddDapperExtension();
builder.Services.AddJogosExtension();

/* flaviocouto1331 */
//builder.Services.AddHttpClient();
builder.Services.AddHttpClient(string.Empty, client =>
{
    client.Timeout = TimeSpan.FromMinutes(3);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/* flaviocouto1331 */
app.AddJogosEndpoint();

app.Run();