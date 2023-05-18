using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using HumanResourcesDepartment;
using HumanResourcesDepartment.Models;
using HumanResourcesDepartment.Models.DTO;
using HumanResourcesDepartment.Repository;
using HumanResourcesDepartment.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
var connectionString = builder.Configuration.GetConnectionString("AppConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/employees", (IMapper _mapper, IEmployeesRepository employeeRepository) =>
{
    var employees = employeeRepository.GetAll();
    APIResponse response = new();
    response.Result = employees.ProjectTo<EmployeeDTO>(_mapper.ConfigurationProvider).ToList();
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetEmployees").Produces<APIResponse>(200);

app.MapGet("/api/employees/{id:int}", (IMapper _mapper, IEmployeesRepository employeeRepository, int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

    var employee = employeeRepository.GetById(id);
    if (employee == null)
    {
        return Results.NotFound(response);
    }
    response.Result = _mapper.Map<EmployeeDTO>(employee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetEmployee").Produces<APIResponse>(200).Produces(404);

app.MapPost("/api/employees",  (IMapper _mapper, IValidator< EmployeeCreateDTO> _validation, IEmployeesRepository employeeRepository, [FromBody] EmployeeCreateDTO employeeCreateDTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult =  _validation.ValidateAsync(employeeCreateDTO).GetAwaiter().GetResult();
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
  
    Employee employee = _mapper.Map<Employee>(employeeCreateDTO);
    employeeRepository.Add(employee);
    response.Result = _mapper.Map<EmployeeDTO>(employee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.Ok(response);
}).WithName("CreateEmployee").Accepts<EmployeeCreateDTO>("application/json").Produces<APIResponse>(201);

app.MapPut("/api/employees", (IMapper _mapper, IValidator<EmployeeUpdateDTO> _validation, IEmployeesRepository employeeRepository, [FromBody] EmployeeUpdateDTO employeeUpdateDTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = _validation.ValidateAsync(employeeUpdateDTO).GetAwaiter().GetResult();

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    Employee employee = _mapper.Map<Employee>(employeeUpdateDTO);

    try
    {
        employeeRepository.Update(employee);
    }
    catch(DbUpdateConcurrencyException e)
    {
        response.ErrorMessages.Add(e.Message);
        return Results.BadRequest(response);
    }
    catch (Exception e)
    {
        response.ErrorMessages.Add(e.Message);
        return Results.BadRequest(response);
    }
    response.Result = _mapper.Map<EmployeeDTO>(employee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK; 
    return Results.Ok(response);
}).Produces<APIResponse>(200).Produces(400).Accepts<EmployeeUpdateDTO>("application/json");

app.MapDelete("/api/employees/{id:int}", (IEmployeesRepository employeeRepository, int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

    var existingEmployee = employeeRepository.GetById(id);
    if (existingEmployee == null)
    {
        response.ErrorMessages.Add("Invalid id");
        return Results.NotFound(response);
    }

    employeeRepository.Delete(existingEmployee);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);
}).Produces(200).Produces(404);

app.Run();


