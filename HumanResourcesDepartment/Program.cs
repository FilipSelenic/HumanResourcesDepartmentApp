using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using FluentValidation;
using HumanResourcesDepartment;
using HumanResourcesDepartment.Models;
using HumanResourcesDepartment.Models.DTO;
using HumanResourcesDepartment.Repository;
using HumanResourcesDepartment.Repository.Interfaces;
using HumanResourcesDepartment.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("AppConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var Configuration = builder.Configuration.GetSection("Jwt");

// Adding Authentication  
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer  
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = Configuration["Audience"],
        ValidIssuer = Configuration["Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Key"]))
    };
});


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmployeesService>();



builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();




app.MapPost("/api/login",  ( AuthService authService, [FromBody] LoginDTO loginDTO) => 
{
    var result = authService.LoginUser(loginDTO);
    return result;
}).Accepts<LoginDTO>("application/json").Produces(200).Produces(400).Produces(401);


app.MapPost("/api/register", (AuthService authService, [FromBody] RegistrationDTO registrationDTO) => 
{
    var result = authService.RegisterUser(registrationDTO);
    return result;
}).Accepts<RegistrationDTO>("application/json").Produces(200).Produces(400);




app.MapGet("/api/employees", (EmployeesService employeesService) =>
{
    var result = employeesService.GetAllEmployees();
    return result;
}).WithName("GetEmployees").Produces<APIResponse>(200);


app.MapGet("/api/employees/{id:int}", (EmployeesService employeesService, int id) =>
{
    var result = employeesService.GetEmployeeById(id);
    return result;
}).WithName("GetEmployee").Produces<APIResponse>(200).Produces<APIResponse>(404);



app.MapPost("/api/employees", (EmployeesService employeesService, EmployeeCreateDTO employeeCreateDTO) =>
{
    var result = employeesService.CreateEmployee(employeeCreateDTO);
    return result;
}).WithName("CreateEmployee").Accepts<EmployeeCreateDTO>("application/json").Produces<APIResponse>(201).RequireAuthorization();



app.MapPut("/api/employees", (EmployeesService employeesService, EmployeeUpdateDTO employeeUpdateDTO) =>
{
    var result = employeesService.UpdateEmployee(employeeUpdateDTO);
    return result;
}).Produces<APIResponse>(200).Produces<APIResponse>(400).Accepts<EmployeeUpdateDTO>("application/json").RequireAuthorization();



app.MapDelete("/api/employees/{id:int}", (EmployeesService employeesService, int id) =>
{
    var result = employeesService.DeleteEmployee(id);
    return result;
}).Produces<APIResponse>(200).Produces<APIResponse>(404).RequireAuthorization();



app.Run();


