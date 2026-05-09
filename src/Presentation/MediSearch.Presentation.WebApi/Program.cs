using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicies.DynamicCors);

app.UseStaticFiles();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseRequestLocalization();

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/scalar"));
app.MapHub<AppHub>(AppHub.Route);
app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);

app.Run();
